using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibTools.Libs.CrossWord
{
    public class GenGrid0
    {
        //public List<string> WordList { get; set; }
        public int NumCol { get; set; }
        public int NumRow { get; set; }
        // public StartingPosition Position { get; set; }
        public int WordCount { get; set; }
        public CrossWordGrid Grid;
        public List<CrossWord> FitWordList { get; set; }

        private List<string> wordList = new List<string>();
        private Dictionary<string, int> rejected;

        public float FitScore { get; set; }

        public GenGrid0()
        {

        }
        public GenGrid0(int numRow, int numCol, string word, List<string> remainingWords, Coord startCoord, CrossWordDirection direction, List<GenGrid0> allGen)
        {
            allGen.Add(this);
            NumRow = numRow;
            NumCol = numCol;
            wordList = remainingWords.OrderByDescending(d => d.Length).ToList();
            Grid = new CrossWordGrid(numRow, numCol);
            FitWordList = new List<CrossWord>();

            rejected = new Dictionary<string, int>();
            GenCrosswordAll(word, startCoord, direction, allGen);
            FitScore = FitWordList.Count / (float)remainingWords.Count;

            Grid.GetGridBarycenter();

        }
        //public GenGrid(string word, List<string> remainingWords, Coord startCoord, CrossWordDirection direction, GenGrid genGrid, List<GenGrid> allGen)
        //{
        //    allGen.Add(this);
        //    wordList = remainingWords.ToList();
        //    Grid = genGrid.Grid;

        //    rejected = new Dictionary<string, int>();
        //    GenCrosswordAll(word, startCoord, direction, allGen);
        //    FitScore = FitWordList.Count / (float)remainingWords.Count;

        //    Grid.GetGridBarycenter();

        //}

        private GenGrid0 Copy()
        {
            GenGrid0 genGrid = new GenGrid0();
            genGrid.Grid = Grid.Copy();
            genGrid.WordCount = WordCount;
            genGrid.FitWordList = FitWordList.ToList();

            return genGrid;
        }

        public void GenCrosswordAll(string word, Coord startCoord, CrossWordDirection direction, List<GenGrid0> allGen)
        {
            // string word = GetNextWord();
            var tripleRejection = false;

            PutWordAt(word, startCoord, direction);
            word = GetNextWord();

            while (word != null && tripleRejection == false)
            {
                var startPosList = Grid.GetAllAnchor(word);

                if (startPosList.Count == 0)
                {
                    if (rejected.ContainsKey(word) == false) rejected[word] = 0;
                    rejected[word] += 1;
                    if (rejected[word] >= 2)
                    {
                        tripleRejection = true;
                    }
                    //add to the st to try later 
                    wordList.Add(word);
                }
                else
                {
                    //var genGridCopy = this.Copy();
                    //process first
                    var first = startPosList.First();
                    PutWordAt(word, first.StartCoord, first.Direction);
                    rejected.Clear();

                    // create new generator for the rest
                    //foreach (var crossingInde in startPosList.Skip(1))
                    //{
                    //    //List<string> wordListCopy=wordList.ToList();
                    //    //new GenGrid(word,wordListCopy, first.StartCoord, first.Direction, genGridCopy, allGen);
                    //}
                }

                word = GetNextWord();
            }
        }




        void PutWordAt(string word, Coord coord, CrossWordDirection direction)
        {
            CrossWord crossWord = new CrossWord(word, coord, direction);
            Grid.PutWordAt(crossWord, coord, direction);
            FitWordList.Add(crossWord);
        }

        string GetNextWord()
        {
            // var word = wordListCopy.PickRandom();
            var word = wordList.FirstOrDefault();
            wordList.Remove(word);
            return word?.ToLowerInvariant();
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendLine($"{FitWordList.Count} - {WordCount}");
            builder.AppendLine($"Bary {Grid.BaryDistance}");
            builder.AppendLine($"BaryRow {Grid.BaryRow} ");
            builder.AppendLine($"BaryCol {Grid.BaryCol} ");



            builder.AppendLine($"");
            return builder.ToString();
        }
    }
}