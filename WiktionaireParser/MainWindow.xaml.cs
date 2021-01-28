using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using CommonLibTools;
using CommonLibTools.DataStructure.Dawg;
using CommonLibTools.DataStructure.Dawg.Construction;
using MongoDB.Driver;
using WiktionaireParser.Models;

namespace WiktionaireParser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string fileName = @"G:\zzzWiktionnaire\frwiktionary-latest-pages-articles.xml";
        private string folder = @"G:\zzzWiktionnaire\";
        public List<WikiPage> PagesList = new List<WikiPage>();
        private MongoClient client;
        private IMongoDatabase database;
        private IMongoCollection<WikiPage> wikiCollection;
        private IMongoCollection<Anagram> anagramCollection;
        private string wikiCollectionName;

        int pageToLoadFromDb = Int32.MaxValue;
        int pageToSkipFromDb = 0;
        private int pageToParseCount = Int32.MaxValue;

        private AnagramBuilder anagramBuilder;
        WordFrequencyBuilder frequencyBuilder = new WordFrequencyBuilder();

        Dictionary<string, bool> correctWikiPageWords = new Dictionary<string, bool>();

        ConstructTrie constructTrie = new ConstructTrie();
        private Trie Trie;
        private DawgService DawgService;
        public MainWindow()
        {

            //constructTrie.GenerateTrieFromFile(@"C:\Users\mboum\Desktop\web\verbes\ods8_final.txt");

            // To directly connect to a single MongoDB server
            // (this will not auto-discover the primary even if it's a member of a replica set)
            client = new MongoClient();
            database = client.GetDatabase("wiki");
            wikiCollectionName = "fr";
            wikiCollection = database.GetCollection<WikiPage>(wikiCollectionName);
            anagramCollection = database.GetCollection<Anagram>("anagram");

            SetDbIndex();

            anagramBuilder = new AnagramBuilder();

            InitializeComponent();

            var len = new List<int>();

            cbxLength.ItemsSource = Enumerable.Range(0, 10);//.Select(x => x * x);
            cbxLength.SelectedIndex = 5;


            cbxAnagramCount.ItemsSource = Enumerable.Range(0, 20);//.Select(x => x * x);
            cbxAnagramCount.SelectedIndex = 0;

            LoadPagesFromDb();
        }

        private void LoadPagesFromDb()
        {
            PagesList = wikiCollection.Find(FilterDefinition<WikiPage>.Empty).Skip(pageToSkipFromDb).Limit(pageToLoadFromDb)
                .Sort(Builders<WikiPage>.Sort.Ascending(p => p.Title))
                .ToList();

            lbxPages.ItemsSource = PagesList;
            var anagrams = anagramCollection.Find(FilterDefinition<Anagram>.Empty).ToList();
            anagramBuilder = new AnagramBuilder(anagrams);

            var lines = File.ReadAllLines(@"D:\__programs_datas\wiki_valid_word_trie.txt");

            DawgService = new DawgService(lines);
            Trie = DawgService.GetTrie();
        }
        private void cmdSerachFilter_Click(object sender, RoutedEventArgs e)
        {
            bool isVerb = chkVerb.IsChecked ?? false;
            bool hasAntonym = chkAnto.IsChecked ?? false;
            bool hasSinonym = chkSino.IsChecked ?? false;
            bool hasAnagram = chkAnagram.IsChecked ?? false;
            bool sortByFrequency = chkFrequency.IsChecked ?? false;


            int anagramCount = cbxAnagramCount.SelectedIndex;
            int len = cbxLength.SelectedIndex;


            FilterDefinition<WikiPage> verbFilter;
            verbFilter = isVerb ? Builders<WikiPage>.Filter.Eq(p => p.IsVerb, true) : FilterDefinition<WikiPage>.Empty;
            var antonymFilter = hasAntonym ? Builders<WikiPage>.Filter.Eq(p => p.HasAntonymes, true) : FilterDefinition<WikiPage>.Empty;
            var sinonymFilter = hasSinonym ? Builders<WikiPage>.Filter.Eq(p => p.HasSinonymes, true) : FilterDefinition<WikiPage>.Empty;
            var anagramFilter = hasAnagram ? Builders<WikiPage>.Filter.Gt(p => p.AnagramCount, anagramCount) : FilterDefinition<WikiPage>.Empty;
            var frequencyFilter = hasAnagram ? Builders<WikiPage>.Filter.Gt(p => p.AnagramCount, anagramCount) : FilterDefinition<WikiPage>.Empty;


            var lengthFilter = len > 0 ? Builders<WikiPage>.Filter.Eq(p => p.Len, len) : FilterDefinition<WikiPage>.Empty;


            var sortByTitle = Builders<WikiPage>.Sort.Ascending(p => p.Title);
            var sortByFrequencyDef = Builders<WikiPage>.Sort.Descending(p => p.Frequency);
            var findFluent = wikiCollection.Find(antonymFilter & sinonymFilter & verbFilter & lengthFilter & anagramFilter)
                .Skip(pageToSkipFromDb).Limit(pageToLoadFromDb);
            if (sortByFrequency)
            {
                findFluent = findFluent.Sort(sortByFrequencyDef);
                //findFluent=  findFluent.Sort(sortByTitle);
            }
            else
            {
                findFluent = findFluent.Sort(sortByTitle);
            }
            PagesList = findFluent
                //.Sort(sortDefinition)
                .ToList();

            lbxPages.ItemsSource = PagesList;
            tblCount.Text = $"found {PagesList.Count}";
        }
        public void ParseWikiDump()
        {
            database.DropCollection(wikiCollectionName);
            wikiCollection = database.GetCollection<WikiPage>(wikiCollectionName);

            anagramBuilder = new AnagramBuilder();
            SplitDicoToPage();
            //remove non valid word in frequency builder

            frequencyBuilder.CheckAllWord(correctWikiPageWords);

            SaveToDB(PagesList);
            MessageBox.Show("Parse Wiki Dump");
            LoadPagesFromDb();
        }
        private async void SetDbIndex()
        {

            var indexDefinition = Builders<WikiPage>.IndexKeys.Combine(
                Builders<WikiPage>.IndexKeys.Ascending(f => f.Title),
                Builders<WikiPage>.IndexKeys.Ascending(f => f.TitleInv),
                Builders<WikiPage>.IndexKeys.Ascending(f => f.AnagramKey),
                Builders<WikiPage>.IndexKeys.Ascending(f => f.AnagramCount),
                Builders<WikiPage>.IndexKeys.Ascending(f => f.Len),
                Builders<WikiPage>.IndexKeys.Ascending(f => f.IsVerb),
                Builders<WikiPage>.IndexKeys.Ascending(f => f.HasAntonymes),
                Builders<WikiPage>.IndexKeys.Ascending(f => f.HasSinonymes),
                //frequency
                Builders<WikiPage>.IndexKeys.Ascending(f => f.Frequency),
                Builders<WikiPage>.IndexKeys.Ascending(f => f.FrequencyCount)

                );

            CreateIndexModel<WikiPage> indexModel = new CreateIndexModel<WikiPage>(indexDefinition);

            await wikiCollection.Indexes.CreateOneAsync(indexModel);

            var indexDefinitionAnagram = Builders<Anagram>.IndexKeys.Combine(
                           Builders<Anagram>.IndexKeys.Ascending(f => f.Key),
                           Builders<Anagram>.IndexKeys.Ascending(f => f.Count)
                           );

            //CreateIndexModel<Anagram> indexModelAnagram = new CreateIndexModel<Anagram>(indexDefinitionAnagram);
            //await anagramCollection.Indexes.CreateOneAsync(indexModelAnagram);



        }

        public void SplitDicoToPage()
        {
            int count = 0;
            //count = int.MaxValue;
            var builder = new StringBuilder();
            using (StreamReader sr = File.OpenText(fileName))
            {
                string line = String.Empty;
                while ((line = sr.ReadLine()) != null && count <= pageToParseCount)
                {
                    count++;
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
                            if (char.IsLetter(firstChar)
                                && char.IsUpper(firstChar) == false
                                && title.Contains("-") == false && title.Contains(" ") == false
                                && title.Contains("’") == false && title.Contains("'") == false
                                && title.Contains(".") == false && title.Contains("/") == false
                                && title.Contains("(") == false && title.Contains(")") == false
                                && title.Contains("[") == false && title.Contains("]") == false
                                && title.Contains(",") == false && title.Contains("*") == false
                                )
                            {
                                nodeList = document.GetElementsByTagName("text");
                                text = nodeList[0].InnerText.Trim();

                                if (text.StartsWith("{{voir") || RegexLib.StartWithLangSectionRegex.IsMatch(text))
                                {
                                    if (RegexLib.ContainsLangSectionRegex.IsMatch(text))
                                    {
                                        var wikiPage = new WikiPage(title, text);
                                        if (wikiPage.IsOnlyVerbFlexion() == false)
                                        {
                                            PagesList.Add(wikiPage);
                                            correctWikiPageWords[wikiPage.TitleInv] = true;

                                            // anagram calculation
                                            var anagram = title.ToLowerInvariant().RemoveDiacritics();
                                            var anagramKey = anagram.SortString();
                                            anagramBuilder.Add(anagramKey, anagram);

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
            await wikiCollection.InsertManyAsync(list);
            await anagramCollection.InsertManyAsync(anagramBuilder.GetAnagramsList());

            File.WriteAllText(@"D:\__programs_datas\wiki_valid_word.txt", builder.ToString());
        }

        private void lbxPages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var page = lbxPages.SelectedItem as WikiPage;
            if (page != null)
            {
                txtPageLangText.Text = page.LangText;
                txtAllPageText.Text = page.Text;
                txtAntonymes.Text = page.Antonymes;
                txtSynonymes.Text = page.Sinonymes;
                tblWordInfos.Text = $"freq:{page.Frequency} {page.FrequencyCount} on {page.FrequencyTotalCount} max is {page.MostFrequentWordCount}";


                GetAnagramFor(page.AnagramKey);
                var validWord = GetAllValidWordFor(page.AnagramKey);
                txtAllPossibleWord.Text = validWord.ToString();
            }
        }

        private void cmdParseDump_Click(object sender, RoutedEventArgs e)
        {
            ParseWikiDump();
        }

        private async void cmdTrouverMot_Click(object sender, RoutedEventArgs e)
        {
            var mot = txtMot.Text.Trim();
            if (mot.IsEmptyString() == false)
            {
                mot = mot.RemoveDiacritics();
                try
                {
                    var page = await wikiCollection.Find(x => x.TitleInv == mot).FirstAsync();//.Limit(1).ToListAsync();
                    tblWordInfos.Text = $"freq:{page.Frequency} {page.FrequencyCount} on {page.FrequencyTotalCount} max is {page.MostFrequentWordCount}";
                    txtAllPageText.Text = page.Text;
                    txtPageLangText.Text = page.LangText;

                    txtAntonymes.Text = page.Antonymes;
                    txtSynonymes.Text = page.Sinonymes;
                    GetAnagramFor(page.AnagramKey);

                    var validWord = GetAllValidWordFor(page.AnagramKey);
                    txtAllPossibleWord.Text = validWord.ToString();
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    txtPageLangText.Text = exception.Message;
                }


                //var filter= Builders<WikiPage>.Filter.Eq(p => p.Title, mot);
            }
        }


        private async void GetAnagramFor(string key)
        {
            try
            {
                var page = anagramBuilder.GetAnagramFor(key);
                //var page = await anagramCollection.Find(x => x.Key == key).FirstAsync();//.Limit(1).ToListAsync();
                var all = string.Join("\r\n", page.AnagramList);
                txtAnagrams.Text = all;

            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                txtAnagrams.Text = exception.Message;
            }


            //var filter= Builders<WikiPage>.Filter.Eq(p => p.Title, mot);
        }
        private ValidWord GetAllValidWordFor(string key)
        {
            try
            {
                var result = DawgService.FindAllPossibleWord(key);
                var validWord = new ValidWord(result);

                return validWord;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                txtAnagrams.Text = exception.Message;
                return null;
            }
        }


    }
}
