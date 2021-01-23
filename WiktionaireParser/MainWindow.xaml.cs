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
        private IMongoCollection<WikiPage> collection;
        private int Take = Int32.MaxValue;
        private string collectionName;
        int pageToLoadFromDb = Int32.MaxValue;
        int pageToSkipFromDb = 0;

        public MainWindow()
        {
            // To directly connect to a single MongoDB server
            // (this will not auto-discover the primary even if it's a member of a replica set)
            client = new MongoClient();
            database = client.GetDatabase("Wiktionaire");
            collectionName = "fr";
            collection = database.GetCollection<WikiPage>(collectionName);

            SetDbIndex();


            InitializeComponent();

            LoadPagesFromDb();
        }

        private void LoadPagesFromDb()
        {
            PagesList = collection.Find(FilterDefinition<WikiPage>.Empty).Skip(pageToSkipFromDb).Limit(pageToLoadFromDb)
                .Sort(Builders<WikiPage>.Sort.Ascending(p => p.Title))
                .ToList();

            lbxPages.ItemsSource = PagesList;
        }
        private void cmdSerachFilter_Click(object sender, RoutedEventArgs e)
        {
            bool isVerb = chkVerb.IsChecked ?? false;
            bool hasAntonym = chkAnto.IsChecked ?? false;
            bool hasSinonym = chkSino.IsChecked ?? false;


            FilterDefinition<WikiPage> verbFilter;
            verbFilter = isVerb ? Builders<WikiPage>.Filter.Eq(p => p.IsVerb, true) : FilterDefinition<WikiPage>.Empty;
            var antonymFilter = hasAntonym ? Builders<WikiPage>.Filter.Eq(p => p.HasAntonymes, true) : FilterDefinition<WikiPage>.Empty;
            var sinonymFilter = hasSinonym ? Builders<WikiPage>.Filter.Eq(p => p.HasSinonymes, true) : FilterDefinition<WikiPage>.Empty;



            PagesList = collection.Find(antonymFilter & sinonymFilter & verbFilter).Skip(pageToSkipFromDb).Limit(pageToLoadFromDb)
                .Sort(Builders<WikiPage>.Sort.Ascending(p => p.Title))
                .ToList();

            lbxPages.ItemsSource = PagesList;
            tblCount.Text = $"found {PagesList.Count}";
        }
        public void ParseWikiDump()
        {
            database.DropCollection(collectionName);
            collection = database.GetCollection<WikiPage>(collectionName);

            SplitDicoToPage();
            SaveToDB(PagesList);
            MessageBox.Show("Parse Wiki Dump");
            LoadPagesFromDb();
        }
        private async void SetDbIndex()
        {

            var indexDefinition = Builders<WikiPage>.IndexKeys.Combine(
                Builders<WikiPage>.IndexKeys.Ascending(f => f.Title),
                Builders<WikiPage>.IndexKeys.Ascending(f => f.Len),
                Builders<WikiPage>.IndexKeys.Ascending(f => f.IsVerb),
                Builders<WikiPage>.IndexKeys.Ascending(f => f.HasAntonymes),
                Builders<WikiPage>.IndexKeys.Ascending(f => f.HasSinonymes)
                );

            CreateIndexModel<WikiPage> indexModel = new CreateIndexModel<WikiPage>(indexDefinition);

            await collection.Indexes.CreateOneAsync(indexModel);

        }

        public void SplitDicoToPage()
        {
            int count = 0;
            //count = int.MaxValue;
            var builder = new StringBuilder();
            using (StreamReader sr = File.OpenText(fileName))
            {
                string line = String.Empty;
                while ((line = sr.ReadLine()) != null && count <= Take)
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

                            if (title == "infarcir")
                            {
                                Console.WriteLine();
                            }
                            var firstChar = title[0];
                            if (char.IsLetter(firstChar)
                                && char.IsUpper(firstChar) == false
                                && title.Contains("-") == false && title.Contains(" ") == false
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
            await collection.InsertManyAsync(list);
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
                try
                {
                    var page = await collection.Find(x => x.Title == mot).FirstAsync();//.Limit(1).ToListAsync();
                    txtAllPageText.Text = page.Text;
                    txtPageLangText.Text = page.LangText;

                    txtAntonymes.Text = page.Antonymes;
                    txtSynonymes.Text = page.Sinonymes;
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    txtPageLangText.Text = exception.Message;
                }


                //var filter= Builders<WikiPage>.Filter.Eq(p => p.Title, mot);
            }
        }


    }
}
