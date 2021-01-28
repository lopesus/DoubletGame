using System.Collections.Generic;
using System.Linq;
using CommonLibTools.Extensions;

namespace CommonLibTools.DataStructure.Dawg
{
    public static class TrieAlgo
    {
        public static List<string> RallongeFinMot(string mot, string tirage, Trie trie, bool useTirage, Range range)
        {

            var re = new List<string>();
            if (mot.IsNotNullOrEmptyString())
            {
                var allPossibleFixedPosWord = TrieUtils.AllPossibleFixedPosWord(mot + ".", tirage, trie.GetRoot(), useTirage, range);
                return allPossibleFixedPosWord;
            }
            return re;

            //HashSet<char> set=new HashSet<char>();
            //var re = new List<string>();
            if (!string.IsNullOrEmpty(mot))
            {
                var node = trie.GetLastNode(mot);
                if (node.ChildNodes == null)
                {
                    return re;
                }
                else
                {
                    if (useTirage)
                    {
                        if (tirage.ContainsJoker())
                        {
                            foreach (char car in node.ChildNodes.Keys)
                            {
                                if (node.GetChild(car).IsEnd)
                                {
                                    if (tirage.Contains(car.ToString()))
                                    {
                                        re.Add(mot + "+" + car + "+");
                                    }
                                    else
                                    {
                                        re.Add(mot + "*" + car + "*");
                                    }

                                }
                            }
                        }
                        else
                        {
                            foreach (char car in node.ChildNodes.Keys)
                            {
                                if (tirage.Contains(car.ToString()))
                                {
                                    if (node.GetChild(car).IsEnd)
                                    {
                                        re.Add(mot + "+" + car + "+");
                                    }
                                }

                            }
                        }

                        return re;
                    }
                    else
                    {
                        foreach (char car in TrieUtils.Alphabet)
                        {
                            if (node.ChildNodes.ContainsKey(car))
                            {
                                if (node.GetChild(car).IsEnd)
                                {
                                    re.Add(mot + "+" + car + "+");
                                }
                            }


                        }
                    }


                }
            }
            return re;


        }

        public static List<char> LettresRallongeFinMot(string mot, string tirage, Trie trie, bool useTirage)
        {
            //HashSet<char> set=new HashSet<char>();
            var re = new List<char>();
            if (!string.IsNullOrEmpty(mot))
            {
                var node = trie.GetLastNode(mot);
                if (node.ChildNodes == null)
                {
                    return re;
                }
                else
                {
                    if (useTirage)
                    {
                        if (tirage.ContainsJoker())
                        {
                            foreach (char car in node.ChildNodes.Keys)
                            {
                                if (node.GetChild(car).IsEnd)
                                {
                                    if (tirage.Contains(car.ToString()))
                                    {
                                        re.Add(car);
                                    }
                                    else
                                    {
                                        re.Add(car);
                                    }

                                }
                            }
                        }
                        else
                        {
                            foreach (char car in node.ChildNodes.Keys)
                            {
                                if (tirage.Contains(car.ToString()))
                                {
                                    if (node.GetChild(car).IsEnd)
                                    {
                                        re.Add(car);
                                    }
                                }

                            }
                        }

                        return re;
                    }
                    else
                    {
                        foreach (char car in TrieUtils.Alphabet)
                        {
                            if (node.ChildNodes.ContainsKey(car))
                            {
                                if (node.GetChild(car).IsEnd)
                                {
                                    re.Add(car);
                                }
                            }


                        }
                    }


                }
            }
            return re;


        }

