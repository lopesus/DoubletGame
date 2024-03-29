﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibTools.Libs.CrossWord
{
    public class GenGrid : CrossWordGenerator
    {
        //private static int branchLimit = 0;
        //public List<string> WordList { get; set; }
        //public int NumCol { get; set; }
        //public int NumRow { get; set; }
        //public StartingPosition Position { get; set; }
        //public CrossWordGrid Grid;
        //public List<CrossWord> FitWordList { get; set; }

        private List<string> wordListCopy = new List<string>();
        private Dictionary<string, int> rejected;

        //public float FitScore { get; set; }

        private Queue<string> Queue;
        public GenGrid(int numRow, int numCol, string levelLetters, List<string> wordList, StartingPosition startingPosition, ConcurrentBag<GenGrid> allGen, int branchLimit, int depthLimit, Dictionary<string, float> dicoFrequency)
        {
            allGen.Add(this);
            NumRow = numRow;
            NumCol = numCol;
            Letters = levelLetters;

            Position = startingPosition;
            WordList = wordList.OrderByDescending(d => d.Length).ToList();
            wordListCopy = WordList.ToList();

            WordCount = WordList.Count;

            Grid = new CrossWordGrid(NumRow, NumCol);
            FitWordList = new List<CrossWord>();
            FitWordDico = new Dictionary<string, int>();

            rejected = new Dictionary<string, int>();
            GenCrosswordSimple(allGen, branchLimit,depthLimit, dicoFrequency);
            FitScore = FitWordList.Count / (float)wordList.Count;

            Grid.GetGridBarycenter();
            Difficulty = SetDifficulty(dicoFrequency);
        }
        private float SetDifficulty(Dictionary<string, float> dicoFrequency)
        {
            float result = 1;
            if (dicoFrequency != null)
            {
                foreach (var word in FitWordList)
                {
                    float freq;
                    dicoFrequency.TryGetValue(word.Word, out freq);
                    result = result +  freq;
                }
            }

            return result / FitWordList.Count;
        }
        private GenGrid(int numRow, int numCol, string levelLetters, int count, List<CrossWord> fitWordList, string word, List<string> wordList, CrossingIndex index, ConcurrentBag<GenGrid> allGen, int branchLimit, int depthLimit, Dictionary<string, float> dicoFrequency)
        {
            allGen.Add(this);
            NumRow = numRow;
            NumCol = numCol;
            Letters = levelLetters;

            WordCount = count;
            Position = new StartingPosition(index.StartCoord, index.Direction);

            WordList = new List<string>(wordList); //wordList.OrderByDescending(d => d.Length).ToList();
            WordList.Insert(0, word);
            wordListCopy = WordList.ToList();
            Grid = new CrossWordGrid(NumRow, NumCol);
            FitWordList = new List<CrossWord>();
            FitWordDico = new Dictionary<string, int>();

            //fill the grid with already fitted words
            foreach (CrossWord crossWord in fitWordList)
            {
                PutWordAt(crossWord.Word, crossWord.Coord, crossWord.Direction);
            }

            rejected = new Dictionary<string, int>();
            GenCrosswordSimple(allGen, branchLimit,depthLimit, dicoFrequency);
            FitScore = FitWordList.Count / (float)WordCount;

            Grid.GetGridBarycenter();
            Difficulty = SetDifficulty(dicoFrequency);
        }


        void GenCrosswordSimple(ConcurrentBag<GenGrid> allGen, int branchLimit,int depthLimit, Dictionary<string, float> dicoFrequency)
        {
            string word = GetNextWord();
            if (FitWordDico.ContainsKey(word))
            {
                Console.WriteLine("error");
            }
            var tripleRejection = false;

            PutWordAt(word, Position.Coord, Position.Direction);
            word = GetNextWord();

            while (word != null && tripleRejection == false)
            {
                var startPosList = Grid.GetAllAnchor(word);

                if (startPosList.Count > 0)
                {
                    if (depthLimit>0)
                    {
                        //compute for other anchor minus the first one 
                        ParallelOptions options = new ParallelOptions()
                        {
                            MaxDegreeOfParallelism = 6
                        };
                        Parallel.ForEach(startPosList.Skip(1).Take(branchLimit), options, index =>
                        {
                            if (index != null)
                            {
                                new GenGrid(NumRow, NumCol, Letters, WordCount, FitWordList, word, wordListCopy, index, allGen, branchLimit,depthLimit-1, dicoFrequency);
                            }
                        });

                        //foreach (var index in startPosList.Skip(1).Take(10))
                        //{
                        //    if (index != null)
                        //    {
                        //        new GenGrid(NumRow, NumCol, WordCount, FitWordList, word, wordListCopy, index, allGen);
                        //    }
                        //}
                    }


                    // compute the first branch
                    //var crossingIndex = Grid.SelectRandomAnchor(word);
                    var crossingIndex = startPosList.First();
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

            if (FitWordDico.ContainsKey(word))
            {
                FitWordDico[word] += 1;
            }
            else
            {
                FitWordDico[word] = 1;
            }
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
            builder.AppendLine(Letters);
            builder.AppendLine($"{FitWordList.Count} - {WordCount}");
            builder.AppendLine($"Bary {Grid.BaryDistance}");
            builder.AppendLine($"Diff {Difficulty}");
            builder.AppendLine($"BaryRow {Grid.BaryRow} ");
            builder.AppendLine($"BaryCol {Grid.BaryCol} ");



            builder.AppendLine($"");
            return builder.ToString();
        }


        public static List<GenGrid> GenAll(string letters, List<string> listOfWords, int size, int branchLimit, int depthLimit, Dictionary<string, float> dicoFrequency)
        {
            int take = 10000;
            var result = new ConcurrentBag<GenGrid>();
            listOfWords = listOfWords.OrderByDescending(d => d.Length).ToList();
            var testGrid = new CrossWordGrid(size, size);
            var word = listOfWords.First().ToLowerInvariant();
            //listOfWords.Remove(word);
            var startingPositions = testGrid.GetStartingCoordFor(word);

            //foreach (var startingPosition in startingPositions)
            //{
            //    var generator = new GenGrid(size, size, letters, new List<string>(listOfWords), startingPosition, result,BranchLimit);
            //}

            ParallelOptions options = new ParallelOptions()
            {
                MaxDegreeOfParallelism = 6
            };
            Parallel.ForEach(startingPositions, options, startingPosition =>
            {
                var generator = new GenGrid(size, size, letters, new List<string>(listOfWords), startingPosition, result, branchLimit,depthLimit, dicoFrequency);
            });


            return result.OrderByDescending(g => g.FitWordList.Count)
                .ThenBy(g => g.Grid.BaryDistance).ToList(); ;
        }
    }
}