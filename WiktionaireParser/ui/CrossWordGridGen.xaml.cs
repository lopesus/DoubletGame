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
using CommonLibTools;
using CommonLibTools.DataStructure.Dawg;
using CommonLibTools.Extensions;
using PathFindingModel;
using WiktionaireParser.Models.CrossWord;

namespace WiktionaireParser.ui
{
    /// <summary>
    /// Interaction logic for CrossWordGridGen.xaml
    /// </summary>
    public partial class CrossWordGridGen : UserControl
    {
        private UICell[,] UIMazeCellList;
        public static int NumCol = 9;
        public static int NumRow = 9;

        public CrossWordGrid Grid { get; set; }

        Random random = new Random();


        List<string> wordList = new List<string>()
        {
            "CRUE",
            "CURE",
            "recu",
            "ECRU",
            "CRU",
            "CUE",
            "CUR",
            "ECU",
            "REC",
            "RUC",
            "RUE",
            "URE",
        };

        private List<string> wordListCopy = new List<string>();

        List<CrossWord> fitOnGridList { get; set; }

        public DawgService DawgService { get; set; }

        public CrossWordGridGen()
        {
            InitializeComponent();
            DawgService = MainWindow.DawgService;
            wordListCopy = wordList.ToList();
        }
        public UICell GetUICell(Coord coord)
        {
            try
            {
                return UIMazeCellList[coord.Row, coord.Col];
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;

            }

        }

        string GetNextWord()
        {
            // var word = wordListCopy.PickRandom();
            var word = wordListCopy.FirstOrDefault();
            wordListCopy.Remove(word);
            return word?.ToLowerInvariant();
        }


        private void lbxGeneators_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var gen = lbxGeneators.SelectedItem as CrossWordGenerator;
            if (gen != null)
            {
                DrawGrid(gen);
            }
        }


        private void cmdGenMany_Click(object sender, RoutedEventArgs e)
        {
            DawgService = MainWindow.DawgService;
            var text = txtWordForGrid.Text;
            if (text.IsNullOrEmptyString())
            {
                text = "cure";
            }

            var allPossibleWord = DawgService.FindAllPossibleWord(text);
            var set=new HashSet<string>();
            foreach (KeyValuePair<int, List<string>> pair in allPossibleWord.Where(p=>p.Key>2))
            {
                set.UnionWith(pair.Value);
            }

            var list1 = set.OrderByDescending(d => d.Length).ToList();

            var temp = string.Join(" ", list1);
            tblAllWord.Text = temp;
            GenerateManyGrid(list1);
        }

        void GenerateManyGrid(List<string> aList)
        {
            int take = 1000;
            var result = new List<CrossWordGenerator>();
            var wordList = aList.OrderByDescending(d => d.Length).ToList();
            var testGrid = new CrossWordGrid(NumRow, NumCol);
            var startingPositions = testGrid.GetStartingCoordFor(wordList.First());

            foreach (var startingPosition in startingPositions.Take(take))
            {
                var generator = new CrossWordGenerator(NumRow, NumCol, wordList, startingPosition);
                result.Add(generator);
            }

            lbxGeneators.ItemsSource = result.OrderByDescending(g=>g.FitWordList.Count);
        }

        public void DrawGrid(CrossWordGenerator generator)
        {
            SetMaze();
            foreach (var crossWord in generator.FitWordList)
            {
                foreach (var letter in crossWord.WordLetterList)
                {
                    var cellCoord = letter.Coord;
                    var uiCell = UIMazeCellList[cellCoord.Row, cellCoord.Col];
                    uiCell.DrawLetter(letter);
                }
            }
        }

        private void cmdGenFullGrid_Click(object sender, RoutedEventArgs e)
        {
            SetMaze();
            GenFullCrossword();
        }

       
        void GenFullCrossword()
        {
            fitOnGridList = new List<CrossWord>();
            Dictionary<string, int> rejected = new Dictionary<string, int>();
            string word = GetNextWord();
            var tripleRejection = false;

            var list = Grid.GetStartingCoordFor(word);
            var startingPosition = list.PickRandom();
            PutWordAt(word, startingPosition.Coord, startingPosition.Direction);

            var uiCell = UIMazeCellList[startingPosition.Coord.Row, startingPosition.Coord.Col];
            uiCell.SetAsGrigStartingCell();

            word = GetNextWord();

            while (word != null && tripleRejection == false)
            {
                //var crossingIndexHoriz = Grid.SelectRandomAnchor(word, CrossWordDirection.Horizontal);
                //var crossingIndexVert = Grid.SelectRandomAnchor(word, CrossWordDirection.Vertical);
                var crossingIndex = Grid.SelectRandomAnchor(word);
                Console.WriteLine();

                if (crossingIndex != null)
                {
                    var start = crossingIndex.StartCoord;
                    PutWordAt(word, start, crossingIndex.Direction);
                    rejected.Clear();
                }
                else
                {
                    if (rejected.ContainsKey(word) == false) rejected[word] = 0;
                    rejected[word] += 1;
                    if (rejected[word] >= 2)
                    {
                        tripleRejection = true;
                    }
                    wordListCopy.Add(word);

                }

                word = GetNextWord();
            }

            MessageBox.Show($"fit on grid {fitOnGridList.Count} on {wordList.Count}".ToUpper());

        }

