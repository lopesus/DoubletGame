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

            PagesList = collection.Find(FilterDefinition<WikiPage>.Empty).Skip(10000).Limit(10000)
                .Sort(Builders<WikiPage>.Sort.Ascending(p=>p.Title))
                .ToList();

            lbxPages.ItemsSource = PagesList;

        }

        public void ParseWikiDump()
        {
            database.DropCollection(collectionName);
            collection = database.GetCollection<WikiPage>(collectionName);

            SplitDicoToPage();
            SaveToDB(PagesList);
            MessageBox.Show("Parse Wiki Dump");
        }
        private async void SetDbIndex()
        {

            var indexDefinition = Builders<WikiPage>.IndexKeys.Combine(
                Builders<WikiPage>.IndexKeys.Ascending(f => f.Title),
                Builders<WikiPage>.IndexKeys.Ascending(f => f.Len),
                Builders<WikiPage>.IndexKeys.Ascending(f => f.IsVerb));

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

                            if (char.IsLetter(title[0]))
                            {
                                nodeList = document.GetElementsByTagName("text");
                                text = nodeList[0].InnerText.Trim();

                                if (text.StartsWith("{{voir") || text.StartsWith("== {{langue|fr}} =="))
                                {
                                    var wikiPage = new WikiPage(title, text);
                                    PagesList.Add(wikiPage);
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
            if (page != null) txtPages.Text = page.Text;
        }

        private void cmdParseDump_Click(object sender, RoutedEventArgs e)
        {
            ParseWikiDump();
        }

        private async void cmdTrouverMot_Click(object sender, RoutedEventArgs e)
        {
            var mot = txtMot.Text.Trim();
            if (true)
            {

                var results = await collection.Find(x => x.Title == mot).Limit(1).ToListAsync();
                var  page = results.FirstOrDefault();
                txtPages.Text = page.Text;

                //var filter= Builders<WikiPage>.Filter.Eq(p => p.Title, mot);
            }
        }
    }
}
