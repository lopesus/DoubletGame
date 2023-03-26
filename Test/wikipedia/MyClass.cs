using SharpCompress.Compressors.BZip2;
using System.IO;
using System;
using System.IO.Compression;
using System.Xml;

namespace Test.wikipedia
{
    public class MyClass
    {

        public static void ExtractWikipediaArticle(string compressedFile, string indexFile, string articleTitle, string outputFile)
        {
            // Open the index file and read the offset and size of the specified article
            using (var indexStream = File.OpenRead(indexFile))
            using (var indexReader = new BinaryReader(indexStream))
            {
                long offset = -1;
                long size = -1;

                while (indexStream.Position < indexStream.Length)
                {
                    string title = ReadString(indexReader);
                    long streamOffset = indexReader.ReadInt64();
                    long streamSize = indexReader.ReadInt64();

                    if (title == articleTitle)
                    {
                        offset = streamOffset;
                        size = streamSize;
                        break;
                    }
                }

                if (offset == -1)
                {
                    throw new ArgumentException($"Article '{articleTitle}' not found in index file '{indexFile}'.");
                }

                // Open the compressed file and seek to the start of the specified stream
                using (var compressedStream = File.OpenRead(compressedFile))
                {
                    compressedStream.Seek(offset, SeekOrigin.Begin);

                    // Create a BZip2Stream that reads only the specified stream
                    var bzip2Stream = new BZip2Stream(compressedStream, SharpCompress.Compressors.CompressionMode.Decompress, true);
                    var limitedStream = new LimitedInputStream(bzip2Stream, size);

                    // Create an XmlReader to parse the article from the stream
                    var settings = new XmlReaderSettings { DtdProcessing = DtdProcessing.Ignore };
                    using (var xmlReader = XmlReader.Create(limitedStream, settings))
                    {
                        while (xmlReader.Read())
                        {
                            if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "page")
                            {
                                // Create the output file and write the contents of the article to it
                                using (var outputStream = File.Create(outputFile))
                                {
                                    //xmlReader.WriteTo(outputStream);
                                }
                                return;
                            }
                        }
                    }
                }
            }
        }

        private static string ReadString(BinaryReader reader)
        {
            int length = reader.ReadInt32();
            byte[] bytes = reader.ReadBytes(length);
            return System.Text.Encoding.UTF8.GetString(bytes);
        }

    }
}