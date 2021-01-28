using System;
using System.Collections.Generic;
using System.Text;
using CommonLibTools.Extensions;

namespace CommonLibTools.DataStructure.Dawg.Algo
{
    public static class zzzzzzzoldCode
    {
        public static List<string> FindAllWordFollowingPatternOld(string pattern, string tirage, TrieNode root, bool limitToTirage, bool useMyLetterToFillJokerOnly, Range range)
        {
            if (range == null)
            {
                range = new Range(2, 17);
            }
            var result = new List<string>();
            var letter = pattern[0];
            var restePattern = pattern.RemoveChar(letter);
            if (limitToTirage)
            {
                if (useMyLetterToFillJokerOnly)
                {

                }
                else
                {
                    if (pattern.Length <= tirage.Length)
                    {
                        letter = pattern[0];
                        if (char.IsLetter(letter))
                        {
                            char containedChar;
                            if (tirage.ContainsCharOrJoker(letter, out containedChar))
                            {
                                bool isJokerChar = containedChar.IsJoker();
                                var resteTirage = tirage.RemoveCharOrJoker(letter);
                                Traiter(letter, new StringBuilder(pattern.Length), pattern.Length, restePattern, resteTirage, root, ref result, isjoker: isJokerChar, range: range);
                            }

                        }
                        else if (letter.IsJoker())
                        {
                            foreach (char car in TrieUtils.Alphabet)
                            {
                                char containedChar;
                                if (tirage.ContainsCharOrJoker(car, out containedChar))
                                {
                                    bool isJokerChar = containedChar.IsJoker();
                                    var resteTirage = tirage.RemoveCharOrJoker(car);
                                    Traiter(car, new StringBuilder(pattern.Length), pattern.Length, restePattern, resteTirage, root, ref result, isjoker: isJokerChar, range: range);
                                }
                            }
                        }
                    }

                }
            }
            else
            {
                //check first letter for joker
                FindAllWordFollowingPatternWorkerOld(letter, new StringBuilder(pattern.Length), pattern.Length, restePattern, tirage, root, ref result, isjoker: false,
                    limitToTirage: limitToTirage, useMyLetterToFillJokerOnly: useMyLetterToFillJokerOnly, range: range);
            }

            return result;
        }

        private static void Traiter(char letter, StringBuilder motactuel, int lenPattern, string restePattern, string resteTirage, TrieNode node, ref List<string> result, bool isjoker, Range range)
        {
            if (restePattern.IsNullOrEmptyString()) //fin
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
            else
            {
                if (node.Contains(letter))
                {
                    node = node.GetChild(letter);
                    if (isjoker)
                    {
                        motactuel.Append("*" + letter + "*");
                    }
                    else
                    {
                        motactuel.Append(letter);
                    }

                    var nextLetter = restePattern[0];
                    restePattern = restePattern.RemoveChar(nextLetter);

                    if (char.IsLetter(nextLetter))
                    {
                        char containedChar;
                        if (resteTirage.ContainsCharOrJoker(nextLetter, out containedChar))
                        {
                            bool isJokerChar = containedChar.IsJoker();
                            resteTirage = resteTirage.RemoveCharOrJoker(letter);
                            Traiter(nextLetter, new StringBuilder(motactuel.ToString(), lenPattern), lenPattern, restePattern, resteTirage, node, ref result, isjoker: isJokerChar, range: range);
                        }
                    }
                    else if (nextLetter.IsJoker())
                    {
                        foreach (char car in TrieUtils.Alphabet)
                        {
                            char containedChar;
                            if (resteTirage.ContainsCharOrJoker(car, out containedChar))
                            {
                                bool isJokerChar = containedChar.IsJoker();
                                var resteTirage2 = resteTirage.RemoveCharOrJoker(car);
                                Traiter(car, new StringBuilder(motactuel.ToString(), lenPattern), lenPattern, restePattern, resteTirage2, node, ref result, isjoker: isJokerChar, range: range);
                            }
                        }
                    }
                }
            }
        }

