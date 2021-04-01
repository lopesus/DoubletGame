using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
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
using CommonLibTools;
using CommonLibTools.Libs;
using CommonLibTools.Libs.CrossWord;
using CommonLibTools.Libs.DataStructure.Dawg;
using CommonLibTools.Libs.Extensions;
using Newtonsoft.Json;

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
        public static int BranchLimit = 0;
        public static int DepthLimit = 3;
        private DateTime start;
        private DateTime end;

        string frequencyDicoFileName = @"D:\__programs_datas\ods8_final_no_verbs_4_to_7_with_freq.txt";

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
        private int gridSize;
        private List<string> AllWords;
        private Dictionary<string, float> frequencyDico;

        List<CrossWord> fitOnGridList { get; set; }

        public DawgService DawgService { get; set; }

        public CrossWordGridGen()
        {
            InitializeComponent();
            gridSize = 9;
            DawgService = MainWindow.DawgService;
            wordListCopy = wordList.ToList();
            cbsGridSize.ItemsSource = Enumerable.Range(7, 12);
            cbsGridSize.SelectedIndex = 2;

            AllWords = File.ReadAllLines(MainWindow.DicoName).ToList().RemovePluralForm();
            lbxDicoCrossWord.ItemsSource = AllWords;
            cbxBranchLimit.ItemsSource = Enumerable.Range(0, 10);
            cbxBranchLimit.SelectedIndex = 0;

            cbxDepthLimit.ItemsSource = Enumerable.Range(1, 5);
            cbxDepthLimit.SelectedIndex = 0;

            frequencyDico = LoadFrequencyDico(frequencyDicoFileName);
        }

        Dictionary<string, float> LoadFrequencyDico(string frequencyDicoFileName)
        {
            var lines = File.ReadAllLines(frequencyDicoFileName);

            var valids = new Dictionary<string, float>();

            foreach (var line in lines)
            {
                var tokens = line.Split();
                var mot = tokens.First();
                var freq = Convert.ToDouble(tokens.Last());

                valids[mot] = (float)freq;

            }

            return valids;
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
                foreach (var crossWord in gen.FitWordList)
                {
                    var zipf = frequencyDico[crossWord.Word];
                    crossWord.Freq = zipf;
                }
                lbxFitWord.ItemsSource = gen.FitWordList.OrderByDescending(w=>w.Freq);
            }
        }



        private void cmdGenGameLevel_Click(object sender, RoutedEventArgs e)
        {
            //SelectLevelSequential();
            SelectLevelParallel();
        }


        void SelectLevelSequential()

        {
            gridSize = 7;
            int minLen = 6;
            DawgService = MainWindow.DawgService;
            List<GenGrid> genGrids = new List<GenGrid>();
            Dictionary<string, List<string>> selectedWord = new Dictionary<string, List<string>>();

            foreach (var allWord in AllWords.Where(w => w.Length >= minLen && w.Length <= 7))
            {
                var word = allWord.ToLowerInvariant().RemoveDiacritics();
                var allPossibleWord = DawgService.FindAllPossibleWord(word);
                var set = new HashSet<string>();
                foreach (KeyValuePair<int, List<string>> pair in allPossibleWord.Where(p => p.Key > 2))
                {
                    set.UnionWith(pair.Value);
                }

                var list1 = set.OrderByDescending(d => d.Length).ToList();
                if (list1.Count >= 4)
                {
                    selectedWord.Add(word, list1);
                }
            }

            var wordsToTake = 500;



            foreach (var allWord in selectedWord.Take(wordsToTake))
            {
                var word = allWord.Key;
                var list1 = allWord.Value;
                var result = GenGrid.GenAll(word, list1, gridSize, BranchLimit,DepthLimit,frequencyDico);
                var grid = result.OrderByDescending(g => g.FitWordList.Count)
                    .ThenBy(g => g.Grid.BaryDistance).FirstOrDefault();

                genGrids.Add(grid);
            }

            lbxGeneators.ItemsSource = genGrids;
            CrossWordGame crossWordGame = new CrossWordGame(Lang.Fr, genGrids.ToList());
            var json = JsonConvert.SerializeObject(crossWordGame);

            string folder = @"D:\__programs_datas\";
            var gameDataPath = $"{folder}wordbox_{crossWordGame.Language}.json";
            File.WriteAllText(gameDataPath, json);
            Console.WriteLine(crossWordGame.Language);

            MessageBox.Show($"{genGrids.Count} level generated - {selectedWord.Count} words treated ");

        }

        void SelectLevelParallel()
        {
            var start = DateTime.UtcNow;
            ParallelOptions options = new ParallelOptions()
            {
                MaxDegreeOfParallelism = 12
            };
            gridSize = 7;
            int minLen = 4;
            int maxLen = 4;
            DawgService = MainWindow.DawgService;

            ConcurrentBag<GenGrid> genGrids = new ConcurrentBag<GenGrid>();
            ConcurrentDictionary<string, List<string>> selectedWord = new ConcurrentDictionary<string, List<string>>();


            var source = AllWords.Where(w => w.Length >= minLen && w.Length <= maxLen).ToList();
            Parallel.ForEach(source, options, allWord =>
              {
                  var word = allWord.ToLowerInvariant().RemoveDiacritics();
                  var allPossibleWord = DawgService.FindAllPossibleWord(word);
                  var set = new HashSet<string>();
                  foreach (KeyValuePair<int, List<string>> pair in allPossibleWord.Where(p => p.Key > 2))
                  {
                      set.UnionWith(pair.Value);
                  }

                  var list1 = set.OrderByDescending(d => d.Length).ToList().RemovePluralForm();
                  if (list1.Count >= 4)
                  {
                      selectedWord.TryAdd(word, list1);
                  }
              });

            var wordsToTake = 100;

            Parallel.ForEach(selectedWord.Take(wordsToTake), options, allWord =>
             {
                 var word = allWord.Key;
                 var list1 = allWord.Value;

                 if (word.Length >= 7)
                 {
                     gridSize = 9;
                 }
                 else
                 {
                     gridSize = 7;
                 }

                 var result = GenGrid.GenAll(word, list1, gridSize, BranchLimit,DepthLimit,frequencyDico);
                 if (result?.Count > 0)
                 {
                     var grid = result.OrderByDescending(g => g.FitWordList.Count)
                         .ThenBy(g => g.Grid.BaryDistance).FirstOrDefault();

                     genGrids.Add(grid);
                 }
             });


            var final = genGrids.OrderByDescending(g => g.Difficulty).ToList();
            lbxGeneators.ItemsSource = final;
            CrossWordGame crossWordGame = new CrossWordGame(Lang.Fr, final);
            var json = JsonConvert.SerializeObject(crossWordGame);

            string folder = @"D:\__programs_datas\";
            var gameDataPath = $"{folder}wordbox_{crossWordGame.Language}.json";
            File.WriteAllText(gameDataPath, json);
            Console.WriteLine(crossWordGame.Language);

            var end = DateTime.UtcNow;
            var duration = end.Subtract(start);

            MessageBox.Show($"{final.Count} level generated - {selectedWord.Count} words treated \r\n duree {duration.TotalMinutes}");

        }
        private void cmdGenMany_Click(object sender, RoutedEventArgs e)
        {
            start=DateTime.UtcNow;
            gridSize = (int)cbsGridSize.SelectedValue;
            DawgService = MainWindow.DawgService;
            var text = txtWordForGrid.Text.ToLowerInvariant().RemoveDiacritics();
            if (text.IsNullOrEmptyString())
            {
                text = "cure";
            }

            var allPossibleWord = DawgService.FindAllPossibleWord(text);
            var set = new HashSet<string>();
            foreach (KeyValuePair<int, List<string>> pair in allPossibleWord.Where(p => p.Key > 2))
            {
                set.UnionWith(pair.Value);
            }

            var list1 = set.OrderByDescending(d => d.Length).ToList();
            list1 = list1.RemovePluralForm();
            var temp = string.Join(" ", list1);
            tblAllWord.Text = temp;

            //GenerateManyGrid(list1, gridSize);
            var result = GenGrid.GenAll(text, list1, gridSize, BranchLimit,DepthLimit,frequencyDico);
            lbxGeneators.ItemsSource = result
                .OrderByDescending(g => g.FitWordList.Count)
                .ThenBy(g => g.Grid.BaryDistance);

            end=DateTime.UtcNow;
            var diff = end.Subtract(start);
            tblResultCount.Text = $"gen many {result.Count} \r\ntime {diff.TotalSeconds} sec branch {BranchLimit} depth {DepthLimit}";

        }


        //ConcurrentBag<GenGrid> GenAll(string letters, List<string> listOfWords, int size)
        //{
        //    int take = 10000;
        //    var result = new ConcurrentBag<GenGrid>();
        //    listOfWords = listOfWords.OrderByDescending(d => d.Length).ToList();
        //    var testGrid = new CrossWordGrid(size, size);
        //    var word = listOfWords.First().ToLowerInvariant();
        //    //listOfWords.Remove(word);
        //    var startingPositions = testGrid.GetStartingCoordFor(word);

        //    //foreach (var startingPosition in startingPositions)
        //    //{
        //    //    var generator = new GenGrid(size, size, letters, new List<string>(listOfWords), startingPosition, result,BranchLimit);
        //    //}

        //    ParallelOptions options = new ParallelOptions()
        //    {
        //        MaxDegreeOfParallelism = 6
        //    };
        //    Parallel.ForEach(startingPositions, options, startingPosition =>
        //    {
        //        var generator = new GenGrid(size, size, letters, new List<string>(listOfWords), startingPosition, result, BranchLimit);
        //    });


        //    return result;
        //}

        void GenerateManyGrid(List<string> aList, int size)
        {
            int take = 1000;
            var result = new List<CrossWordGenerator>();
            var wordList = aList.OrderByDescending(d => d.Length).ToList();
            var testGrid = new CrossWordGrid(NumRow, NumCol);
            var startingPositions = testGrid.GetStartingCoordFor(wordList.First());

            foreach (var startingPosition in startingPositions.Take(take))
            {
                var generator = new CrossWordGenerator(size, size, wordList, startingPosition);

                result.Add(generator);
            }

            lbxGeneators.ItemsSource = result
                .OrderByDescending(g => g.FitWordList.Count)
                .ThenBy(g => g.Grid.BaryDistance);
        }

        public void DrawGrid(CrossWordGenerator generator)
        {
            SetMaze(generator.NumRow, generator.NumCol);
            var firstLetter = true;
            foreach (var crossWord in generator.FitWordList)
            {
                foreach (var letter in crossWord.WordLetterList)
                {
                    var cellCoord = letter.Coord;
                    var uiCell = UIMazeCellList[cellCoord.Row, cellCoord.Col];
                    uiCell.DrawLetter(letter, firstLetter);
                    firstLetter = false;
                }
            }
        }

        private void cmdGenFullGrid_Click(object sender, RoutedEventArgs e)
        {
            SetMaze(NumRow, NumCol);
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
                // var crossingIndexHoriz = Grid.SelectRandomAnchor(word, CrossWordDirection.Horizontal);
                // var crossingIndexVert = Grid.SelectRandomAnchor(word, CrossWordDirection.Vertical);
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
            SetMaze(NumRow, NumCol);
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
        public void SetMaze(int gridRow, int gridCol)
        {
            mainCanvas.Children.Clear();
            wordListCopy = wordList.ToList();
            NumRow = gridRow;
            NumCol = gridCol;

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

        private void cbsGridSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int size = (int)cbsGridSize.SelectedValue;

            NumRow = size;
            NumCol = size;
        }

        private void lbxFitWord_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void lbxDicoCrossWord_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var mot = lbxDicoCrossWord.SelectedItem as string;
            if (mot != null)
            {
                txtWordForGrid.Text = mot;
            }
        }

        private void cbxBranchLimit_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BranchLimit = cbxBranchLimit.SelectedIndex;
        }

        private void cbxDepthLimit_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DepthLimit = (int) cbxDepthLimit.SelectedValue;
        }
    }
}
