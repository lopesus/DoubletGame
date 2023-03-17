using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using CommonLibTools.Libs;
using CommonLibTools.Libs.DataStructure.Dawg.Construction;
using MongoDB.Driver;
using WiktionaireParser.Models;

namespace WiktionaireParser.UiControls
{
    /// <summary>
    /// Interaction logic for WiktioParser.xaml
    /// </summary>
    public partial class WiktioParser : UserControl
    {
        IMongoDatabase Database;
        IMongoCollection<WikiPage> WikiCollection;
        IMongoCollection<Anagram> AnagramCollection;
        IMongoCollection<WordFrequency> WordFrequencyCollection;
        string wikiCollectionName;
        AnagramBuilder anagramBuilder;
        WordFrequencyBuilder frequencyBuilder;
        private string WiktioParserResultFolder;


        Dictionary<string, bool> correctWikiPageWords = new Dictionary<string, bool>();
        public List<WikiPage> PagesList = new List<WikiPage>();

        int pageToLoadFromDb = Int32.MaxValue;
        int pageToSkipFromDb = 0;
        private int pageToParseCount = Int32.MaxValue;

        private string officialScrabbleDico = @"D:\zzzWiktionnaire\DICO\ODS8.txt";
        private Dictionary<string, bool> officiaScrabbleWordList = new Dictionary<string, bool>();
        public WiktioParser()
        {
            InitializeComponent();

            txtTest.Text2 = "demo";
            txtTest.Header = "header";

            cbxTest2.ItemsSource= Enumerable.Range(1,10).ToList();

            Database = MainWindow.Database;
            WikiCollection = MainWindow.WikiCollection;
            AnagramCollection = MainWindow.AnagramCollection;
            WordFrequencyCollection = MainWindow.WordFrequencyCollection;
            wikiCollectionName = MainWindow.wikiCollectionName;
            anagramBuilder = MainWindow.anagramBuilder;
            frequencyBuilder = MainWindow.frequencyBuilder;
            WiktioParserResultFolder = MainWindow.WiktioParserResultFolder;
            Database = MainWindow.Database;

            officiaScrabbleWordList = File.ReadAllLines(officialScrabbleDico).ToList().ToDictionary(s => s.ToLowerInvariant(), s => true);


        }

        private void cmdParse_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = cbxTest2.GetSelectedItem();
            var ddd = selectedItem.ToString();
            var dddg = txtTest.Text2;
            return;
            Database.DropCollection(wikiCollectionName);
            Database.DropCollection(AnagramCollection.CollectionNamespace.CollectionName);
            WikiCollection = Database.GetCollection<WikiPage>(wikiCollectionName);

            anagramBuilder = new AnagramBuilder();

            Stopwatch stopwatch = new Stopwatch();
            // Avvia il timer
            stopwatch.Start();
            SplitDicoToPage(MainWindow.WiktioFileName);
            stopwatch.Stop();
            //remove non valid word in frequency builder

            frequencyBuilder.CheckAllWord(correctWikiPageWords);

            SaveToDB(PagesList);
            SaveWordBoxdico(3, 15, true);
            SaveWordBoxdico(4, 7,true);
            SaveWordBoxdico(4, 8,true);
            MessageBox.Show($"Parse Wiki Dump in {stopwatch.Elapsed.TotalMinutes} minutes");
            LoadPagesFromDb();
        }

        private void SaveWordBoxdico(int minWordLen, int maxWordLen, bool createTrie = false)
        {
            var list = PagesList.Where(p => p.TitleInv.Length >= minWordLen && p.TitleInv.Length <= maxWordLen)
                .Select(p => p.TitleInv)
                .OrderBy(p => p.Length).ThenBy(p => p)
                .ToList();

            var builder = new StringBuilder();
            foreach (var text in list)
            {
                builder.AppendLine(text);
            }

            var path = $"{WiktioParserResultFolder}/wordbox_valid_word_{minWordLen}_{maxWordLen}.txt";
            var contents = builder.ToString();
            File.WriteAllText(path, contents);
            if (createTrie)
            {
                ConstructTrie constructTrie = new ConstructTrie();
                constructTrie.GenerateTrieFromFile(path);
            }
        }

