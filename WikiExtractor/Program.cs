using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Xml;
using SharpCompress.Compressors.BZip2;

using MwParserFromScratch;
using MwParserFromScratch.Nodes;
namespace WikiExtractor
{
    class Program
    {
        public static void Main()
        {
            var reader = new WikipediaReader(
                @"D:\zzzWiktionnaire\wikipedia\frwiki-20230320-pages-articles-multistream.xml.bz2",
                @"D:\zzzWiktionnaire\wikipedia\frwiki-20230320-pages-articles-multistream-index.txt"
            );

            var articleContent = reader.ExtractArticleContent("Mangue");
            Console.WriteLine(articleContent);
        }
        public static void Main00()
        {
            var reader = new WikipediaReader1(
                @"D:\zzzWiktionnaire\wikipedia\frwiki-20230320-pages-articles-multistream.xml.bz2",
                @"D:\zzzWiktionnaire\wikipedia\frwiki-20230320-pages-articles-multistream-index.txt"
            );

            var articleContent = reader.GetArticle("Mangue");
            Console.WriteLine(articleContent);
        }

        static void Main1(string[] args)
        {
            var articleDumpPath = @"D:\zzzWiktionnaire\wikipedia\frwiki-20230320-pages-articles-multistream.xml.bz2";
            var indexPath = @"D:\zzzWiktionnaire\wikipedia\frwiki-20230320-pages-articles-multistream-index.txt";
            var indexPathBZip = @"D:\zzzWiktionnaire\wikipedia\frwiki-20230320-pages-articles-multistream-index.txt.bz2";

            var resultFolder = Path.GetFileNameWithoutExtension(articleDumpPath).Split('-').FirstOrDefault();
            var wikipediaPagesFolder = Path.Combine(Path.GetDirectoryName(articleDumpPath), $"{resultFolder}_pages");
            Directory.CreateDirectory(wikipediaPagesFolder);

            // Article title to search for
            string title = "Mangue";

            long maxCount = Int64.MaxValue;
            long count = 0;


            // Open the dump file
            using (var dumpStream = File.OpenRead(articleDumpPath))
            using (var dumpReader = new BZip2Stream(dumpStream, SharpCompress.Compressors.CompressionMode.Decompress, true))
            {
                // Read the dump file
                using (var xmlReader = XmlReader.Create(dumpReader))
                {
                    while (xmlReader.Read() && count<maxCount)
                    {
                        // Check if the current element is a page element
                        if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "page")
                        {
                            // Read the entire page element into a StringBuilder
                            var articleText = new StringBuilder();
                            xmlReader.ReadStartElement();
                            while (xmlReader.NodeType != XmlNodeType.EndElement || xmlReader.Name != "page")
                            {
                                articleText.Append(xmlReader.ReadOuterXml());
                            }
                            articleText.Append(xmlReader.ReadOuterXml()); // Read the </page> element

                            // Extract the article title and text
                            var articleString = articleText.ToString(); // Convert StringBuilder to string
                            XmlDocument document = new XmlDocument();
                            document.LoadXml($"<page>{articleString}</page>");
                            //document.LoadXml(articleString);

                            //var parser = new WikitextParser();
                            //var plainText = parser.Parse(articleString).ToPlainText();
                            //var ast2 = parser.Parse(articleString).ToString();
                            // var plainText = WikiMarkup.Parse(articleString).ToPlainText();

                            var nodeList = document.GetElementsByTagName("title");
                            title = nodeList[0].InnerText.Trim();

                            nodeList = document.GetElementsByTagName("text");
                            var text = nodeList[0].InnerText.Trim();

                            //var plainText2 = parser.Parse(text).ToPlainText();
                            var articleTitle = new string(title.Where(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c)).ToArray());

                            // Save the parsed article to a file
                            var articlePath = Path.Combine(wikipediaPagesFolder, $"{articleTitle}.txt");
                            try
                            {
                                File.WriteAllText(articlePath, text);
                            }
                            catch (Exception)
                            {
                            }
                            count++;
                            var divRem = Math.DivRem(count,10000,out var res);
                            if (count % 10000 ==0 )
                            {
                                Console.WriteLine($"extracted {count} articles");
                                Debug.WriteLine($"extracted {count} articles");
                            }
                        }
                    }
                }
            }

