using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WiktionaireParser.Models
{
    class WordSearch2
    {
        public string Word { get; private set; }
        public List<string> Permutations { get; private set; }
        public List<string> AdjacentWords { get; private set; }
        public List<string> OnePositionWords { get; private set; }

        public WordSearch2(string word, HashSet<string> dictionary)
        {
            Word = word;
            Permutations = GeneratePermutations(word, dictionary);
            AdjacentWords = GenerateAdjacentWords(word, dictionary);
            OnePositionWords = GenerateOnePositionWords(word, dictionary);
        }

        /// <summary>
        /// Generates all possible permutations of the input word using a recursive algorithm.
        /// </summary>
        private List<string> GeneratePermutations(string word, HashSet<string> dictionary)
        {
            List<string> permutations = new List<string>();

            GeneratePermutationsHelper(word.ToCharArray(), 0, permutations, dictionary);

            return permutations;
        }

        /// <summary>
        /// Helper method that generates all permutations of the input word.
        /// </summary>
        private void GeneratePermutationsHelper(char[] word, int index, List<string> permutations, HashSet<string> dictionary)
        {
            if (index == word.Length - 1)
            {
                string newWord = new string(word);
                if (dictionary.Contains(newWord))
                {
                    permutations.Add(newWord);
                }
            }
            else
            {
                for (int i = index; i < word.Length; i++)
                {
                    char temp = word[i];
                    word[i] = word[index];
                    word[index] = temp;

                    GeneratePermutationsHelper(word, index + 1, permutations, dictionary);

                    temp = word[i];
                    word[i] = word[index];
                    word[index] = temp;
                }
            }
        }

        /// <summary>
        /// Generates all possible words that can be formed by changing one letter of the input word by a letter of the alphabet.
        /// </summary>
        private List<string> GenerateAdjacentWords(string word, HashSet<string> dictionary)
        {
            ConcurrentBag<string> adjacentWords = new ConcurrentBag<string>();

            // Parallel loop that generates adjacent words by changing one letter of the input word
            Parallel.ForEach(word, (c, state) =>
            {
                for (char newChar = 'a'; newChar <= 'z'; newChar++)
                {
                    if (c != newChar)
                    {
                        char[] letters = word.ToCharArray();
                        letters[word.IndexOf(c)] = newChar;
                        string newWord = new string(letters);

                        if (dictionary.Contains(newWord))
                        {
                            adjacentWords.Add(newWord);
                        }
                    }
                }
            });

            return adjacentWords.ToList();
        }


        /// <summary>
        /// Generates all possible words that can be formed by changing the position of one letter of the input word and that exist in a given dictionary.
        /// </summary>
        private List<string> GenerateOnePositionWords(string word, HashSet<string> dictionary)
        {
            ConcurrentBag<string> onePositionWords = new ConcurrentBag<string>();

            // Parallel loop that generates one-position words by changing the position of one letter of the input word and checking against a dictionary
            Parallel.ForEach(Enumerable.Range(0, word.Length), (i, state) =>
            {
                for (int j = i + 1; j < word.Length; j++)
                {
                    char[] letters = word.ToCharArray();
                    char temp = letters[i];
                    letters[i] = letters[j];
                    letters[j] = temp;
                    string newWord = new string(letters);

                    if (dictionary.Contains(newWord))
                    {
                        onePositionWords.Add(newWord);
                    }
                }
            });

            return onePositionWords.ToList();
        }
    }
}
