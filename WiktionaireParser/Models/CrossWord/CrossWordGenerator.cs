using System.Collections.Generic;
using System.Linq;
using System.Windows;
using CommonLibTools;
using CommonLibTools.Extensions;
using PathFindingModel;

namespace WiktionaireParser.Models.CrossWord
{
    public class CrossWordGenerator
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
        public CrossWordGenerator(int numRow, int numCol, List<string> wordList, StartingPosition startingPosition)
        {
            NumRow = numRow;
            NumCol = numCol;
            Position = startingPosition;
            WordList = wordList.OrderByDescending(d => d.Length).ToList();
            wordListCopy = WordList.ToList();
            Grid = new CrossWordGrid(NumRow, NumCol);
            FitWordList = new List<CrossWord>();

            rejected=new Dictionary<string, int>();
            GenCrossword();
            FitScore = FitWordList.Count / (float)wordList.Count;
        }

        void GenCrossword()
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
            return $"{FitWordList.Count} - {WordList.Count}";
        }
    }
}