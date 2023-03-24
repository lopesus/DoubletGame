using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace WiktionaireParser.Models
{
    public class WordGenerator
    {
        private HashSet<string> dictionary;

        public WordGenerator(string dictionaryPath)
        {
            dictionary = LoadDictionary(dictionaryPath);
        }

        public List<string> GenerateValidWords(string word)
        {
            List<string> validWords = new List<string>();

            Parallel.For(0, word.Length, i =>
            {
                List<string> localWords = new List<string>();

                for (char c = 'a'; c <= 'z'; c++)
                {
                    if (c != word[i])
                    {
                        string newWord = word.Substring(0, i) + c + word.Substring(i + 1);
                        if (dictionary.Contains(newWord))
                        {
                            localWords.Add(newWord);
                        }
                    }
                }

                lock (validWords)
                {
                    validWords.AddRange(localWords);
                }
            });

            return validWords;
        }

        public List<string> GeneratePermutations(string word)
        {
            List<string> permutations = new List<string>();

            GeneratePermutationsHelper(word.ToCharArray(), 0, permutations);

            return permutations;
        }

        private void GeneratePermutationsHelper(char[] word, int index, List<string> permutations)
        {
            if (index == word.Length - 1)
            {
                string perm = new string(word);
                if (dictionary.Contains(perm))
                {
                    permutations.Add(perm);
                }
            }
            else
            {
                for (int i = index; i < word.Length; i++)
                {
                    char temp = word[i];
                    word[i] = word[index];
                    word[index] = temp;

                    GeneratePermutationsHelper(word, index + 1, permutations);

                    temp = word[i];
                    word[i] = word[index];
                    word[index] = temp;
                }
            }
        }

        private HashSet<string> LoadDictionary(string filePath)
        {
            HashSet<string> dictionary = new HashSet<string>();
            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    dictionary.Add(line);
                }
            }
            return dictionary;
        }


        public List<string> GenerateOneLetterWords(string word)
        {
            List<string> words = new List<string>();

            for (int i = 0; i < word.Length; i++)
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
                        words.Add(newWord);
                    }
                }
            }

            return words;
        }

        public List<string> GenerateOneLetterWordsParallel(string word)
        {
            List<string> words = new List<string>();

            Parallel.For(0, word.Length, i =>
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
                        lock (words)
                        {
                            words.Add(newWord);
                        }
                    }
                }
            });

            return words;
        }

    }
}
