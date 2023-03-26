using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CommonLibTools.Libs;
using CommonLibTools.Libs.Extensions;
using WiktionaireParser.Models;
using WiktionaireParser.Models.wordsearch;

namespace WiktionaireParser.UiControls
{
    /// <summary>
    /// Interaction logic for DoubletFinder.xaml
    /// </summary>
    public partial class DoubletFinder : UserControl
    {
        string DicoName = @"D:\zzzWiktionnaire\frwiktionary_Parse\wordbox_valid_word_3_7.txt";
        //string DicoName = @"D:\zzzWiktionnaire\DICO\ODS8.txt";

        private List<string> wordList;
        private HashSet<string> wordListHash;
        private List<string> AllWordslist;
        AnagramBuilder anagramBuilder = new AnagramBuilder();

        public DoubletFinder()
        {
            AllWordslist = File.ReadAllLines(DicoName).Select(m => m.ToLowerInvariant().SansAccent()).ToList();
            wordListHash = new HashSet<string>(AllWordslist);
            wordList = wordListHash.ToList();
            InitializeComponent();

            lbxWordList.ItemsSource = wordList;
            cbxLen.ItemsSource = Enumerable.Range(3, 8);
            cbxLen.SelectIndex(0);

            foreach (var word in AllWordslist)
            {
                var anagram = word;
                var anagramKey = anagram.SortString();
                anagramBuilder.Add(anagramKey, anagram);
            }

        }

        private void lbxWordList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var word = lbxWordList.SelectedItem as string;
            if (word == null) return;

            var list = ShiftUtils.GenerateWordsByInsertion(word);

            var valid = list.Where(w => AllWordslist.Contains(w));
            var builder = new StringBuilder();
            builder.AppendLine($"all words");
            var res = string.Join(" ", list);
            builder.AppendLine(res);
            builder.AppendLine();
            builder.AppendLine($"valid words");
            builder.AppendLine(string.Join(" ", valid));

            //anagrams
            builder.AppendLine().AppendLine($"valid permutations");
            var anagramList = anagramBuilder.GetAnagramFor(word.SortString())?.AnagramList;
            builder.AppendLine(string.Join(" ", anagramList ?? new List<string>()));
            txtResult.Text = builder.ToString();
        }

        private void cmdFilter_Click(object sender, RoutedEventArgs e)
        {
            int len = Convert.ToInt32(cbxLen.GetSelectedItem().ToString());

            wordListHash = new HashSet<string>(AllWordslist.Where(w => w.Length == len));
            wordList = wordListHash.ToList();

            lbxWordList.ItemsSource = wordList;
        }
    }
}
