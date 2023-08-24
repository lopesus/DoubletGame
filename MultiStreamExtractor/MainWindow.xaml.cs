using System;
using System.Collections.Generic;
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

            var reader = new WikipediaReader(
                @"D:\zzzWiktionnaire\wikipedia\frwiki-20230320-pages-articles-multistream.xml.bz2",
                @"D:\zzzWiktionnaire\wikipedia\frwiki-20230320-pages-articles-multistream-index.txt"
            );

            var articleContent = reader.ExtractArticleContent("Mangue");
            txtPage.Text= articleContent;
            Console.WriteLine(articleContent);
        }
    }
}