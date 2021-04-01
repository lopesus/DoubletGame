using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibTools.Libs.Extensions;
using Newtonsoft.Json;

namespace CommonLibTools.Libs.CrossWord
{
    public class CrossWordLevel
    {
        [JsonProperty(PropertyName = "R")]
        public int Row { get; set; }


        [JsonProperty(PropertyName = "C")]
        public int Col { get; set; }

        [JsonProperty(PropertyName = "L")]
        public string Letter { get; set; }

        [JsonProperty(PropertyName = "W")]
        public List<CrossWordSimple> WordList { get; set; }

        [JsonProperty(PropertyName = "T")]
        public float Time { get; set; }

        [JsonProperty(PropertyName = "M")]
        public int Move { get; set; }

        [JsonProperty(PropertyName = "G")]
        public GameMode GameMode { get; set; }



        public List<string> AllPossibleWord
        {
            get => allPossibleWord;
            set => allPossibleWord = value;
        }

        /// <summary>
        /// valid word but not present in the level
        /// </summary>
        public List<string> ExtraWordList
        {
            get => extraWordList;
            set => extraWordList = value;
        }

        public List<CrossWordSimple> FoundWord
        {
            get => foundWord;
            set => foundWord = value;
        }

        public List<CrossWordSimple> RemainingWordToFound
        {
            get => remainingWordToFound;
            set => remainingWordToFound = value;
        }

        private List<string> allPossibleWord;
        private List<string> extraWordList;
        private List<CrossWordSimple> foundWord;
        private List<CrossWordSimple> remainingWordToFound;


        public CrossWordLevel()
        {
            AllPossibleWord = new List<string>();
            ExtraWordList = new List<string>();
            FoundWord = new List<CrossWordSimple>();
            remainingWordToFound = new List<CrossWordSimple>();
        }

        public void Reset()
        {
            ExtraWordList = new List<string>();
            FoundWord = new List<CrossWordSimple>();
            RemainingWordToFound = new List<CrossWordSimple>(WordList);
        }

        public CrossWordLevel(int row, int col, string levelLeters, List<CrossWord> allWords)
        {
            Row = row;
            Col = col;
            Letter = levelLeters;
            WordList = new List<CrossWordSimple>();
            foreach (CrossWord crossWord in allWords)
            {
                CrossWordSimple crossWordSimple = new CrossWordSimple()
                {
                    Word = crossWord.Word,
                    Coord = new CoordSimple(crossWord.Coord),
                    Direction = crossWord.Direction,
                };
                WordList.Add(crossWordSimple);
            }
        }

        public CrossWordLevel(GenGrid genGrid, int currentLevel)
        {
            AllPossibleWord = new List<string>();
            ExtraWordList = new List<string>();
            FoundWord = new List<CrossWordSimple>();

            Row = genGrid.NumRow;
            Col = genGrid.NumCol;
            Letter = genGrid.Letters;
            WordList = new List<CrossWordSimple>();

            foreach (CrossWord crossWord in genGrid.FitWordList)
            {
                CrossWordSimple crossWordSimple = new CrossWordSimple()
                {
                    Word = crossWord.Word,
                    Coord = new CoordSimple(crossWord.Coord),
                    Direction = crossWord.Direction,
                };
                WordList.Add(crossWordSimple);
            }
        }

        public CrossWordSimple IsValidForLevel(string word)
        {
            var found = WordList.FirstOrDefault(w => w.Word == word);
            return found;
        }

        public void SetAllPossibleWords(List<string> allWords)
        {
            AllPossibleWord = allWords;

        }
        public void AddFoundWord(CrossWordSimple word)
        {
            if (word != null)
            {
                FoundWord.Add(word);

                var found = RemainingWordToFound.FirstOrDefault(w => w.Word == word.Word);
                RemainingWordToFound.Remove(found);
            }
        }

        /// <summary>
        /// add a word to the list of extra word
        /// return true if the word is added for the first time
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public bool AddExtraWord(string word)
        {
            if (word == null) return false;
            if (ExtraWordList.Contains(word))
            {
                return false;
            }
            else
            {
                ExtraWordList.Add(word);
                return true;
            }

        }

        public CrossWordSimple GetNotFoundWord()
        {
            var word = RemainingWordToFound.PickRandom();
            return word;
        }

        public bool IsCompleted()
        {
            return foundWord.Count == WordList.Count;
        }

        public bool IsFound(string word)
        {
            return FoundWord.FirstOrDefault(w => w.Word == word) != null;
        }

        //public override string ToString()
        //{
        //    var builder=new StringBuilder();
        //    foreach (var word in AllWords)
        //    {
        //        builder.Append($"{word}:");
        //    }
        //    return builder.ToString();
        //}
    }
}