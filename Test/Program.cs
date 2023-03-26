using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonLibTools.Libs.DataStructure.Dawg.Construction;
using SharpCompress.Common;
using SharpCompress.Readers;
using Test.wikipedia;
using System.IO;
using SharpCompress.Archives;
using SharpCompress.Common;
using SharpCompress.Readers;

namespace Test
{
    class Program
    {
        private static ConstructTrie constructTrie = new ConstructTrie();
        static void Main(string[] args)
        {
            var articleDumpPath = @"D:\zzzWiktionnaire\wikipedia\frwiki-20230320-pages-articles-multistream.xml.bz2";
            var indexPath = @"D:\zzzWiktionnaire\wikipedia\frwiki-20230320-pages-articles-multistream-index.txt";
            var indexPathBZip = @"D:\zzzWiktionnaire\wikipedia\frwiki-20230320-pages-articles-multistream-index.txt.bz2";
            //var output = @"D:\zzzWiktionnaire\wikiFR2.txt";


            long start = 675;
            long end = 1293840;

            var output = $@"D:\zzzWiktionnaire\chunk_{start}_{end}.txt";
            //Tools.ExtractBZip2StreamChunk(articleDumpPath,start,end-start,output);
            Tools.ExtractBZip2StreamChunk(articleDumpPath,start,end,output);




            //// Define the path to the Multistream BZ2 file and the output directory path
            //string filePath = "path/to/file.bz2";
            //string outputDirectory = "path/to/output/directory";

            //// Open the Multistream BZ2 file for reading
            //using (var stream = File.OpenRead(filePath))
            //{
            //    // Create a reader for the Multistream BZ2 file
            //    var reader = ReaderFactory.Open(stream, ArchiveType.BZip2);

            //    // Extract all streams to the output directory
            //    while (reader.MoveToNextEntry())
            //    {
            //        if (!reader.Entry.IsDirectory)
            //        {
            //            reader.WriteEntryToDirectory(outputDirectory, new ExtractionOptions()
            //            {
            //                ExtractFullPath = true,
            //                Overwrite = true
            //            });
            //        }
            //    }
            //}


            return;
            //constructTrie.GenerateTrieFromFile(@"C:\Users\mboum\Desktop\web\verbes\ods8_final.txt");
            //constructTrie.GenerateTrieFromFile(@"G:\zzzWiktionnaire\DICO\ODS7.txt");

            //constructTrie.GenerateTrieFromFile(@"D:\zzzWiktionnaire\DICO\ODS8.txt");


            //var text = File.ReadAllText(@"D:\__programs_datas\wiki_valid_word_trie.txt");
            //var lines = File.ReadAllLines(@"D:\__programs_datas\wiki_valid_word_trie.txt");

            //string[] allLines = text.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            //var trie0 = new Trie();
            //trie0.LoadFromUniqueFileData(allLines);
            //trie0.LoadFromUniqueFileData(lines);

            //var dawgService = new DawgService(lines);
            //var trie = dawgService.GetTrie();


          
            Tools.ExtractBZip2Stream(articleDumpPath, indexPathBZip, 2, @"D:\zzzWiktionnaire\wikipedia\stream2.txt");


           // var text = WikiDump.GetArticleText(articleDumpPath, indexPath, "Mangue");
            //Console.WriteLine(text);

            Console.ReadLine();
        }
    }
}
