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

        public CrossWordGrid WordGrid { get; set; }

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
        public CrossWordGridGen()
        {
            InitializeComponent();

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

        private void cmdPutNextWord_Click(object sender, RoutedEventArgs e)
        {
            string word = GetNextWord();
            if (WordGrid.IsEmpty)
            {
                var list = WordGrid.GetStartingCoordFor(word);
                var startingPosition = list.PickRandom();
                PutWordAt(word, startingPosition.Coord, startingPosition.Direction);
            }
            else
            {
                var crossingIndexHoriz = WordGrid.SelectRandomAnchor(word, CrossWordDirection.Horizontal);
                var crossingIndexVert = WordGrid.SelectRandomAnchor(word, CrossWordDirection.Vertical);
                var crossingIndex = WordGrid.SelectRandomAnchor(word);
                Console.WriteLine();

                if (crossingIndex != null)
                {
                    var start = crossingIndex.StartCoord;
                    PutWordAt(word,start,crossingIndex.Direction);
                }
                else
                {
                    MessageBox.Show($"NO CROSSING for {word}".ToUpper());
                }
            }
        }
        
        void PutWordAt(string word, Coord coord, CrossWordDirection direction)
        {

            CrossWord crossWord = new CrossWord(word, coord, direction);
            WordGrid.PutWordAt(crossWord, coord, direction);

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
                   var uiCell= UIMazeCellList[row, col];
                   uiCell.UpdateData();
                }
            }
        }
        public void SetMaze()
        {
            mainCanvas.Children.Clear();
            wordListCopy = wordList.ToList();

            WordGrid = new CrossWordGrid(NumRow, NumCol);

            mainCanvas.Width = NumCol * UICell.CellSize;
            mainCanvas.Height = NumRow * UICell.CellSize;

            UIMazeCellList = new UICell[NumRow, NumCol];
            var empty = false;

            for (int col = 0; col < NumCol; col++)
            {
                for (int row = 0; row < NumRow; row++)
                {
                    empty = random.Next(0, 101) > 50;
                    var wordCell = WordGrid.MazeCellList[row, col];
                    UICell uiCell = new UICell(wordCell);
                    mainCanvas.Children.Add(uiCell);
                    UIMazeCellList[row, col] = uiCell;
                }
            }

        }

    }
}