        public void SplitDicoToPage(string fileName)
        {

            int pageCount = 0;
            //count = int.MaxValue;
            var builder = new StringBuilder();
            SectionBuilder sectionBuilder = new SectionBuilder();
            Dictionary<string, List<WikiPage>> ExistingWord = new Dictionary<string, List<WikiPage>>();
            using (StreamReader sr = File.OpenText(fileName))
            {
                string line = String.Empty;
                while ((line = sr.ReadLine()) != null && pageCount <= pageToParseCount)
                {
                    if (line.Trim().StartsWith("<page>"))
                    {
                        builder.Clear();
                        builder.AppendLine(line);


                        do
                        {
                            line = sr.ReadLine();
                            builder.AppendLine(line);
                        } while (line != null && line.Trim().StartsWith("</page>") == false);

                        var page = builder.ToString();
                        pageCount++;

                        //Console.WriteLine(page);

                        XmlDocument document = new XmlDocument();
                        document.LoadXml(page);

                        var title = "";
                        var text = "";
                        try
                        {
                            var nodeList = document.GetElementsByTagName("title");
                            title = nodeList[0].InnerText.Trim();


                            if (title == "écru")
                            {
                                Console.WriteLine();
                            }
                            var firstChar = title[0];
                            if (title.ToLowerInvariant().Trim().ContainsOnlyLettersAToZ()

                                //char.IsLetter(firstChar)
                                //&& char.IsUpper(firstChar) == false
                                //&& title.Contains("-") == false && title.Contains(" ") == false
                                //&& title.Contains("’") == false && title.Contains("'") == false
                                //&& title.Contains(".") == false && title.Contains("/") == false
                                //&& title.Contains("(") == false && title.Contains(")") == false
                                //&& title.Contains("[") == false && title.Contains("]") == false
                                //&& title.Contains(",") == false && title.Contains("*") == false
                                )
                            {
                                nodeList = document.GetElementsByTagName("text");
                                text = nodeList[0].InnerText.Trim();

                                if (text.StartsWith("{{voir") || RegexLibFr.StartWithLangSectionRegex.IsMatch(text))
                                {
                                    if (RegexLibFr.ContainsLangSectionRegex.IsMatch(text))
                                    {
                                        var wikiPage = new WikiPage(title, text, sectionBuilder);
                                        if (wikiPage.IsVerbFlexion == false && wikiPage.TitleInv.Length >= 3 && officiaScrabbleWordList.ContainsKey(wikiPage.TitleInv))
                                        {
                                            if (ExistingWord.ContainsKey(wikiPage.TitleInv))
                                            {
                                                var pages = ExistingWord[wikiPage.TitleInv];
                                                var first = pages.First();
                                                first.AddDataFrom(wikiPage);
                                                pages.Add(wikiPage);

                                            }
                                            else
                                            {
                                                ExistingWord.Add(wikiPage.TitleInv, new List<WikiPage>(){wikiPage});

                                                PagesList.Add(wikiPage);
                                                correctWikiPageWords[wikiPage.TitleInv] = true;
                                                // anagram calculation
                                                var anagram = title.ToLowerInvariant().RemoveDiacritics();
                                                var anagramKey = anagram.SortString();
                                                anagramBuilder.Add(anagramKey, anagram);
                                            }
                                            

                                            // word frequency calculation
                                            var tokenList = text
                                                .Split(" .\n\r\t\b\0\\=?^+-*’–<>!|([{)}]#@,:;?\"'%&/".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                                                .Where(w => w.Length >= 3);

                                            foreach (var token in tokenList)
                                            {
                                                frequencyBuilder.AddWord(token.ToLowerInvariant().RemoveDiacritics());
                                            }
                                        }
                                    }

                                }
                            }


                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    }
                }
            }

            var sectionList = sectionBuilder.Sections.ToList();
            sectionList.Sort();
            File.WriteAllLines($"{WiktioParserResultFolder}/sectionList.txt", sectionList);

            var verbFlexion = sectionBuilder.VerbFlexion.ToList();
            verbFlexion.Sort();
            File.WriteAllLines($"{WiktioParserResultFolder}/VerbFlexion.txt", verbFlexion);

        }

        public async void SaveToDB(List<WikiPage> list)
        {
            var builder = new StringBuilder();
            foreach (var wikiPage in list)
            {
                builder.AppendLine(wikiPage.TitleInv);
                wikiPage.AnagramCount = anagramBuilder.GetCountFor(wikiPage.AnagramKey);
                var frequency = frequencyBuilder.GetWordFrequency(wikiPage.TitleInv);
                if (frequency != null)
                {
                    wikiPage.FrequencyCount = frequency.Count;
                    wikiPage.FrequencyTotalCount = frequency.AllWordCount;
                    wikiPage.Frequency = frequency.Frequency;
                }

                wikiPage.MostFrequentWordCount = frequencyBuilder.MostFrequentWordCount;
            }
            await WikiCollection.InsertManyAsync(list);
            var anagramsList = anagramBuilder.GetAnagramsList();

            if (anagramsList.Count > 1)
            {
                await AnagramCollection.InsertManyAsync(anagramsList);
            }

            ////save single page
            //Directory.CreateDirectory($"{WiktioParserResultFolder}/pages");
            //foreach (var wikiPage in list)
            //{
            //    var path = $"{WiktioParserResultFolder}/pages/{wikiPage.TitleInv}.txt";
            //    Debug.WriteLine($"saving {path}");

            //    try
            //    {
            //        File.WriteAllText(path, wikiPage.Text);
            //    }
            //    catch (Exception e)
            //    {
            //        Console.WriteLine(path);
            //       // throw;
            //    }
            //}

            File.WriteAllText($"{WiktioParserResultFolder}/wiki_valid_word.txt", builder.ToString());
        }

        private void LoadPagesFromDb()
        {
            PagesList = WikiCollection.Find(FilterDefinition<WikiPage>.Empty).Skip(pageToSkipFromDb).Limit(pageToLoadFromDb)
                .Sort(Builders<WikiPage>.Sort.Ascending(p => p.Title))
                .ToList();

            lbxPages.ItemsSource = PagesList;
            var anagrams = AnagramCollection.Find(FilterDefinition<Anagram>.Empty).ToList();
            anagramBuilder = new AnagramBuilder(anagrams);

            // LoadTrie();
        }

        private void lbxPages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            var page = lbxPages.SelectedItem as WikiPage;
            if (page != null)
            {
                txtPagecontent.Text = page.Text;

                //txtPageLangText.Text = page.LangText;
                //txtAntonymes.Text = page.Antonymes;
                //txtSynonymes.Text = page.Sinonymes;
                //tblWordInfos.Text = $"freq:{page.Frequency} {page.FrequencyCount} on {page.FrequencyTotalCount} max is {page.MostFrequentWordCount}";
                //GetAnagramFor(page.AnagramKey);
                //var validWord = GetAllValidWordFor(page.AnagramKey);
                //txtAllPossibleWord.Text = validWord.ToString();
            }
        }

        private void cbxTest2_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
