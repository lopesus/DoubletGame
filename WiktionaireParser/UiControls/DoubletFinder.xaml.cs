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

namespace WiktionaireParser.UiControls
{
    /// <summary>
    /// Interaction logic for DoubletFinder.xaml
    /// </summary>
    public partial class DoubletFinder : UserControl
    {
          string DicoName = @"D:\zzzWiktionnaire\frwiktionary_Parse\wordbox_valid_word_3_15.txt";
        private List<string> wordList;
        public DoubletFinder()
        {
            var list = File.ReadAllLines(DicoName).ToList();
            var hash = new HashSet<string>(list);
            wordList = hash.ToList();
            InitializeComponent();
        }
    }
}
