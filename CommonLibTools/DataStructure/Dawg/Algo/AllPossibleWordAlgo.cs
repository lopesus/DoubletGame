using System.Collections.Generic;
using CommonLibTools.Extensions;

namespace CommonLibTools.DataStructure.Dawg.Algo
{
    public static class AllPossibleWordAlgo
    {
        public static Dictionary<int, List<string>> FindAllPossibleWord(string word, TrieNode node,
            DisplayOptions options, Range range, string mustContainCar)
        {
            var list = new Dictionary<int, List<string>>();
            if (range == null)
            {
                range = new Range(2, 17);
            }
            range.CheckRangeValues();
            var sansDoubleChar = word.SansDoubleChar();
            var dejaCherche = new HashSet<char>();
            foreach (char letter in sansDoubleChar)
            {
                var restemot = StringExtensions.RemoveChar(word, letter);
                FindAllPossibleWordWorker(letter, false, "", restemot, node, range, ref list, options, mustContainCar, new HashSet<char>(dejaCherche));
                dejaCherche.Add(letter);
            }
            return list;
        }

        public static void FindAllPossibleWordWorker(char letter, bool letterIsJoker, string mot, string resteMot,
            TrieNode rootNode, Range range, ref Dictionary<int, List<string>> result,
            DisplayOptions options, string mustContainCar, HashSet<char> dejaCherche)
        {

            if (resteMot.IsNullOrEmptyString())//dernier char du mot
            {
                if (letter.IsScrabbleLetter())
                {
                    #region MyRegion
                    if (rootNode.Contains(letter))
                    {
                        var childNode = rootNode.GetChildOrNull(letter);
                        if (childNode.IsEnd)
                        {
                            TrieAlgoForDisplay.AddToResult(mot, letter, letterIsJoker, range, ref result, options,mustContainCar);
                        }
                    }
                    #endregion
                }
                else
                {
                    if (letter.IsJoker())
                    {
                        foreach (char car in TrieUtils.Alphabet)
                        {
                            FindAllPossibleWordWorker(car, true, mot, resteMot, rootNode, range, ref result, options, mustContainCar, dejaCherche);
                        }
                    }
                }
            }
            else
            {
                if (letter.IsScrabbleLetter())
                {
                    #region MyRegion
                    if (rootNode.Contains(letter))
                    {
                        var childNode = rootNode.GetChildOrNull(letter);

                        if (childNode.IsEnd)
                        {
                            TrieAlgoForDisplay.AddToResult(mot, letter, letterIsJoker, range, ref result, options, mustContainCar: "");
                        }

                        mot = TrieAlgoForDisplay.SetMot(letter, letterIsJoker, mot, options, mustContainCar);

                        var dejaCherche2 = new HashSet<char>();
                        var sansDoubleChar = resteMot.SansDoubleChar();
                        foreach (char car in sansDoubleChar)
                        {
                            var restemot2 = StringExtensions.RemoveChar(resteMot, car);
                            FindAllPossibleWordWorker(car, false, mot, restemot2, childNode, range, ref result, options,
                                mustContainCar, new HashSet<char>(dejaCherche2));
                            //dejaCherche2.Add(car);
                        }

                    }
                    #endregion
                }
                else
                {
                    if (letter.IsJoker())
                    {
                        var dejaCherche2 = new HashSet<char>();
                        foreach (char car in TrieUtils.Alphabet)
                        {
                            if (dejaCherche != null)
                            {
                                if (dejaCherche.Contains(car))
                                {

                                }
                                else
                                {
                                    FindAllPossibleWordWorker(car, true, mot, resteMot, rootNode, range,
                                        ref result, options, mustContainCar, new HashSet<char>(dejaCherche2));
                                    //dejaCherche2.Add(car);
                                }
                            }
                            else
                            {
                                FindAllPossibleWordWorker(car, true, mot, resteMot, rootNode, range, ref result,
                                    options, mustContainCar, null);
                            }

                        }
                    }
                }
            }
        }
    }
}