        public static Dictionary<int, List<string>> AllRallongeFinMot(string mot, string tirage, Trie trie, bool useTirage, Range range)
        {
            range.CheckRangeValues();

            var result = new Dictionary<int, List<string>>();
            var uneLettre = RallongeFinMot(mot, tirage, trie, useTirage, range);

            var lettreRallonge = RallongeFinMot(mot, tirage, trie, useTirage, new Range(2, 50));
            List<char> lettres = lettreRallonge.Select(m =>
            {
                var temp = m.RemoveAllMarks().ToLower();
                return temp[temp.Length - 1];
            }).Distinct().ToList();


            if (useTirage)
            {
                var possibleWithTirage = TrieUtils.FindAllPossibleWord(tirage, trie.GetRoot(), showJoker: true, range: range);
                foreach (KeyValuePair<int, List<string>> pair in possibleWithTirage)
                {
                    List<string> value = pair.Value;
                    foreach (var possibleMot in value)
                    {
                        if (possibleMot.ContainsAnyLetter(lettres))
                        {
                            var wordLength = possibleMot.LengthWithoutDelimiter();
                            if (result.ContainsKey(wordLength) == false)
                            {
                                result[wordLength] = new List<string>();
                            }
                            string showCrossingLetter = possibleMot.ShowCrossingLetter(lettres);
                            result[wordLength].Add(showCrossingLetter.ToUpperInvariant());
                        }
                    }
                }
            }

            //ajout resultat une lettre
            foreach (string temp in uneLettre)
            {
                var wordLength = temp.LengthWithoutDelimiter();
                if (result.ContainsKey(wordLength) == false)
                {
                    result[wordLength] = new List<string>();
                }
                result[wordLength].Add(temp);
            }
            return result;
        }

        public static List<string> RallongeDebutMot(string mot, string tirage, Trie trie, bool useTirage, Range range, bool includeMot)
        {
            var re = new List<string>();
            if (mot.IsNotNullOrEmptyString())
            {
                var allPossibleFixedPosWord = TrieUtils.AllPossibleFixedPosWord("." + mot, tirage, trie.GetRoot(), useTirage, range);
                return allPossibleFixedPosWord;
            }
            return re;
        }
        public static IDictionary<int, List<string>> AllRallongeDebutMot(string mot, string tirage, Trie trie, bool useTirage, Range range, bool includeMot)
        {
            range.CheckRangeValues();
            mot = mot.RemoveAllJokerAndNonAlpha();

            if (includeMot)
            {
                //tirage += mot;
            }

            var result = new Dictionary<int, List<string>>();
            var uneLettre = RallongeDebutMot(mot, tirage, trie, useTirage, range, includeMot);

            var lettreRallonge = RallongeDebutMot(mot, tirage, trie, useTirage, new Range(2, 50), includeMot);
            List<char> lettres = lettreRallonge.Select(m =>
            {
                var temp = m.RemoveAllMarks().ToLower();
                return temp[0];
            }).Distinct().ToList();

            if (useTirage)
            {
                var possibleWithTirage = TrieUtils.FindAllPossibleWord(tirage, trie.GetRoot(), showJoker: true, range: range);
                foreach (KeyValuePair<int, List<string>> pair in possibleWithTirage)
                {
                    List<string> value = pair.Value;
                    foreach (var possibleMot in value)
                    {
                        if (possibleMot.ContainsAnyLetter(lettres))
                        {
                            var wordLength = possibleMot.LengthWithoutDelimiter();
                            if (result.ContainsKey(wordLength) == false)
                            {
                                result[wordLength] = new List<string>();
                            }
                            string showCrossingLetter = possibleMot.ShowCrossingLetter(lettres);
                            result[wordLength].Add(showCrossingLetter.ToUpperInvariant());
                        }
                    }
                }
            }

            //ajout resultat une lettre
            foreach (string temp in uneLettre)
            {
                var wordLength = temp.LengthWithoutDelimiter();
                if (result.ContainsKey(wordLength) == false)
                {
                    result[wordLength] = new List<string>();
                }
                result[wordLength].Add(temp);
            }
            return result;
        }

