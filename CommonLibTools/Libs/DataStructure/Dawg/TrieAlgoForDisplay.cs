using System.Collections.Generic;
using CommonLibTools.Libs.Extensions;

namespace CommonLibTools.Libs.DataStructure.Dawg
{
    public static class TrieAlgoForDisplay
    {
        public static void AddToResult(string mot, char letter, bool letterIsJoker, Range range, ref Dictionary<int, List<string>> result, DisplayOptions options
            , string mustContainCar)
        {
            var ajout = letter.ToString();
            if (mustContainCar.IsNotNullOrEmptyString())
            {
                if (ajout == mustContainCar)
                {
                    ajout = "+" + letter + "+";
                }
            }

            if (letterIsJoker)
            {
                if (options.ShowJoker)
                {
                    mot += "*" + ajout + "*";
                }
                else
                {
                    mot += ajout;
                }
            }
            else
            {
                mot += ajout;
            }

            if (mustContainCar.IsNotNullOrEmptyString())
            {
                if (mot.Contains(mustContainCar))
                {
                    var wordLength = mot.LengthWithoutDelimiter();
                    if (range.IsInRange(wordLength))
                    {
                        if (result.ContainsKey(wordLength) == false)
                        {
                            result[wordLength] = new List<string>();
                        }
                        result[wordLength].Add(options.ToUppercase ? mot.ToUpperInvariant() : mot);
                    }
                }
            }
            else
            {
                var wordLength = mot.LengthWithoutDelimiter();
                if (range.IsInRange(wordLength))
                {
                    if (result.ContainsKey(wordLength) == false)
                    {
                        result[wordLength] = new List<string>();
                    }
                    result[wordLength].Add(options.ToUppercase ? mot.ToUpperInvariant() : mot);
                }
            }


        }


        public static void AddToResult(string mot, char letter, bool letterIsJoker, Range range, ref  List<string> result, DisplayOptions options
           , string mustContainCar)
        {
            var ajout = letter.ToString();
            if (mustContainCar.IsNotNullOrEmptyString())
            {
                if (ajout == mustContainCar)
                {
                    ajout = "+" + letter + "+";
                }
            }

            if (letterIsJoker)
            {
                if (options.ShowJoker)
                {
                    mot += "*" + ajout + "*";
                }
                else
                {
                    mot += ajout;
                }
            }
            else
            {
                mot += ajout;
            }

            if (mustContainCar.IsNotNullOrEmptyString())
            {
                if (mot.Contains(mustContainCar))
                {
                    var wordLength = mot.LengthWithoutDelimiter();
                    if (range.IsInRange(wordLength))
                    {
                        result.Add(options.ToUppercase ? mot.ToUpperInvariant() : mot);

                    }
                }
            }
            else
            {
                var wordLength = mot.LengthWithoutDelimiter();
                if (range.IsInRange(wordLength))
                {
                    result.Add(options.ToUppercase ? mot.ToUpperInvariant() : mot);
                }
            }


        }

        public static string SetMot(char letter, bool letterIsJoker, string mot, DisplayOptions options, string mustContainCar = null)
        {

            var ajout = letter.ToString();
            if (mustContainCar.IsNotNullOrEmptyString())
            {
                if (ajout == mustContainCar)
                {
                    ajout = "+" + letter + "+";
                }
            }

            if (letterIsJoker)
            {
                if (options.ShowJoker)
                {
                    mot += "*" + ajout + "*";
                }
                else
                {
                    mot += ajout;
                }
            }
            else
            {
                mot += ajout;
            }
            return mot;
        }



    }
}