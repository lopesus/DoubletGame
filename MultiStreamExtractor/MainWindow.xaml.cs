using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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

namespace MultiStreamExtractor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //var readerSingle = new WikipediaReaderSingle(
            //    @"D:\zzzWiktionnaire\wikipedia\frwiki-20230320-pages-articles-multistream.xml.bz2",
            //    @"D:\zzzWiktionnaire\wikipedia\frwiki-20230320-pages-articles-multistream-index.txt"
            //);

            //var articleContent = readerSingle.ExtractArticleContent("Mangue");
            //txtPage.Text = articleContent;
            //Console.WriteLine(articleContent);


           
        }

        private void cmdExtract_Click(object sender, RoutedEventArgs e)
        {
            var processAllChunk = false;
            var artciclePerchunk = 1;
            var savePage = true;
            var reader = new WikipediaReader(
                @"D:\zzzWiktionnaire\wikipedia\frwiki-20230320-pages-articles-multistream.xml.bz2",
                @"D:\zzzWiktionnaire\wikipedia\frwiki-20230320-pages-articles-multistream-index.txt",
                processAllChunk,
                artciclePerchunk,
                savePage
            );

            var path = @"D:\zzzWiktionnaire\wikipedia\zzz_test_extraction";
            Directory.CreateDirectory(path);
            int numberOfCores = Environment.ProcessorCount;

            Stopwatch sw = Stopwatch.StartNew();
            reader.ExtractAndProcessArticles(path);
            sw.Stop();
            var elapsed = sw.Elapsed.TotalMinutes;

            MessageBox.Show($"DONE IN {elapsed} min");
        }
    }
}