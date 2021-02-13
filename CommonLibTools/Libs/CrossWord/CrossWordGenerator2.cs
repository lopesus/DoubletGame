using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibTools.Libs.CrossWord
{
    public class CrossWordGenerator2
    {
        public List<string> WordList { get; set; }
        public int NumCol { get; set; }
        public int NumRow { get; set; }
        public StartingPosition Position { get; set; }
        public CrossWordGrid Grid;
        public List<CrossWord> FitWordList { get; set; }

        private List<string> wordListCopy = new List<string>();
        private Dictionary<string, int> rejected;

        public float FitScore { get; set; }

        private Queue<string> Queue;
        public CrossWordGenerator2(int numRow, int numCol, List<string> wordList, StartingPosition startingPosition)
        {
            NumRow = numRow;
            NumCol = numCol;
            Position = startingPosition;
            WordList = wordList.OrderByDescending(d => d.Length).ToList();
            wordListCopy = WordList.ToList();
            Grid = new CrossWordGrid(NumRow, NumCol);
            FitWordList = new List<CrossWord>();

            rejected = new Dictionary<string, int>();
            GenCrosswordSimple();
            FitScore = FitWordList.Count / (float)wordList.Count;

            Grid.GetGridBarycenter();

        }


        void GenCrosswordSimple()
        {
            string word = GetNextWord();
            var tripleRejection = false;

            PutWordAt(word, Position.Coord, Position.Direction);
            word = GetNextWord();

            while (word != null && tripleRejection == false)
            {
                var crossingIndex = Grid.SelectRandomAnchor(word);
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
                    //add to the st to try later 
                    wordListCopy.Add(word);

                    //MessageBox.Show($"NO CROSSING for {word}".ToUpper());
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
            var word = wordListCopy.FirstOrDefault();
            wordListCopy.Remove(word);
            return word?.ToLowerInvariant();
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendLine($"{FitWordList.Count} - {WordList.Count}");
            builder.AppendLine($"Bary {Grid.BaryDistance}");
            builder.AppendLine($"BaryRow {Grid.BaryRow} ");
            builder.AppendLine($"BaryCol {Grid.BaryCol} ");



            builder.AppendLine($"");
            return builder.ToString();
        }
    }
}