        public static IDictionary<int, List<string>> AllRallongeMot(string mot, string tirage, Trie trie, bool useTirage, Range range, bool includeMot)
        {
            var result = new Dictionary<int, List<string>>();
            if (useTirage)
            {
                if (mot.IsNotNullOrEmptyString())
                {
                    foreach (char car in mot.SansDoubleChar())
                    {
                        var word = tirage + car;

                        //find all possible word
                        var sansDoubleChar = word.SansDoubleChar();
                        var dejaCherche = new HashSet<char>();
                        foreach (char letter in sansDoubleChar)
                        {
                            var restemot = StringExtensions.RemoveChar(word, letter);
                            TrieUtils.FindAllPossibleWordWorker(letter, false, "", restemot, trie.GetRoot(), range, ref result, true, true, new HashSet<char>(dejaCherche), car.ToString());
                            dejaCherche.Add(letter);
                        }

                    }


                }
            }

            //return result;

            var debut = AllRallongeDebutMot(mot, tirage, trie, useTirage, range, includeMot);
            foreach (KeyValuePair<int, List<string>> pair in debut)
            {
                var key = pair.Key;
                var value = pair.Value;
                if (result.ContainsKey(key))
                {
                    result[key].AddRange(value);
                }
                else
                {
                    result[key] = value;
                }
            }
            var fin = AllRallongeFinMot(mot, tirage, trie, useTirage, range);
            foreach (KeyValuePair<int, List<string>> pair in fin)
            {
                var key = pair.Key;
                var value = pair.Value;
                if (result.ContainsKey(key))
                {
                    result[key].AddRange(value);
                }
                else
                {
                    result[key] = value;
                }
            }

            //distinct
            IDictionary<int, List<string>> temp = new Dictionary<int, List<string>>();
            foreach (KeyValuePair<int, List<string>> pair in result)
            {
                List<string> value = pair.Value.OrderBy(v =>
                {
                    v = v.RemoveAllMarks();// v.Replace("*", "");
                    return v.ToLower();
                }).Distinct(new ScrabbleWordComparer()).ToList();

                temp.Add(pair.Key, value);
            }

            return temp;
        }

        public static Dictionary<int, List<string>> MotFinissantPar(string tirage, string fin, Trie trie, bool useTirage, Range range, bool includeComplementToTirage)
        {
            range.CheckRangeValues();
            fin = fin.RemoveAllJokerAndNonAlpha();
            var result = new Dictionary<int, List<string>>();
            if (useTirage)
            {
                Dictionary<int, List<string>> tempResult;
                if (includeComplementToTirage)
                {
                    tirage += fin.RemoveAllJokerAndNonAlpha();
                    tempResult = TrieUtils.FindAllPossibleWord(tirage, trie.GetRoot(), showJoker: true, range: range);
                    
                }
                else
                {
                     tempResult = TrieUtils.FindAllPossibleWord(tirage, trie.GetRoot(), showJoker: true, range: range);
                    
                }

                foreach (KeyValuePair<int, List<string>> pair in tempResult)
                {
                    var value = pair.Value;
                    var key = pair.Key;
                    foreach (string mot in value)
                    {
                        var tempMot = mot.RemoveAllMarks();//.EndsWith(fin);
                        if (tempMot.EndsWith(fin))
                        {
                            var length = tempMot.Length;
                            if (!result.ContainsKey(length))
                            {
                                result[key] = new List<string>();
                            }
                            result[length].Add(mot.ToUpperInvariant());
                        }
                    }
                }
                
            }
            else
            {
                var node = trie.GetRoot();
                DawgVisitor(node, "", fin, ContainsOptions.End, ref result, range);
            }

            return result;
        }

