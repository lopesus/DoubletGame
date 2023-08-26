//using SharpCompress.Compressors.BZip2;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Text.RegularExpressions;
//using System.Xml;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Text.RegularExpressions;
//using SharpCompress.Compressors;

//public class EEE
//{
//    public static string GetArticleText(string dumpFilePath, string indexFilePath, string articleTitle)
//    {
//        // Get the index of the article based on its title
//        int articleIndex = GetArticleIndexFromDumpFile(indexFilePath, articleTitle);

//        // Extract the byte ranges for the article from the index file
//        var articleByteRanges = GetArticleByteRangesFromIndexFile(indexFilePath, articleIndex);

//        // Open the Wikipedia dump file for reading
//        using (var dumpStream = new FileStream(dumpFilePath, FileMode.Open))
//        {
//            // Iterate over each byte range for the article
//            string articleText = null;
//            foreach (var byteRange in articleByteRanges)
//            {
//                // Seek to the start of the byte range in the dump file
//                dumpStream.Seek(byteRange.Item1, SeekOrigin.Begin);

//                // Create a BZip2 decompression stream for the byte range
//                using (var decompressionStream = new BZip2Stream(dumpStream, CompressionMode.Decompress))
//                {
//                    // Create an XML reader for the decompressed stream
//                    using (var reader = XmlReader.Create(decompressionStream))
//                    {
//                        // Move to the first page element
//                        while (reader.ReadToFollowing("page"))
//                        {
//                            // Check if this is the page we're looking for based on its index
//                            if (int.Parse(reader.GetAttribute("id")) == articleIndex)
//                            {
//                                // Move to the text element within the page
//                                reader.ReadToFollowing("text");

//                                // Read the text content of the article
//                                articleText = reader.ReadElementContentAsString().TrimEnd();
//                                break;
//                            }
//                        }
//                    }
//                }

//                if (articleText != null)
//                {
//                    break;
//                }
//            }

//            if (articleText == null)
//            {
//                throw new ArgumentException($"Article with title '{articleTitle}' not found in the Wikipedia dump file.");
//            }

//            return articleText;
//        }
//    }




//    public static List<Tuple<long, long>> GetArticleByteRangesFromIndexFile(string indexFilePath, int articleIndex)
//    {
//        // Create a list to store the byte ranges for the article
//        var articleByteRanges = new List<Tuple<long, long>>();

//        // Open the index file for reading
//        using (var indexStream = new BZip2Stream(new FileStream(indexFilePath, FileMode.Open), CompressionMode.Decompress))
//        using (var indexReader = new StreamReader(indexStream))
//        {
//            // Skip the first line of the index file (contains metadata)
//            indexReader.ReadLine();

//            // Find the line in the index file that corresponds to the article with the given index
//            string articleLine = null;
//            while (!indexReader.EndOfStream)
//            {
//                string line = indexReader.ReadLine();
//                if (line.StartsWith($"{articleIndex}:"))
//                {
//                    articleLine = line;
//                    break;
//                }
//            }

//            // If the line for the article was not found, throw an exception
//            if (articleLine == null)
//            {
//                throw new ArgumentException($"Article with index {articleIndex} not found in the Wikipedia dump index file.");
//            }

//            // Extract the byte ranges for the article from the line
//            string[] byteRanges = articleLine.Split(':')[1].Split(',');
//            foreach (string byteRange in byteRanges)
//            {
//                string[] parts = byteRange.Split('-');
//                long startByte = long.Parse(parts[0]);
//                long endByte = long.Parse(parts[1]);
//                articleByteRanges.Add(new Tuple<long, long>(startByte, endByte));
//            }
//        }

//        return articleByteRanges;
//    }


//public static int GetArticleIndexFromDumpFile(string indexFilePath, string articleTitle)
//    {
//        // Open the index file for reading
//        using (var indexStream = new BZip2Stream(new FileStream(indexFilePath, FileMode.Open), CompressionMode.Decompress))
//        using (var indexReader = new StreamReader(indexStream))
//        {
//            // Skip the first line of the index file (contains metadata)
//            indexReader.ReadLine();

//            // Find the line in the index file that corresponds to the article with the given title
//            string articleLine = null;
//            while (!indexReader.EndOfStream)
//            {
//                string line = indexReader.ReadLine();
//                if (line.EndsWith(articleTitle))
//                {
//                    articleLine = line;
//                    break;
//                }
//            }

//            // If the line for the article was not found, throw an exception
//            if (articleLine == null)
//            {
//                throw new ArgumentException($"Article with title '{articleTitle}' not found in the Wikipedia dump index file.");
//            }

//            // Extract the article index from the line
//            int articleIndex = int.Parse(articleLine.Split(':')[0]);

//            return articleIndex;
//        }
//    }


//}





