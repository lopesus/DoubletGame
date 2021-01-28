using System.Collections.Generic;
using System.Text;
using CommonLibTools.Extensions;

namespace CommonLibTools.DataStructure.Dawg.Algo
{
    public static class FindAllWordFollowingPatternAlgo
    {
        public static List<string> FindAllWordFollowingPattern(string pattern, string tirage, TrieNode root,
            bool limitToTirage, Range range)
        {
            if (range == null)
            {
                range = new Range(2, 17);
            }
            var result = new List<string>();

            if (limitToTirage)
            {
                var letter = pattern[0];
                var restePattern = pattern.RemoveChar(letter);
                if (letter.IsScrabbleLetter())
                {
                    FindAllWordFollowingPatternWorker(letter, new StringBuilder(pattern.Length), pattern.Length, restePattern, tirage, root, ref result, false, limitToTirage, range);
                }
                else
                {
                    var alphabet = tirage;
                    if (tirage.ContainsJoker())
                    {
                        alphabet = TrieUtils.Alphabet;
                    }
                    foreach (char car in alphabet)
                    {
                        var resteTirage = tirage.RemoveCharOrJoker(car);
                        FindAllWordFollowingPatternWorker(car, new StringBuilder(pattern.Length), pattern.Length, restePattern, resteTirage, root, ref result, true, limitToTirage, range);
                    }
                }
            }
            else
            {
                #region MyRegion
                //limitToTirage false
                var letter = pattern[0];
                var restePattern = pattern.RemoveChar(letter);
                var isJokerChar = letter.IsJoker();
                if (letter.IsScrabbleLetter())
                {
                    FindAllWordFollowingPatternWorker(letter, new StringBuilder(pattern.Length), pattern.Length, restePattern, tirage, root, ref result, false, limitToTirage, range);
                }
                else
                {
                    foreach (char car in TrieUtils.Alphabet)
                    {
                        FindAllWordFollowingPatternWorker(car, new StringBuilder(pattern.Length), pattern.Length, restePattern, tirage, root, ref result, isJokerChar, limitToTirage, range);
                    }
                }

                #endregion
            }
            return result;
        }

        private static void FindAllWordFollowingPatternWorker(char letter, StringBuilder motactuel, int lenPattern, string restePattern, string resteTirage, TrieNode node, ref List<string> result, bool isjoker, bool limitToTirage, Range range)
        {
            if (limitToTirage == false)
            {
                #region MyRegion
                if (restePattern.IsNullOrEmptyString())
                {
                    if (letter.IsScrabbleLetter())
                    {
                        #region MyRegion
                        if (node.Contains(letter))
                        {
                            node = node.GetChild(letter);
                            if (node.IsEnd)
                            {
                                TrieAlgoForDisplay.AddToResult(motactuel.ToString(), letter, isjoker, range, ref result, new DisplayOptions(), "");
                            }
                        }
                        #endregion
                    }
                }
                else // restepattern non vide
                {
                    if (letter.IsScrabbleLetter())
                    {
                        #region MyRegion
                        if (node.Contains(letter))
                        {
                            if (isjoker)
                            {
                                motactuel.Append("*" + letter + "*");
                            }
                            else
                            {
                                motactuel.Append(letter);
                            }

                            node = node.GetChild(letter);

                            letter = restePattern[0];
                            restePattern = restePattern.RemoveChar(letter);

                            if (letter.IsScrabbleLetter())
                            {
                                FindAllWordFollowingPatternAlgo.FindAllWordFollowingPatternWorker(letter, new StringBuilder(motactuel.ToString(), lenPattern), lenPattern, restePattern, resteTirage, node, ref result, false, limitToTirage, range);
                            }
                            else
                            {
                                foreach (char car in TrieUtils.Alphabet)
                                {
                                    FindAllWordFollowingPatternAlgo.FindAllWordFollowingPatternWorker(car, new StringBuilder(motactuel.ToString(), lenPattern), lenPattern, restePattern, resteTirage, node, ref result, true, limitToTirage, range);
                                }
                            }
                        }
                        #endregion
                    }

                }
                #endregion
            }
            else
            {
                //limitToTirage true
                #region MyRegion
                if (restePattern.IsNullOrEmptyString())
                {
                    #region MyRegion
                    if (letter.IsScrabbleLetter())
                    {
                        if (node.Contains(letter))
                        {
                            node = node.GetChild(letter);
                            if (node.IsEnd)
                            {
                                TrieAlgoForDisplay.AddToResult(motactuel.ToString(), letter, isjoker, range, ref result, new DisplayOptions(), "");
                            }
                        }
                    }
                    #endregion
                }
                else // restepattern non vide
                {
                    if (letter.IsScrabbleLetter())
                    {
                        #region MyRegion
                        if (node.Contains(letter))
                        {
                            if (isjoker)
                            {
                                motactuel.Append("*" + letter + "*");
                            }
                            else
                            {
                                motactuel.Append(letter);
                            }

                            node = node.GetChild(letter);

                            letter = restePattern[0];
                            restePattern = restePattern.RemoveChar(letter);

                            if (letter.IsScrabbleLetter())
                            {
                                FindAllWordFollowingPatternAlgo.FindAllWordFollowingPatternWorker(letter, new StringBuilder(motactuel.ToString(), lenPattern), lenPattern, restePattern, resteTirage, node, ref result, false, limitToTirage, range);
                            }
                            else
                            {
                                var alphabet = resteTirage;
                                if (resteTirage.ContainsJoker())
                                {
                                    alphabet = TrieUtils.Alphabet;
                                }
                                foreach (char car in alphabet)
                                {
                                    var resteTirage2 = resteTirage.RemoveCharOrJoker(car);
                                    FindAllWordFollowingPatternAlgo.FindAllWordFollowingPatternWorker(car, new StringBuilder(motactuel.ToString(), lenPattern), lenPattern, restePattern, resteTirage2, node, ref result, true, limitToTirage, range);
                                }
                            }
                        }
                        #endregion
                    }

                }
                #endregion
            }
        }
    }
}