using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using CommonLibTools.Libs;
using MongoDB.Driver;
using WiktionaireParser.Models;
using Path = System.IO.Path;

namespace WiktionaireParser.UiControls
{
    /// <summary>
    /// Interaction logic for WordFrequencyParser.xaml
    /// </summary>
    public partial class WordFrequencyParser : UserControl
    {
        public static string dataFolder = @"D:\__programs_datas\";
        private string corpusName = @"G:\zzzWiktionnaire\__corpus\frwiki-20181001-corpus.xml";
        private string lang = "fr";
        private int minLen = 3;
        private int maxLen = 40;
        private long pageToParseCount = long.MaxValue;
        WordFrequencyBuilder frequencyBuilder = new WordFrequencyBuilder();
        private IMongoDatabase Database;
        public static IMongoCollection<WordFrequency> WordFrequencyCollection;
        public WordFrequencyParser()
        {
            InitializeComponent();
        }

        private void cmdParse_Click(object sender, RoutedEventArgs e)
        {
            SplitDicoToPage();
            cmdParse.IsEnabled = false;
        }
        public void SplitDicoToPage()
        {
            Task.Run(() =>
            {
                Database = MainWindow.Database;

                long pageCount = 0;
                //count = int.MaxValue;
                var builder = new StringBuilder();
                using (StreamReader sr = File.OpenText(corpusName))
                {
                    string line = String.Empty;
                    while ((line = sr.ReadLine()) != null && pageCount <= pageToParseCount)
                    {
                        if (line.Trim().StartsWith("<article"))
                        {
                            builder.Clear();
                            builder.AppendLine(line);


                            do
                            {
                                line = sr.ReadLine();
                                builder.AppendLine(line);
                            } while (line != null && line.Trim().StartsWith("</article>") == false);

                            var page = builder.ToString();
                            pageCount++;

                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                /* Your code here */
                                cmdParse.Content = pageCount.ToString();

                            });
                            //Console.WriteLine(page);

                            XmlDocument document = new XmlDocument();
                            document.LoadXml(page);

                            var text = "";
                            try
                            {

                                var nodeList = document.GetElementsByTagName("content");
                                text = nodeList[0].InnerText.Trim();
                                // txtResult.Text = text;

                                // word frequency calculation
                                var tokenList = text
                                    .Split(" .\n\r\t\b\0\\=´?^+-*’–<>!|([{)}]#@,:;?\"'%&/".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                                    .Where(w => w.Length >= minLen && w.Length <= maxLen);

                                foreach (var token in tokenList)
                                {
                                    if (char.IsLetter(token[0]))
                                    {
                                        frequencyBuilder.AddWord(token.ToLowerInvariant().RemoveDiacritics());
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

                var list = frequencyBuilder.GetFrequencyLists();
                var freqBuilder = new StringBuilder();
                freqBuilder.AppendLine($"totalCount {frequencyBuilder.AllWordCount}");
                foreach (var wordFrequency in list)
                {
                    freqBuilder.AppendLine($"{wordFrequency.Key} {wordFrequency.Count}");
                }

                var resultFile = $"{dataFolder}word_freq_{lang}_{minLen}_{maxLen}.txt";
                File.WriteAllText(resultFile, freqBuilder.ToString());

                Application.Current.Dispatcher.Invoke(() =>
                {
                    /* Your code here */
                    lbxFreqList.ItemsSource = list;

                });
                WordFrequencyCollection = Database.GetCollection<WordFrequency>($"word_freq_{lang}_{minLen}_{maxLen}");
                WordFrequencyCollection.InsertManyAsync(list);


            });

        }


        public void ParseWikiDump()
        {
            //database.DropCollection(wikiCollectionName);
            //wikiCollection = database.GetCollection<WikiPage>(wikiCollectionName);

            //anagramBuilder = new AnagramBuilder();
            //SplitDicoToPage();
            ////remove non valid word in frequency builder

            //frequencyBuilder.CheckAllWord(correctWikiPageWords);

            //SaveToDB(PagesList);
            //MessageBox.Show("Parse Wiki Dump");
            //LoadPagesFromDb();
        }

        private void cmdMergeto_wiktio_freq_Click(object sender, RoutedEventArgs e)
        {
            var fileName = @"D:\__programs_datas\word_freq_fr_3_40.txt";

            //var dirPath = Path.GetFullPath(fileName);
            var dirPath = Path.GetDirectoryName(fileName);
            var name = Path.GetFileNameWithoutExtension(fileName);
            var newName = $"{dirPath}\\{name}_final.txt";
            // Fix(fileName);return;

            var list = MainWindow.WikiCollection.Find(FilterDefinition<WikiPage>.Empty)
                .ToList();
            // var set = new HashSet<string>(enumerable);
            var valids = new Dictionary<string, long>();
            long totalCount = 0;

            foreach (var page in list)
            {
                if (valids.ContainsKey(page.TitleInv) == false)
                {
                    valids.Add(page.TitleInv, page.FrequencyCount);
                }
                else
                {
                    valids[page.TitleInv] += page.FrequencyCount;
                }

                totalCount += page.FrequencyCount;
            }

            var lines = File.ReadAllLines(fileName);

            long count;

            foreach (var line in lines)
            {
                var tokens = line.Split();
                var mot = tokens.First();
                count = Convert.ToInt32(tokens.Last());

                if (valids.ContainsKey(mot))
                {
                    valids[mot] += count;
                    totalCount += count;
                }
            }

            var final = new StringBuilder();
            final.AppendLine($"totalCount {totalCount}");

            foreach (var word in valids.OrderByDescending(w => w.Value))
            {
                //count += valids[word.Key];
                final.AppendLine($"{word.Key} {word.Value}");
            }

            final.AppendLine();
            File.WriteAllText(newName, final.ToString());


            //build final game dico with frequency
            final.Clear();
            fileName = @"D:\__programs_datas\ods8_final_no_verbs_4_to_7.txt";

            dirPath = Path.GetDirectoryName(fileName);
            name = Path.GetFileNameWithoutExtension(fileName);
            newName = $"{dirPath}\\{name}_with_freq.txt";

            lines = File.ReadAllLines(fileName);
            var dico = new Dictionary<string, double>();
            foreach (var line in lines)
            {
                if (valids.ContainsKey(line))
                {
                    var zipfScale = LetterFrequency.GetZipfFrequency(valids[line], totalCount);
                    dico.Add(line, zipfScale);
                }
            }

            foreach (var line in dico.OrderByDescending(d=>d.Value))
            {
                final.AppendLine($"{line.Key} {line.Value:N2}");
            }


            File.WriteAllText(newName, final.ToString());
            MessageBox.Show("done");
        }
    }
}
