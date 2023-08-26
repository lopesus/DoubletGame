using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Threading;
using MongoDB.Driver;
using WiktionaireParser.Models;

namespace MultiStreamExtractor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Dictionary<string, bool> officiaScrabbleWordList = new Dictionary<string, bool>();
        private readonly string officialScrabbleDico = @"D:\zzzWiktionnaire\DICO\ODS8.txt";

        readonly string pagesArticlesMultistreamXmlBz2 = @"D:\zzzWiktionnaire\wikipedia\frwiki-20230320-pages-articles-multistream.xml.bz2";
        readonly string pagesArticlesMultistreamIndexTxt = @"D:\zzzWiktionnaire\wikipedia\frwiki-20230320-pages-articles-multistream-index.txt";
        private readonly MultiStreamInfos wiktionary;
        private Stopwatch _stopwatch;

        public static int MinLen = 3;
        public static int MaxLen = 7;
        public MainWindow()
        {
            InitializeComponent();

            wiktionary = Wiktionaries.GetWiktionary(WikiLang.French);
            AppResources.Instance.Initialize(wiktionary , MinLen, MaxLen);
            SetDbIndex();

            WeakReferenceMessenger.Default.Register<UpdateUIMessage>(this, (recipient, message) =>
            {
                Dispatcher.InvokeAsync(() =>
                {
                    var elapsed = 0d;
                    if (_stopwatch != null)
                    {
                        elapsed = _stopwatch.Elapsed.TotalMinutes;
                    }
                    tblProgress.Text = $"processing {message.CurrentChunk}/{message.TotalChunk} in {elapsed} minutes";

                }, DispatcherPriority.Send);
            });


            officiaScrabbleWordList = File.ReadAllLines(officialScrabbleDico).ToList().ToDictionary(s => s.ToLowerInvariant(), s => true);

            pagesArticlesMultistreamXmlBz2 = wiktionary.ArticlesPath;
            pagesArticlesMultistreamIndexTxt = wiktionary.IndexPath;

            //var readerSingle = new WikipediaReaderSingle(
            //    @"D:\zzzWiktionnaire\wikipedia\frwiki-20230320-pages-articles-multistream.xml.bz2",
            //    @"D:\zzzWiktionnaire\wikipedia\frwiki-20230320-pages-articles-multistream-index.txt"
            //);

            //var articleContent = readerSingle.ExtractArticleContent("Mangue");
            //txtPage.Text = articleContent;
            //Console.WriteLine(articleContent);



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

            await AppResources.Instance.WikiPageCollection.Indexes.CreateOneAsync(indexModel);

            var indexDefinitionAnagram = Builders<Anagram>.IndexKeys.Combine(
                Builders<Anagram>.IndexKeys.Ascending(f => f.Key),
                Builders<Anagram>.IndexKeys.Ascending(f => f.Count)
            );

            //CreateIndexModel<Anagram> indexModelAnagram = new CreateIndexModel<Anagram>(indexDefinitionAnagram);
            //await anagramCollection.Indexes.CreateOneAsync(indexModelAnagram);
        }

        private async void cmdExtract_Click(object sender, RoutedEventArgs e)
        {
            var processAllChunk = true;
            var artciclePerchunk = Int32.MaxValue;// 1000;
            var savePage = false;
            var saveToDb = true;
            var processorCount = Environment.ProcessorCount - 2;
            var checkInValidWordList = true;

            var reader = new WikipediaReader(
                wiktionary,
                pagesArticlesMultistreamXmlBz2,
                pagesArticlesMultistreamIndexTxt,
                 officiaScrabbleWordList,
                processAllChunk,
                processorCount,
                artciclePerchunk,
                savePage,
                checkInValidWordList,
                saveToDb
            );

            _stopwatch = Stopwatch.StartNew();
            var result = await reader.ExtractAndProcessArticles();
            WikiPageDataExtractor.Extract(reader.PagesList);

            _stopwatch.Stop();
            var elapsed = _stopwatch.Elapsed.TotalMinutes;

            MessageBox.Show($"DONE IN {elapsed} min");
        }

        //private void LoadTrie()
        //{
        //    var lines = File.ReadAllLines(DicoTrieName);

        //    DawgService = new DawgService(lines);
        //    Trie = DawgService.GetTrie();
        //}
    }
}