﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using CommonLibTools.Libs;
using CommonLibTools.Libs.DataStructure.Dawg.Construction;
using CommonLibTools.Libs.Extensions;
using MongoDB.Driver;
using WiktionaireParser.Models;
using static System.Net.Mime.MediaTypeNames;

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

            cbxTest2.ItemsSource = Enumerable.Range(1, 10).ToList();

            Database = MainWindow.Database;
            WikiCollection = MainWindow.WikiCollection;
            AnagramCollection = MainWindow.AnagramCollection;
            WordFrequencyCollection = MainWindow.WordFrequencyCollection;
            wikiCollectionName = MainWindow.WikiCollectionName;
            anagramBuilder = MainWindow.anagramBuilder;
            frequencyBuilder = MainWindow.frequencyBuilder;
            WiktioParserResultFolder = MainWindow.WiktioParserResultFolder;
            Database = MainWindow.Database;

            officiaScrabbleWordList = File.ReadAllLines(officialScrabbleDico).ToList().ToDictionary(s => s.ToLowerInvariant(), s => true);


        }

        private void cmdParse_Click(object sender, RoutedEventArgs e)
        {
            //var selectedItem = cbxTest2.GetSelectedItem();
            //var ddd = selectedItem.ToString();
            //var dddg = txtTest.Text2;
            //return;



            anagramBuilder = new AnagramBuilder();

            Stopwatch stopwatch = new Stopwatch();
            // Avvia il timer
            stopwatch.Start();
            SplitDicoToPage(MainWindow.WiktioFileName);
            stopwatch.Stop();
            //remove non valid word in frequency builder

            frequencyBuilder.CheckAllWord(correctWikiPageWords);

            Database.DropCollection(wikiCollectionName);
            Database.DropCollection(AnagramCollection.CollectionNamespace.CollectionName);
            WikiCollection = Database.GetCollection<WikiPage>(wikiCollectionName);

            if (PagesList.Any(p => p.TitleInv == "agrippa"))
            {
                Console.WriteLine();
            }
            SaveToDB(PagesList);
            SaveWordBoxdico(3, 15, true);
            SaveWordBoxdico(3, 7, true);
            SaveWordBoxdico(4, 7, true);
            SaveWordBoxdico(4, 8, true);
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
            Dictionary<string, WikiPage> ExistingWord = new Dictionary<string, WikiPage>();
            Dictionary<string, WikiPage> SelectedFinalWords = new Dictionary<string, WikiPage>();
            var onlyVerbFlexion = new List<string>();

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

                                        if (officiaScrabbleWordList.ContainsKey(wikiPage.TitleInv) && wikiPage.TitleInv.Length >= 3)
                                        {
                                            if (ExistingWord.ContainsKey(wikiPage.TitleInv))
                                            {
                                                var first = ExistingWord[wikiPage.TitleInv];
                                                first.AddDataFrom(wikiPage);

                                                if (first.IsOnlyVerbFlexion())
                                                {
                                                    SelectedFinalWords.Remove(first.TitleInv);
                                                }
                                            }
                                            else
                                            {
                                                ExistingWord.Add(wikiPage.TitleInv, wikiPage);
                                                SelectedFinalWords.Add(wikiPage.TitleInv, wikiPage);
                                            }
                                        }

                                        //if (wikiPage.IsVerbFlexion == false && wikiPage.TitleInv.Length >= 3 && officiaScrabbleWordList.ContainsKey(wikiPage.TitleInv))
                                        //{

                                        //    if (wikiPage.TitleInv == "agrippa")
                                        //    {
                                        //        Console.WriteLine();
                                        //    }

                                        //    if (ExistingWord.ContainsKey(wikiPage.TitleInv) == false)
                                        //    {
                                        //        ExistingWord.Add(wikiPage.TitleInv, wikiPage);

                                        //        PagesList.Add(wikiPage);
                                        //        correctWikiPageWords[wikiPage.TitleInv] = true;
                                        //        // anagram calculation
                                        //        var anagram = title.ToLowerInvariant().RemoveDiacritics();
                                        //        var anagramKey = anagram.SortString();
                                        //        anagramBuilder.Add(anagramKey, anagram);
                                        //    }
                                        //}
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


            var singularKeys = SelectedFinalWords.Keys.ToList().RemovePluralForm();
            //build pagelist of valid words
            foreach (var key in singularKeys)
            {
                var wikiPage = SelectedFinalWords[key];
                if (wikiPage.IsOnlyVerbFlexion() == false && wikiPage.TitleInv.Length >= 3 && officiaScrabbleWordList.ContainsKey(wikiPage.TitleInv))
                //if (wikiPage.IsVerbFlexion == false && wikiPage.TitleInv.Length >= 3 && officiaScrabbleWordList.ContainsKey(wikiPage.TitleInv))
                {
                    PagesList.Add(wikiPage);
                    correctWikiPageWords[wikiPage.TitleInv] = true;
                    // anagram calculation
                    var anagram = wikiPage.TitleInv;
                    var anagramKey = anagram.SortString();
                    anagramBuilder.Add(anagramKey, anagram);
                }
                else
                {
                    onlyVerbFlexion.Add(key);
                }
            }

            ////build pagelist of valid words
            //foreach (var wikiPage in SelectedFinalWords.Values)
            //{
            //    if (wikiPage.IsVerbFlexion == false && wikiPage.TitleInv.Length >= 3 && officiaScrabbleWordList.ContainsKey(wikiPage.TitleInv))
            //    {
            //        PagesList.Add(wikiPage);
            //        correctWikiPageWords[wikiPage.TitleInv] = true;
            //        // anagram calculation
            //        var anagram = wikiPage.TitleInv;
            //        var anagramKey = anagram.SortString();
            //        anagramBuilder.Add(anagramKey, anagram);
            //    }
            //}


            // word frequency calculation
            foreach (var wikiPage in PagesList)
            {
                var text = wikiPage.Text;
                var tokenList = text
                    .Split(" .\n\r\t\b\0\\=?^+-*’–<>!|([{)}]#@,:;?\"'%&/".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                    .Where(w => w.Length >= 3);

                foreach (var token in tokenList)
                {
                    if (SelectedFinalWords.ContainsKey(token))
                    {
                        frequencyBuilder.AddWord(token.ToLowerInvariant().RemoveDiacritics());
                    }
                }
            }

            var sectionList = sectionBuilder.Sections.ToList();
            sectionList.Sort();
            File.WriteAllLines($"{WiktioParserResultFolder}/sectionList.txt", sectionList);

            sectionList = sectionBuilder.SectionsWithNoSpace.ToList();
            sectionList.Sort();
            File.WriteAllLines($"{WiktioParserResultFolder}/SectionsWithNoSpaceList.txt", sectionList);

            var verbFlexion = sectionBuilder.VerbFlexion.ToList();
            verbFlexion.Sort();
            File.WriteAllLines($"{WiktioParserResultFolder}/VerbFlexion.txt", verbFlexion);
            File.WriteAllLines($"{WiktioParserResultFolder}/OnlyVerbFlexion.txt", onlyVerbFlexion);



            var list = frequencyBuilder.GetFrequencyLists();
            builder.Clear();
            builder.AppendLine($"AllWordCount {frequencyBuilder.AllWordCount}");
            foreach (var wordFrequency in list)
            {
                builder.AppendLine($"{wordFrequency.Key.PadRight(15, ' ')} {wordFrequency.Count.ToString().PadRight(10, ' ')} zipf-{wordFrequency.ZipfFrequency.ToString().PadRight(5, ' ')} {wordFrequency.Frequency:N10}");
            }
            File.WriteAllText($"{WiktioParserResultFolder}/ValidWordsFrequency.txt", builder.ToString());
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
            //    //Debug.WriteLine($"saving {path}");

            //    try
            //    {
            //        File.WriteAllText(path, wikiPage.Text);
            //    }
            //    catch (Exception e)
            //    {
            //        Debug.WriteLine(path);
            //        // throw;
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

        private void cmdBuildWordFreqFromwikipedia_Click(object sender, RoutedEventArgs e)
        {

            Stopwatch stopwatch = new Stopwatch();
            // Avvia il timer
            stopwatch.Start();

            var frequencyBuilder = new WordFrequencyBuilder();
            var dico = CountWords(@"D:\zzzWiktionnaire\wikipedia\frwiki_pages", "*.txt");
            foreach (var words in dico)
            {
                frequencyBuilder.AddWord(words.Key, words.Value);
            }

            var list = frequencyBuilder.GetFrequencyLists();

            stopwatch.Stop();
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"AllWordCount {frequencyBuilder.AllWordCount}");
            foreach (var wordFrequency in list)
            {
                builder.AppendLine($"{wordFrequency.Key.PadRight(28, ' ')} {wordFrequency.Count.ToString().PadRight(10, ' ')} zipf-{wordFrequency.ZipfFrequency.ToString().PadRight(5, ' ')} {wordFrequency.Frequency:N10}");
            }
            File.WriteAllText($"{WiktioParserResultFolder}/zzzWikipediaFrequency.txt", builder.ToString());

            MessageBox.Show($"Parse Wiki pages for frequency in {stopwatch.Elapsed.TotalMinutes} minutes");

        }

        private ConcurrentDictionary<string, int> CountWords(string directory, string fileExtension)
        {

            //var text = wikiPage.Text;
            //var tokenList = text
            //    .Split(" .\n\r\t\b\0\\=?^+-*’–<>!|([{)}]#@,:;?\"'%&/".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
            //    .Where(w => w.Length >= 3);

            //foreach (var token in tokenList)
            //{
            //    if (SelectedFinalWords.ContainsKey(token))
            //    {
            //        frequencyBuilder.AddWord(token.ToLowerInvariant().RemoveDiacritics());
            //    }
            //}

            var bigFileDir = $@"{directory}\_bigFiles";
            var smallFileDir = $@"{directory}\_smallFiles";

            Directory.CreateDirectory(bigFileDir);
            Directory.CreateDirectory(smallFileDir);

            long processed = 0;

            var wordCounts = new ConcurrentDictionary<string, int>();

            // Get the list of files in the directory
            //string[] files = Directory.GetFiles(directory, fileExtension);

            DirectoryInfo directoryInfo = new DirectoryInfo(directory);
            FileInfo[] fileInfos = directoryInfo.GetFiles("*.*",SearchOption.TopDirectoryOnly);
            //FileInfo[] fileInfos = directoryInfo.GetFiles();

            // Process each file in parallel
            Parallel.ForEach(fileInfos, fileInfo =>
            {
                if (GetFileSizeInKilobytes(fileInfo)>50)
                {
                    MoveFileToDirectory(fileInfo.FullName,bigFileDir);
                    //ChangeFileExtension(fileInfo.FullName, "bigText");
                    return;
                }

                if (GetFileSizeInKilobytes(fileInfo) <= 1)
                {
                    MoveFileToDirectory(fileInfo.FullName, smallFileDir);
                    //ChangeFileExtension(fileInfo.FullName, "bigText");
                    return;
                }

                // Read the file into a string
                string text = File.ReadAllText(fileInfo.FullName).ToLowerInvariant().RemoveDiacritics();

                // Split the text into words
                var words = text
                    .Split(" .\n\r\t\b\0\\=?^+-*’–<>!|([{)}]#@,:;?\"'%&/".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                    .Where(w => w.Length >= 3);

                // string[] words = text.Split(new char[] { ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                // Count the occurrence of each word and add it to the dictionary
                foreach (string word in words)
                {
                    if (officiaScrabbleWordList.ContainsKey(word))
                    {
                        wordCounts.AddOrUpdate(word.ToLower(), 1, (key, oldValue) => oldValue + 1);
                    }
                }

                ChangeFileExtension(fileInfo.FullName,"old");

                Interlocked.Increment(ref processed);
                if (processed% 50000==0)
                {
                    var currentCount = processed;
                    Console.WriteLine($"processed {processed} files");
                    Debug.WriteLine($"processed {processed} files");
                    var builder=new StringBuilder();
                    foreach (var pair in wordCounts)
                    {
                        builder.AppendLine($"{pair.Key} {pair.Value}");
                    }
                    File.WriteAllText($"{WiktioParserResultFolder}/zzzWikipediaFrequency{currentCount}.txt", builder.ToString());
                }
            });
            return wordCounts;

        }


        public static void MoveFileToDirectory(string filePath, string directoryPath)
        {
            string fileName = Path.GetFileName(filePath);
            string newFilePath = Path.Combine(directoryPath, fileName);

            File.Move(filePath, newFilePath);
        }

        public static long GetFileSizeInKilobytes(FileInfo fileInfo)
        {
            return (fileInfo.Length ) / 1024;
        }
        public static void ChangeFileExtension(string filePath, string newExtension)
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            string newFileName = fileName + "." + newExtension;
            string newFilePath = Path.Combine(Path.GetDirectoryName(filePath), newFileName);

            File.Move(filePath, newFilePath);
        }

        public static void RenameFileWithExtensionSuffix(string filePath, string suffix)
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            string fileExtension = Path.GetExtension(filePath);

            string newFileName = fileName + suffix + fileExtension;
            string newFilePath = Path.Combine(Path.GetDirectoryName(filePath), newFileName);

            File.Move(filePath, newFilePath);
        }
    }
}
