using System.IO;
using SharpCompress.Compressors.BZip2;

namespace Test.wikipedia
{
    public class Tools
    {
        public static void ExtractBZip2Stream(string compressedFile, string indexFile, int streamIndex, string outputFile)
        {
            // Open the index file and read the offset and size of the specified stream
            using (var indexStream = File.OpenRead(indexFile))
            using (var indexReader = new BinaryReader(indexStream))
            {
                indexStream.Seek(streamIndex * 8, SeekOrigin.Begin);
                long offset = indexReader.ReadInt64();
                long size = indexReader.ReadInt64();

                // Open the compressed file and seek to the start of the specified stream
                using (var compressedStream = File.OpenRead(compressedFile))
                {
                    compressedStream.Seek(offset, SeekOrigin.Begin);

                    // Create a BZip2Stream that reads only the specified stream
                    var bzip2Stream = new BZip2Stream(compressedStream, SharpCompress.Compressors.CompressionMode.Decompress, true);
                    var limitedStream = new LimitedInputStream(bzip2Stream, size);

                    // Create the output file and copy the contents of the limited stream to it
                    using (var outputStream = File.Create(outputFile))
                    {
                        limitedStream.CopyTo(outputStream);
                    }
                }
            }
        }


        public static void ExtractBZip2StreamChunk(string compressedFile, long offset, long size, string outputFile)
        {
            using (var compressedStream = File.OpenRead(compressedFile))
            {
                compressedStream.Seek(offset, SeekOrigin.Begin);

                // Create a BZip2Stream that reads only the specified stream
                var bzip2Stream = new BZip2Stream(compressedStream, SharpCompress.Compressors.CompressionMode.Decompress, true);
                var limitedStream = new LimitedInputStream(bzip2Stream, size);

                // Create the output file and copy the contents of the limited stream to it
                using (var outputStream = File.Create(outputFile))
                {
                    limitedStream.CopyTo(outputStream);
                }
            }


            //// Open the index file and read the offset and size of the specified stream
            //using (var indexStream = File.OpenRead(indexFile))
            //using (var indexReader = new BinaryReader(indexStream))
            //{
            //    indexStream.Seek(streamIndex * 8, SeekOrigin.Begin);
            //    long offset = indexReader.ReadInt64();
            //    long size = indexReader.ReadInt64();

            //    // Open the compressed file and seek to the start of the specified stream
               
            //}
        }

        public static void ExtractBZip2StreamArticle(string compressedFile, long offset, long size, string outputFile)
        {
            using (var compressedStream = File.OpenRead(compressedFile))
            {
                compressedStream.Seek(offset, SeekOrigin.Begin);

                // Create a BZip2Stream that reads only the specified stream
                var bzip2Stream = new BZip2Stream(compressedStream, SharpCompress.Compressors.CompressionMode.Decompress, true);
                var limitedStream = new LimitedInputStream(bzip2Stream, size);

                // Create the output file and copy the contents of the limited stream to it
                using (var outputStream = File.Create(outputFile))
                {
                    limitedStream.CopyTo(outputStream);
                }
            }


            //// Open the index file and read the offset and size of the specified stream
            //using (var indexStream = File.OpenRead(indexFile))
            //using (var indexReader = new BinaryReader(indexStream))
            //{
            //    indexStream.Seek(streamIndex * 8, SeekOrigin.Begin);
            //    long offset = indexReader.ReadInt64();
            //    long size = indexReader.ReadInt64();

            //    // Open the compressed file and seek to the start of the specified stream

            //}
        }
    }
}