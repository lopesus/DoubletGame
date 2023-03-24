using System.Collections.Generic;
using System.Linq;

static class WordSearchUtils
{
    /// <summary>
    /// Generates all possible permutations of the input word using a recursive algorithm.
    /// </summary>
    public static List<string> GeneratePermutations(string word, HashSet<string> dictionary)
    {
        List<string> permutations = new List<string>();

        GeneratePermutationsHelper(word.ToCharArray(), 0, permutations, dictionary);

        return permutations;
    }

    /// <summary>
    /// Helper method that generates all permutations of the input word.
    /// </summary>
    private static void GeneratePermutationsHelper(char[] word, int index, List<string> permutations, HashSet<string> dictionary)
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
    public static List<string> GenerateAdjacentWords(string word, HashSet<string> dictionary)
    {
        List<string> adjacentWords = new List<string>();

        foreach (char c in word)
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
        }

        return adjacentWords;
    }

    /// <summary>
    /// Generates all possible words that can be formed by changing the position of one letter of the input word and that exist in a given dictionary.
    /// </summary>
    public static List<string> GenerateOnePositionWords(string word, HashSet<string> dictionary)
    {
        List<string> onePositionWords = new List<string>();

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
                    onePositionWords.Add(newWord);
                }
            }
        }

        return onePositionWords;
    }
}