            Console.WriteLine("Done press enter");
            Console.ReadLine();
        }

        static void Main2(string[] args)
        {
            var articleDumpPath = @"D:\zzzWiktionnaire\wikipedia\frwiki-20230320-pages-articles-multistream.xml.bz2";
            var indexPath = @"D:\zzzWiktionnaire\wikipedia\frwiki-20230320-pages-articles-multistream-index.txt";
            var indexPathBZip = @"D:\zzzWiktionnaire\wikipedia\frwiki-20230320-pages-articles-multistream-index.txt.bz2";

            var resultFolder = Path.GetFileNameWithoutExtension(articleDumpPath).Split('-').FirstOrDefault();
            var wikipediaPagesFolder = Path.Combine(Path.GetDirectoryName(articleDumpPath), $"{resultFolder}_pages");
            Directory.CreateDirectory(wikipediaPagesFolder);

            // Article title to search for
            string title = "Mangue";

            long maxCount = 100;
            long count = 0;


            // Open the dump file
            using (var dumpStream = File.OpenRead(articleDumpPath))
            using (var decompressedStream = new BZip2Stream(dumpStream, SharpCompress.Compressors.CompressionMode.Decompress, true))
            {
                // Seek to the byte offset
                // decompressedStream.Seek(offset, SeekOrigin.Begin);

                // Read the article text
                var articleText = new StringBuilder();
                using (var xmlReader = XmlReader.Create(decompressedStream))
                {
                    while (xmlReader.Read() &&  count<maxCount)
                    {
                        if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "page")
                        {
                            // Read the entire <page> element into the StringBuilder
                            xmlReader.ReadStartElement();
                            while (xmlReader.NodeType != XmlNodeType.EndElement || xmlReader.Name != "page")
                            {
                                articleText.Append(xmlReader.ReadOuterXml());
                            }
                            articleText.Append(xmlReader.ReadOuterXml()); // Read the </page> element

                            //var text = articleText.ToString();
                            //Console.WriteLine(text);

                            // Extract the article title and text
                            var articleString = articleText.ToString(); // Convert StringBuilder to string
                            XmlDocument document = new XmlDocument();
                            document.LoadXml($"<page>{articleString}</page>");

                            var nodeList = document.GetElementsByTagName("title");
                            title = nodeList[0].InnerText.Trim();

                            nodeList = document.GetElementsByTagName("text");
                            var text = nodeList[0].InnerText.Trim();


                            var articleTitle = new string(title.Where(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c)).ToArray());

                           //// Create a directory to store the articles
                           // var articlesDirectory = Path.Combine(AppContext.BaseDirectory, "articles");
                           // Directory.CreateDirectory(articlesDirectory);

                            // Save the parsed article to a file
                            var articlePath = Path.Combine(wikipediaPagesFolder, $"{articleTitle}.txt");
                            File.WriteAllText(articlePath, text);
                            count++;

                            //break; // Stop reading the dump file once we've found the article
                        }
                    }
                }

               
            }


            return;

            // Open the index file
            using (var indexStream = File.OpenRead(indexPath))
            using (var indexReader = new StreamReader(indexStream))
            {
                string line;
                while ((line = indexReader.ReadLine()) != null)
                {
                    // Split the line into title and byte offset
                    var splitLine = line.Split(':');
                    if (splitLine[2].Trim() != title)
                        continue;
                    var offset = long.Parse(splitLine[0]);

                    // Open the dump file
                    using (var dumpStream = File.OpenRead(articleDumpPath))
                    using (var decompressedStream = new BZip2Stream(dumpStream, SharpCompress.Compressors.CompressionMode.Decompress,true))
                    {
                        // Seek to the byte offset
                       // decompressedStream.Seek(offset, SeekOrigin.Begin);

                        // Read the article text
                        var articleText = new StringBuilder();
                        using (var xmlReader = XmlReader.Create(decompressedStream))
                        {
                            while (xmlReader.Read())
                            {
                                if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "page")
                                {
                                    // Read the entire <page> element into the StringBuilder
                                    xmlReader.ReadStartElement();
                                    while (xmlReader.NodeType != XmlNodeType.EndElement || xmlReader.Name != "page")
                                    {
                                        articleText.Append(xmlReader.ReadOuterXml());
                                    }
                                    articleText.Append(xmlReader.ReadOuterXml()); // Read the </page> element

                                    var text = articleText.ToString();
                                    Console.WriteLine(text);
                                    //break; // Stop reading the dump file once we've found the article
                                }
                            }
                        }

                        // Extract the article title and text
                        var articleString = articleText.ToString(); // Convert StringBuilder to string
                        var titleStart = articleString.IndexOf("<title>") + "<title>".Length;
                        var titleEnd = articleString.IndexOf("</title>");
                        var articleTitle = articleString.Substring(titleStart, titleEnd - titleStart);
                        articleTitle = new string(articleTitle.Where(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c)).ToArray());

                        var textStart = articleString.IndexOf("<text>") + "<text>".Length;
                        var textEnd = articleString.IndexOf("</text>");
                        var articleContent = articleString.Substring(textStart, textEnd - textStart);

                        // Create a directory to store the articles
                        var articlesDirectory = Path.Combine(AppContext.BaseDirectory, "articles");
                        Directory.CreateDirectory(articlesDirectory);

                        // Save the parsed article to a file
                        var articlePath = Path.Combine(articlesDirectory, $"{articleTitle}.txt");
                        File.WriteAllText(articlePath, articleContent);
                    }
                    break;
                }
            }
        }


    }
}