        private void cmdPutNextWord_Click(object sender, RoutedEventArgs e)
        {
            string word = GetNextWord();
            if (Grid.IsEmpty)
            {
                var list = Grid.GetStartingCoordFor(word);
                var startingPosition = list.PickRandom();
                PutWordAt(word, startingPosition.Coord, startingPosition.Direction);
            }
            else
            {
                var crossingIndexHoriz = Grid.SelectRandomAnchor(word, CrossWordDirection.Horizontal);
                var crossingIndexVert = Grid.SelectRandomAnchor(word, CrossWordDirection.Vertical);
                var crossingIndex = Grid.SelectRandomAnchor(word);
                Console.WriteLine();

                if (crossingIndex != null)
                {
                    var start = crossingIndex.StartCoord;
                    PutWordAt(word, start, crossingIndex.Direction);
                }
                else
                {
                    wordListCopy.Add(word);
                    MessageBox.Show($"NO CROSSING for {word}".ToUpper());
                }
            }
        }



        void PutWordAt(string word, Coord coord, CrossWordDirection direction)
        {
            CrossWord crossWord = new CrossWord(word, coord, direction);
            fitOnGridList.Add(crossWord);

            Grid.PutWordAt(crossWord, coord, direction);

            foreach (var crossWordCell in crossWord.WordLetterList)
            {
                var cellCoord = crossWordCell.Coord;
                var uiCell = UIMazeCellList[cellCoord.Row, cellCoord.Col];
                uiCell.SetWord(crossWordCell);
            }

            //var uiCell2 = GetUICell(crossWord.BeforeStartCell.Coord);
            //if (uiCell2 != null)
            //{
            //    uiCell2.SetWord(crossWord.BeforeStartCell);
            //}

            //uiCell2 = GetUICell(crossWord.AfterEndCell.Coord);
            //if (uiCell2 != null)
            //{
            //    uiCell2.SetWord(crossWord.AfterEndCell);
            //}

            RefreshUIGrid();
        }

        private void cmdGenGrid_Click(object sender, RoutedEventArgs e)
        {
            SetMaze();
        }

        public void GenerateGridForWord()
        {

        }

        bool CanFitOnGrid(string word, Coord coord, CrossWordDirection direction)
        {

            return false;
        }

        void CanFitHorizontally(string word, Coord coord)
        {

        }

        int GetDistanceToGridEdge(Coord coord, CrossWordDirection direction)
        {
            switch (direction)
            {
                case CrossWordDirection.Horizontal:
                    return NumCol - coord.Col;
                case CrossWordDirection.Vertical:
                    return NumRow - coord.Row;
                default:
                    return 0;
            }
        }

        void RefreshUIGrid()
        {
            for (int col = 0; col < NumCol; col++)
            {
                for (int row = 0; row < NumRow; row++)
                {
                    var uiCell = UIMazeCellList[row, col];
                    uiCell.UpdateData();
                }
            }
        }
        public void SetMaze()
        {
            mainCanvas.Children.Clear();
            wordListCopy = wordList.ToList();

            Grid = new CrossWordGrid(NumRow, NumCol);

            mainCanvas.Width = NumCol * UICell.CellSize;
            mainCanvas.Height = NumRow * UICell.CellSize;

            UIMazeCellList = new UICell[NumRow, NumCol];
            var empty = false;

            for (int col = 0; col < NumCol; col++)
            {
                for (int row = 0; row < NumRow; row++)
                {
                    empty = random.Next(0, 101) > 50;
                    var wordCell = Grid.MazeCellList[row, col];
                    UICell uiCell = new UICell(wordCell);
                    mainCanvas.Children.Add(uiCell);
                    UIMazeCellList[row, col] = uiCell;
                }
            }

        }

    }
}
