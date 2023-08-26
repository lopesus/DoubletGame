//using SharpCompress.Compressors.BZip2;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Text;
//using SharpCompress.Compressors;

//namespace WiktionaireParser.Models.Wikipedia
//{
//    public class Tools
//    {
//        public static int GetArticleIndexFromDumpFile(string indexFilePath, string articleTitle)
//        {
//            // Open the index file for reading
//            using (var indexStream = new BZip2Stream(new FileStream(indexFilePath, FileMode.Open), CompressionMode.Decompress))
//            using (var indexReader = new StreamReader(indexStream))
//            {
//                // Skip the first line of the index file (contains metadata)
//                indexReader.ReadLine();

//                // Find the line in the index file that corresponds to the article with the given title
//                string articleLine = null;
//                while (!indexReader.EndOfStream)
//                {
//                    string line = indexReader.ReadLine();
//                    if (line.EndsWith(articleTitle))
//                    {
//                        articleLine = line;
//                        break;
//                    }
//                }

//                // If the line for the article was not found, throw an exception
//                if (articleLine == null)
//                {
//                    throw new ArgumentException($"Article with title '{articleTitle}' not found in the Wikipedia dump index file.");
//                }

//                // Extract the article index from the line
//                int articleIndex = int.Parse(articleLine.Split(':')[0]);

//                return articleIndex;
//            }
//        }

//    }
//}
