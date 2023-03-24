using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DoubletGame.Algo;

namespace DoubletGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string fileName = @"D:\zzzWiktionnaire\DICO\ODS8.txt";
        DoubletFinder finder;
        private List<string> selection;
        private List<string> doubletResultList;
        private List<string> wordList;

        public MainWindow()
        {
            var list = File.ReadAllLines(fileName).Select(m=>m.ToLowerInvariant()).ToList();
            var hash=new HashSet<string>(list);
            wordList = hash.ToList();

            InitializeComponent();
            lbxWorlist.ItemsSource = wordList;
        }

        private void lbxWorlist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var word = lbxWorlist.SelectedItem as string;
            doubletResultList = finder.GetDoubletFor(word);
            lbxDoubleResult.ItemsSource = doubletResultList;
        }

        private void lbxDoubleResult_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var word = lbxDoubleResult.SelectedItem as string;
            if (word != null)
            {
                var index = selection.IndexOf(word);
                lbxWorlist.SelectedIndex = index;
            }
        }

        private void cmdFndDoublet_Click(object sender, RoutedEventArgs e)
        {
            int size;
            try
            {
                size = Convert.ToInt32(txtLen.Text);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                size = 4;
            }

            selection = wordList.Where(w => w.Length == size).ToList();
            tblResult.Text = $" found {selection.Count} of {size}";
            lbxWorlist.ItemsSource = selection;

            finder = new DoubletFinder(selection);
            finder.FindDoublet();
            lbxWorlist.SelectedIndex = 0;
        }

        private void txtFindLink_Click(object sender, RoutedEventArgs e)
        {
            var source = txtFrom.Text.Trim().ToLowerInvariant();
            var dest = txtTo.Text.Trim().ToLowerInvariant();
            int maxMove = 10;
            try
            {
                maxMove = Convert.ToInt32(txtLimit.Text);
            }
            catch (Exception ex)
            {
            }
            var result = finder.GetWordsInBetween(source, dest, maxMove);
            var resultList = result.OrderBy(r => r.WordList.Count).ToList();

            tblResult.Text = $"link count is {resultList.Count}";

            lbxLinkResult.ItemsSource = resultList;

            MessageBox.Show("fini");
        }

        private void lbxLinkResult_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DoubletResult doubletResult = lbxLinkResult.SelectedItem as DoubletResult;

            if (doubletResult != null) lbxLinDetail.ItemsSource = doubletResult.WordList;
        }

        private void lbxLinDetail_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var word = lbxLinDetail.SelectedItem as string;
            if (word != null)
            {
                var index = selection.IndexOf(word);
                lbxWorlist.SelectedIndex = index;
            }
        }
    }
}
