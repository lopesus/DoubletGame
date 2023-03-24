using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WiktionaireParser.Models.wordsearch
{
    public class ShiftUtils
    {
        /// <summary>
        /// Inserts a character into a string at the specified index.
        /// </summary>
        /// <param name="str">The input string to insert the character into.</param>
        /// <param name="index">The index at which to insert the character.</param>
        /// <param name="ch">The character to insert into the string.</param>
        /// <returns>A new string with the specified character inserted at the specified index.</returns>
        public static string InsertChar(string str, int index, char ch)
        {
            // Convert the input string to a char[] array
            char[] chars = str.ToCharArray();

            // Increase the size of the char[] array by 1
            Array.Resize(ref chars, chars.Length + 1);

            // Shift the characters after the insertion point to the right
            Array.Copy(chars, index, chars, index + 1, chars.Length - index - 1);

            // Insert the new character at the insertion point
            chars[index] = ch;

            // Convert the modified char[] array back to a string and return it
            return new string(chars);
        }

        /// <summary>
        /// Removes a character from a string at the specified index.
        /// </summary>
        /// <param name="str">The input string to remove the character from.</param>
        /// <param name="index">The index of the character to remove from the string.</param>
        /// <returns>A new string with the specified character removed at the specified index.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the index is outside the bounds of the input string.</exception>
        public static string RemoveChar(string str, int index)
        {
            // Check if the specified index is valid
            if (index < 0 || index >= str.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            // Handle the special cases where the index is at the beginning or end of the string
            if (index == 0)
            {
                return str.Substring(1);
            }
            else if (index == str.Length - 1)
            {
                return str.Substring(0, str.Length - 1);
            }
            else // General case
            {
                // Concatenate the left and right substrings around the character to be removed
                return str.Substring(0, index) + str.Substring(index + 1);
            }
        }

        public static string InsertChar2(string str, int index, char ch)
        {
            StringBuilder sb = new StringBuilder(str);
            sb.Insert(index, ch);
            return sb.ToString();
        }

        public static string RemoveChar2(string str, int index)
        {
            StringBuilder sb = new StringBuilder(str);
            sb.Remove(index, 1);
            return sb.ToString();
        }

        public static List<string> GenerateWordsByInsertion(string word)
        {
            HashSet<string> words = new HashSet<string>();

            // For each character in the word, remove it and generate words by inserting it into all possible positions in the remaining string
            for (int i = 0; i < word.Length; i++)
            {
                string current = word.Remove(i, 1); // Remove the i-th character
                for (int j = 0; j <= current.Length; j++)
                {
                    string newWord = current.Insert(j, word[i].ToString()); // Insert the i-th character at position j
                    words.Add(newWord);
                }
            }

            return words.ToList();
        }

        /// <summary>
        /// BUGGY
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public static List<string> GenerateShiftedWords(string word)
        {
            List<string> shiftedWords = new List<string>();

            for (int fromIndex = 0; fromIndex < word.Length; fromIndex++)
            {
                for (int toIndex = 0; toIndex < word.Length; toIndex++)
                {
                    if (fromIndex == toIndex)
                    {
                        continue; // Skip shifting to the same position
                    }

                    string shiftedWord = ShiftLetter(word, fromIndex, toIndex);

                    if (!shiftedWords.Contains(shiftedWord))
                    {
                        shiftedWords.Add(shiftedWord);
                    }
                }
            }

            return shiftedWords;
        }

        public static string ShiftLetter(string word, int fromIndex, int toIndex)
        {
            // Convert the input word to a character array to allow modification
            char[] wordArray = word.ToCharArray();

            // Store the letter to be shifted in a temporary variable
            char letterToShift = wordArray[fromIndex];

            // Shift the letter to the desired position
            for (int i = fromIndex; i < toIndex; i++)
            {
                wordArray[i] = wordArray[i + 1];
            }
            wordArray[toIndex] = letterToShift;

            // Convert the modified character array back to a string and return it
            return new string(wordArray);
        }
    }
}