        public static Dictionary<int, List<string>> MotCommencantPar(string tirage, string debut, Trie trie, bool useTirage, Range range, bool includeComplementToTirage)
        {
            range.CheckRangeValues();
            debut = debut.RemoveAllJokerAndNonAlpha();
            var result = new Dictionary<int, List<string>>();
            
            if (useTirage)
            {
                var resteTirage = tirage;
                if (includeComplementToTirage)
                {
                    resteTirage += debut;
                }
                var node = trie.GetRoot();
                foreach (char car in debut)
                {
                    if (node.Contains(car) && resteTirage.ContainsCharOrJoker(car))
                    {
                        resteTirage = resteTirage.RemoveCharOrJoker(car);
                        node = node.GetChild(car);
                    }
                    else
                    {
                        return result;
                    }
                }

                if (includeComplementToTirage)
                {
                    result = TrieUtils.FindAllPossibleWord(tirage, node, showJoker: true, range: range, debutMot: debut);
                }
                else
                {
                    tirage = tirage.RemoveCharFromString(debut);
                    result = TrieUtils.FindAllPossibleWord(tirage, node, showJoker: true, range: range, debutMot: debut);
                }

            }
            else
            {
                var node = trie.GetRoot();
                DawgVisitor(node, "", debut, ContainsOptions.Begin, ref result, range);
            }

            return result;
        }




        private static void DawgVisitor(TrieNode node, string mot, string fin, ContainsOptions containsOptions, ref Dictionary<int, List<string>> result, Range range)
        {
            range.CheckRangeValues();

            if (node != null)
            {
                if (char.IsLetter(node.value))
                {
                    mot += node.value;
                }

                if (node.IsEnd)
                {
                    var length = mot.Length;
                    if (range.IsInRange(length))
                    {
                        #region MyRegion

                        switch (containsOptions)
                        {
                            case ContainsOptions.Begin:
                                {
                                    if (mot.StartsWith(fin))
                                    {

                                        if (!result.ContainsKey(length))
                                        {
                                            result[length] = new List<string>();
                                        }
                                        result[length].Add(mot);
                                    }
                                    break;
                                }
                            case ContainsOptions.Middle:
                                {
                                    if (mot.Contains(fin))
                                    {
                                        length = mot.Length;
                                        if (!result.ContainsKey(length))
                                        {
                                            result[length] = new List<string>();
                                        }
                                        result[length].Add(mot);
                                    }
                                    break;
                                }
                            case ContainsOptions.End:
                                {
                                    if (mot.EndsWith(fin))
                                    {
                                        length = mot.Length;
                                        if (!result.ContainsKey(length))
                                        {
                                            result[length] = new List<string>();
                                        }
                                        result[length].Add(mot);
                                    }
                                    break;
                                }
                        }
                        #endregion
                    }

                }

                if (node.ChildNodes != null)
                {
                    foreach (char car in node.ChildNodes.Keys)
                    {
                        DawgVisitor(node.GetChild(car), mot, fin, containsOptions, ref result, range);
                    }
                }

            }

        }

        public static Dictionary<int, List<string>> MotContenant(string tirage, string fin, Trie trie, bool useTirage, Range range, bool includeComplementToTirage)
        {
            range.CheckRangeValues();

            var result = new Dictionary<int, List<string>>();
            if (useTirage)
            {
                Dictionary<int, List<string>> tempResult;
                if (includeComplementToTirage)
                {
                    tirage += fin.RemoveAllJokerAndNonAlpha();
                    tempResult = TrieUtils.FindAllPossibleWord(tirage, trie.GetRoot(), showJoker: true, range: range);
                }
                else
                {
                     tempResult = TrieUtils.FindAllPossibleWord(tirage, trie.GetRoot(), showJoker: true, range: range);
                }

                foreach (KeyValuePair<int, List<string>> pair in tempResult)
                {
                    var value = pair.Value;
                    var key = pair.Key;
                    foreach (string mot in value)
                    {
                        var tempMot = mot.RemoveAllMarks();//.EndsWith(fin);
                        if (tempMot.Contains(fin))
                        {
                            var length = tempMot.Length;
                            if (!result.ContainsKey(length))
                            {
                                result[key] = new List<string>();
                            }
                            result[length].Add(mot.ToUpperInvariant());
                        }
                    }
                }
            }
            else
            {
                var node = trie.GetRoot();
                DawgVisitor(node, "", fin, ContainsOptions.Middle, ref result, range);
            }

            return result;
        }

    }
}