        public static void FindAllWordFollowingPatternWorkerOld(char letter, StringBuilder motactuel, int lenPattern, string restePattern,
            string resteTirage, TrieNode node, ref List<string> result, bool isjoker, bool limitToTirage, bool useMyLetterToFillJokerOnly, Range range)
        {
            if (limitToTirage == false)
            {
                #region MyRegion
                if (restePattern.IsNullOrEmptyString())
                {
                    if (char.IsLetter(letter))
                    {
                        #region MyRegion
                        if (node.Contains(letter))
                        {
                            node = node.GetChild(letter);
                            if (node.IsEnd)
                            {
                                //motactuel.Append(letter);
                                TrieAlgoForDisplay.AddToResult(motactuel.ToString(), letter, isjoker, range, ref result, new DisplayOptions(), "");
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        #region MyRegion
                        if (letter.IsJoker())
                        {
                            foreach (char car in TrieUtils.Alphabet)
                            {
                                FindAllWordFollowingPatternWorkerOld(car, new StringBuilder(motactuel.ToString(), lenPattern), lenPattern, restePattern, resteTirage, node, ref result, true, limitToTirage, useMyLetterToFillJokerOnly, range);
                            }
                        }
                        #endregion
                    }
                }
                else // restepattern non vide
                {
                    if (char.IsLetter(letter))
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
                            //if (node.IsEnd)
                            //{
                            //    //AddToResult(motactuel.ToString(),letter,isjoker,range,ref result,new DisplayOptions(),"" );
                            //}

                            letter = restePattern[0];
                            restePattern = restePattern.RemoveChar(letter);
                            FindAllWordFollowingPatternWorkerOld(letter, new StringBuilder(motactuel.ToString(), lenPattern), lenPattern, restePattern, resteTirage, node, ref result, false, limitToTirage, useMyLetterToFillJokerOnly, range);
                        }
                        #endregion
                    }
                    else
                    {
                        if (letter.IsJoker())
                        {
                            foreach (char car in TrieUtils.Alphabet)
                            {
                                FindAllWordFollowingPatternWorkerOld(car, new StringBuilder(motactuel.ToString(), lenPattern), lenPattern, restePattern, resteTirage, node, ref result, true, limitToTirage, useMyLetterToFillJokerOnly, range);
                            }
                        }
                    }
                }
                #endregion
            }
            else //limitToTirage==true
            {
                if (useMyLetterToFillJokerOnly)
                {

                }
                else
                {
                    if (restePattern.Length > resteTirage.Length)
                    {
                        return;
                    }
                    else
                    {
                        if (restePattern.IsNullOrEmptyString()) //fin du pattern
                        {
                            if (char.IsLetter(letter))
                            {
                                if (node.Contains(letter))
                                {
                                    node = node.GetChild(letter);
                                    if (node.IsEnd)
                                    {
                                        TrieAlgoForDisplay.AddToResult(motactuel.ToString(), letter, isjoker, range, ref result, new DisplayOptions(), "");
                                    }
                                    //motactuel.Append(letter);
                                }
                            }
                            else
                            {
                                throw new Exception("char must be a letter non joker");

                            }
                        }
                        else //pas fin du pattern
                        {
                            if (char.IsLetter(letter))
                            {
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

                                    //take next letter
                                    letter = restePattern[0];
                                    restePattern = restePattern.RemoveChar(letter);

                                    //check and call
                                    if (char.IsLetter(letter))
                                    {
                                        char containedChar;
                                        if (resteTirage.ContainsCharOrJoker(letter, out containedChar))
                                        {
                                            bool isJokerChar = containedChar.IsJoker();
                                            resteTirage = resteTirage.RemoveCharOrJoker(letter);
                                            FindAllWordFollowingPatternWorkerOld(letter, new StringBuilder(motactuel.ToString(), lenPattern), lenPattern, restePattern, resteTirage, node, ref result, isjoker: isJokerChar,
                                                limitToTirage: limitToTirage, useMyLetterToFillJokerOnly: useMyLetterToFillJokerOnly, range: range);
                                        }
                                    }
                                    else if (letter.IsJoker())
                                    {
                                        string resteTiragefiltre = resteTirage;//.SansDoubleChar();
                                        foreach (char car in resteTiragefiltre)
                                        {
                                            char containedChar;
                                            if (resteTirage.ContainsCharOrJoker(car, out containedChar)) //always true
                                            {
                                                bool isJokerChar = containedChar.IsJoker();
                                                var resteTirage2 = resteTirage.RemoveCharOrJoker(car);
                                                FindAllWordFollowingPatternWorkerOld(car, new StringBuilder(motactuel.ToString(), lenPattern), lenPattern, restePattern, resteTirage2, node, ref result, isjoker: isJokerChar,
                                                    limitToTirage: limitToTirage, useMyLetterToFillJokerOnly: useMyLetterToFillJokerOnly, range: range);
                                            }
                                        }
                                    }
                                }


                            }
                            else if (letter.IsJoker())
                            {
                                throw new Exception("char must be a letter non joker");
                                //foreach (char car in resteTirage)
                                //{
                                //    if (resteTirage.ContainsCharOrJoker(car)) //always true
                                //    {
                                //        var resteTirage2 = resteTirage.RemoveCharOrJoker(car);
                                //        string motactuel2 = motactuel.ToString()+car;
                                //        FindAllWordFollowingPatternWorkerOld(car, new StringBuilder(motactuel2, lenPattern), lenPattern, restePattern, resteTirage2, node, ref result, isjoker: true,
                                //                        limitToTirage: limitToTirage, useMyLetterToFillJokerOnly: useMyLetterToFillJokerOnly, range: range);
                                //    }
                                //}

                            }

                        }
                    }
                }
            }


        }
    }
}