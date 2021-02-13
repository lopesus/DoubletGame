using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonLibTools.Libs.DataStructure.Dawg.Construction;

namespace Test
{
    class Program
    {
        private static ConstructTrie constructTrie = new ConstructTrie();
        static void Main(string[] args)
        {
            //constructTrie.GenerateTrieFromFile(@"C:\Users\mboum\Desktop\web\verbes\ods8_final.txt");
            //constructTrie.GenerateTrieFromFile(@"G:\zzzWiktionnaire\DICO\ODS7.txt");
            constructTrie.GenerateTrieFromFile(@"D:\__programs_datas\ods8_final_no_verbs_4_to_7.txt");


            //var text = File.ReadAllText(@"D:\__programs_datas\wiki_valid_word_trie.txt");
            //var lines = File.ReadAllLines(@"D:\__programs_datas\wiki_valid_word_trie.txt");

            //string[] allLines = text.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            //var trie0 = new Trie();
            //trie0.LoadFromUniqueFileData(allLines);
            //trie0.LoadFromUniqueFileData(lines);

            //var dawgService = new DawgService(lines);
            //var trie = dawgService.GetTrie();

            Console.ReadLine();
        }
    }
}
