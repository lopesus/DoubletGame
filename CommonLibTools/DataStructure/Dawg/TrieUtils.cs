using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using CommonLibTools.Extensions;

namespace CommonLibTools.DataStructure.Dawg
{
    public static class TrieUtils
    {
        //public const string Alphabet = "abcdefghijklmnopqrstuvwxyz";
        public const string Alphabet = "abcdefghijklmnopqrstuvwxyz!&#ñ";

        public static void AddScrabbleWordList(this List<string> source, List<string> toAdd)
        {
            foreach (var word in toAdd)
            {

            }
        }

        public static List<string> AllPossibleFixedPosWord(string tableau, string tirage, TrieNode root, bool useTirage, Range range = null)
        {
            //var limitToTirage = tirage.IsNotNullOrEmptyString();
            if (range == null)
            {
                range = new Range(2, 17);
            }
            var result = new List<string>();
            var letter = tableau[0];
            var resteTableau = tableau.RemoveChar(letter);
            AllPossibleFixedPosWordWorker(letter, new StringBuilder(), 0, resteTableau, tirage, root, ref result, false, useTirage: useTirage, range: range);
            return result;
        }

        public static void AllPossibleFixedPosWordWorker(char car, StringBuilder motactuel, int lenMotActuel, string resteTableau,
            string resteComplement, TrieNode node, ref List<string> result, bool isjoker = false, bool useTirage = false, Range range = null)
        {
            if (car.IsScrabbleLetter())
            {
                if (node.Contains(car))
                {
                    node = node.GetChild(car);
                    if (isjoker)
                    {
                        motactuel.Append("*" + car + "*");
                    }
                    else
                    {
                        motactuel.Append(car);
                    }
                    lenMotActuel++;

                    if (resteTableau == "")
                    {
                        if (node.IsEnd)
                        {
                            if (range != null && range.IsInRange(lenMotActuel))
                            {
                                result.Add(motactuel.ToString().ToUpperInvariant());
                            }
                        }
                    }
                    else
                    {
                        var letter = resteTableau[0];
                        var resteTirage2 = resteTableau.RemoveChar(letter);
                        AllPossibleFixedPosWordWorker(letter, new StringBuilder(motactuel.ToString()), lenMotActuel, resteTirage2, resteComplement, node, ref result, false, useTirage, range);
                    }
                }
            }
            else if (car.IsJoker())
            {
                if (useTirage)
                {

                    foreach (char letter in resteComplement)
                    {
                        if (letter.IsJoker())
                        {
                            var resteComplement2 = resteComplement.RemoveChar(letter);
                            foreach (char letter2 in Alphabet)
                            {
                                AllPossibleFixedPosWordWorker(letter2, new StringBuilder(motactuel.ToString()), lenMotActuel, resteTableau, resteComplement2, node, ref result, true, useTirage, range);
                            }
                        }
                        else
                        {
                            var resteComplement2 = resteComplement.RemoveChar(letter);
                            AllPossibleFixedPosWordWorker(letter, new StringBuilder(motactuel.ToString()), lenMotActuel, resteTableau, resteComplement2, node, ref result, true, useTirage, range);
                        }

                    }
                }
                else
                {
                    foreach (char letter in Alphabet)
                    {
                        AllPossibleFixedPosWordWorker(letter, new StringBuilder(motactuel.ToString()), lenMotActuel, resteTableau, resteComplement, node, ref result, true, useTirage, range);
                    }
                }

            }
        }
       
        
        public static List<string> FindExactAnagramsWithFixedPosition(string pattern, string tirage, TrieNode node)
        {
            //todo algo a paralleliser 
            var list = new List<string>();

            var car = pattern[0];
            bool fixedPos = car.IsScrabbleLetter();
            FindExactAnaWithFixedPositionWorker(car, fixedPos, "", pattern.RemoveChar(car), tirage, node, list, pattern.Length);

            return list;
        }

        private static void FindExactAnaWithFixedPositionWorker(char car, bool fixedPos, string motAccu, string restePattern, string tirage, TrieNode node, List<string> resultat, int longueurMot)
        {
            if (node != null)
            {
                if (motAccu.Length + 1 > longueurMot)
                {
                    return;
                }

                if (motAccu.Length + 1 == longueurMot)
                {
                    if (node.Contains(car))
                    {
                        motAccu += car;
                        node = node.GetChild(car);
                        if (node.IsEnd)
                        {
                            resultat.Add(motAccu);
                        }
                    }
                    else
                    {
                        if (IsJoker(car))
                        {
                            var motAccuTemp = motAccu;
                            var nodeTemp = node;
                            var lettres = Alphabet;
                            if (tirage.IsNotNullOrEmptyString())
                            {
                                lettres = tirage.SansDoubleChar();
                                if (tirage.ContainsJoker())
                                {
                                    lettres = Alphabet;
                                }
                            }
                            foreach (char carAlpha in lettres)
                            {
                                motAccu = motAccuTemp;
                                node = nodeTemp;
                                if (node.Contains(carAlpha))
                                {
                                    motAccu += carAlpha;
                                    node = node.GetChild(carAlpha);
                                    if (node.IsEnd)
                                    {
                                        resultat.Add(motAccu);
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (node.Contains(car))
                    {
                        motAccu += car;
                        node = node.GetChild(car);
                        if (restePattern.IsNotNullOrEmptyString())
                        {
                            var car2 = restePattern[0];
                            restePattern = restePattern.RemoveChar(car2);
                            fixedPos = false;
                            if (car2.IsScrabbleLetter())
                            {
                                fixedPos = true;
                            }
                            FindExactAnaWithFixedPositionWorker(car2, fixedPos, motAccu, restePattern, tirage, node, resultat, longueurMot);
                        }
                    }
                    else
                    {
                        if (fixedPos == false)
                        {
                            if (tirage.IsNotNullOrEmptyString())
                            {
                                if (tirage.ContainsJoker())
                                {
                                    var motAccuTemp = motAccu;
                                    var tirageTemp = tirage;
                                    var nodeTemp = node;
                                    var restePatternTemp = restePattern;

                                    foreach (char lettre in Alphabet)
                                    {
                                        motAccu = motAccuTemp;
                                        node = nodeTemp;
                                        restePattern = restePatternTemp;
                                        if (node.Contains(lettre))
                                        {
                                            motAccu += lettre;
                                            node = node.GetChild(lettre);
                                            if (tirageTemp.Contains(lettre.ToString()))
                                            {
                                                tirage = tirageTemp.RemoveChar(lettre);
                                            }
                                            else
                                            {
                                                tirage = tirageTemp.RemoveJoker();
                                            }

                                            if (restePattern.IsNotNullOrEmptyString())
                                            {
                                                var car2 = restePattern[0];
                                                restePattern = restePattern.RemoveChar(car2);
                                                var fixedPos2 = car2.IsScrabbleLetter();
                                                FindExactAnaWithFixedPositionWorker(car2, fixedPos2, motAccu, restePattern, tirage, node, resultat, longueurMot);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    string lettres = tirage.SansDoubleChar();
                                    var motAccuTemp = motAccu;
                                    var tirageTemp = tirage;
                                    var nodeTemp = node;
                                    var restePatternTemp = restePattern;

                                    foreach (char lettre in lettres)
                                    {
                                        motAccu = motAccuTemp;
                                        node = nodeTemp;
                                        restePattern = restePatternTemp;
                                        if (node.Contains(lettre))
                                        {
                                            motAccu += lettre;
                                            node = node.GetChild(lettre);
                                            tirage = tirageTemp.RemoveChar(lettre);
                                            if (restePattern.IsNotNullOrEmptyString())
                                            {
                                                var car2 = restePattern[0];
                                                restePattern = restePattern.RemoveChar(car2);
                                                var fixedPos2 = car2.IsScrabbleLetter();
                                                FindExactAnaWithFixedPositionWorker(car2, fixedPos2, motAccu, restePattern, tirage, node, resultat, longueurMot);
                                            }
                                        }
                                    }
                                }

                            }
                            else
                            {//tirage null

                                var motAccuTemp = motAccu;
                                var nodeTemp = node;
                                var restePatternTemp = restePattern;
                                foreach (char lettre in Alphabet)
                                {
                                    motAccu = motAccuTemp;
                                    node = nodeTemp;
                                    restePattern = restePatternTemp;
                                    if (node.Contains(lettre))
                                    {
                                        motAccu += lettre;
                                        node = node.GetChild(lettre);
                                        if (restePattern.IsNotNullOrEmptyString())
                                        {
                                            var car2 = restePattern[0];
                                            restePattern = restePattern.RemoveChar(car2);
                                            var fixedPos2 =car2.IsScrabbleLetter();
                                            FindExactAnaWithFixedPositionWorker(car2, fixedPos2, motAccu, restePattern, tirage, node, resultat, longueurMot);
                                        }
                                    }
                                }
                            }


                        }

                    }
                }
            }

        }

        public static bool IsJoker(this char car)
        {
            return car == '?' || car == '.' || car == ',';
        }

        public static bool IsJoker(this string c)
        {
            if (c != null && c.Length == 1)
            {
                var car = Convert.ToChar(c);
                return car == '?' || car == '.' || car == ',';
            }
            return false;
        }


        public static bool IsScrabbleLetter(this char car)
        {
            return char.IsLetter(car) || car.IsBigraph();
        }

        public static bool ContainsScrabbletter(this string word)
        {
            return word != null && word.Any(car => car.IsScrabbleLetter());
        }
        public static bool IsBigraph(this char car)
        {
            return car == '&' //ch spanish
                || car == '!' //ll spanish
                || car == '#' //rr spanish
                || car == '^' //ñ spanish
                ;
        }
        public static bool IsBigraph(this string car)
        {
            return car == "&" //ch spanish
                || car == "!" //ll spanish
                || car == "#" //rr spanish
                || car == "^" //ñ spanish
                ;
        }

        public static string ConvertBigraph(this string car)
        {
            switch (car)
            {
                case "^":
                    return "ñ";
                case "&":
                    return "ch";
                case "!":
                    return "ll";
                case "#":
                    return "rr";
                default:
                    return car;
            }
        }


        public static string ReplaceCharAtIndexOrJoker(this string word, char car, char replaceCar, int indexSurPorteLettre)
        {
            var charArray = word.ToCharArray();
            if (charArray.Length > indexSurPorteLettre)
            {
                charArray[indexSurPorteLettre] = replaceCar;
            }

            return new string(charArray);
        }

       
        public static bool ContainsJoker(this string s)
        {
            return s.Contains("?") || s.Contains(".") || s.Contains(",");
        }

        public static bool ContainsCharOrJoker(this string s, char car)
        {
            return s.IndexOf(car) >= 0 || s.Contains("?") || s.Contains(".") || s.Contains(",");
        }

        public static bool ContainsCharOrJoker(this string s, char car,out char containedChar)
        {
            containedChar = car;
            if (s.IndexOf(car) < 0)
            {
                containedChar = '.';
            }
            return s.IndexOf(car) >= 0 || s.Contains("?") || s.Contains(".") || s.Contains(",");
        }

        public static int JokerCount(this string s)
        {
            int count = 0;
            if (s != null)
                foreach (char car in s)
                {
                    if (car.IsJoker())
                    {
                        count++;
                    }
                }
            return count;
        }

        public static string RemoveJoker(this string s)
        {
            var temp = s.RemoveChar('?');
            if (temp == s)
            {
                temp = s.RemoveChar('.');
            }
            if (temp == s)
            {
                temp = s.RemoveChar(',');
            }
            return temp;
        }
        public static string RemoveAllJoker(this string s)
        {
            if (s.IsNotNullOrEmptyString())
            {
                s = s.Replace(".", "");
                s = s.Replace(",", "");
                s = s.Replace("?", "");
            }
            return s;
        }

        public static string RemoveCharOrJoker(this string word, char car)
        {
            var startIndex = word.IndexOf(car);
            if (startIndex >= 0)
            {
                return word.Remove(startIndex, 1);
            }
            else
            {
                return  word.RemoveJoker();
            }
        }



        public static string RemoveAllJokerAndNonAlpha(this string s)
        {
            StringBuilder builder = new StringBuilder();
            if (s.IsNotNullOrEmptyString())
            {
                s = s.Replace(".", "");
                s = s.Replace(",", "");
                s = s.Replace("?", "");

                foreach (char car in s)
                {
                    if (car.IsScrabbleLetter())
                    {
                        builder.Append(car);
                    }
                }
                s = builder.ToString();
            }
            return s;
        }

        static public string RemoveNonAlphabeticalChar(this string s)
        {
            if (!String.IsNullOrEmpty(s))
            {
                StringBuilder sb = new StringBuilder(s.Length);
                foreach (char c in s)
                {
                    if (c.IsScrabbleLetter()||c.IsJoker())
                    {
                        sb.Append(c);
                    }
                }
                s = sb.ToString();
            }
            return s;
        }

        public static string ShowCrossingLetter(this string s, List<char> crossList)
        {
            if (crossList != null)
            {
                foreach (char c in crossList)
                {
                    s = s.Replace(c.ToString(), "+" + c + "+");
                }
            }

            return s;
        }

        public static string RemoveAllMarks(this string s)
        {
            if (s.IsNotNullOrEmptyString())
            {
                s = s.Replace("*", "");
                s = s.Replace("+", "");
                s = s.Replace("=", "");
            }
            return s;

        }

        public static int LengthWithoutDelimiter(this string s)
        {
            if (s.IsNotNullOrEmptyString())
            {
                s = s.Replace("*", "");
                s = s.Replace("+", "");
                s = s.Replace("=", "");
                return s.Length;
            }
            return 0;
        }
        public static List<string> FindExactAnagrams(string word, TrieNode node)
        {
            //todo algo a paralleliser 
            var list = new List<string>();
            //ConcurrentBag<string> ConcurrentBag;
            var temp = word.ToCharArray().OrderBy(c => c).ToArray();
            word = new string(temp);

            var sansDoubleChar = word.ToLower().SansDoubleChar();
            //Parallel.ForEach(word, c =>
            //{

            //});
            foreach (char c in sansDoubleChar)
            {
                if (IsJoker(c))
                {

                }
                FindExactAnaWorker(c, "", word.RemoveChar(c), node, list, word.Length);
            }

            return list.Distinct().ToList();
        }

        private static void FindExactAnaWorker(char car, string motAccu, string resteMot, TrieNode node, List<string> resultat, int longueurMot)
        {

            if (node != null)
            {



                if (motAccu.Length + 1 > longueurMot)
                {
                    return;
                }
                if (motAccu.Length + 1 == longueurMot)
                {
                    if (node.Contains(car))
                    {
                        motAccu += car;
                        node = node.GetChild(car);
                        if (node.IsEnd)
                        {
                            resultat.Add(motAccu);
                        }
                    }
                    else
                    {
                        if (car.IsJoker())
                        {
                            var motAccuTemp = motAccu;
                            var nodeTemp = node;
                            foreach (char carAlpha in Alphabet)
                            {
                                motAccu = motAccuTemp;
                                node = nodeTemp;
                                if (node.Contains(carAlpha))
                                {
                                    motAccu += carAlpha;
                                    node = node.GetChild(carAlpha);
                                    if (node.IsEnd)
                                    {
                                        resultat.Add(motAccu);
                                    }
                                }
                            }
                        }
                    }
                }
                else //motAccu.Length+1 < longueurMot
                {
                    if (node.Contains(car))
                    {
                        motAccu += car;
                        node = node.GetChild(car);
                        var sansDoubleChar = resteMot.ToLower().SansDoubleChar();
                        foreach (char car2 in sansDoubleChar)
                        {
                            var resteMot2 = resteMot.RemoveChar(car2);
                            FindExactAnaWorker(car2, motAccu, resteMot2, node, resultat, longueurMot);
                        }
                    }
                    else
                    {
                        if (car.IsJoker())
                        {
                            var motAccuTemp = motAccu;
                            var nodeTemp = node;

                            foreach (char carAlpha in Alphabet)
                            {
                                motAccu = motAccuTemp;
                                node = nodeTemp;
                                if (node.Contains(carAlpha))
                                {
                                    motAccu += carAlpha;
                                    node = node.GetChild(carAlpha);
                                    var sansDoubleChar = resteMot.ToLower().SansDoubleChar();
                                    foreach (char car2 in sansDoubleChar)
                                    {
                                        var resteMot2 = resteMot.RemoveChar(car2);
                                        FindExactAnaWorker(car2, motAccu, resteMot2, node, resultat, longueurMot);
                                    }
                                }
                            }
                        }
                    }
                }
            }

        }


        /// <summary>
        /// genere les annagrammes sans tenir
        /// compte de la position des lettres
        /// </summary>
        /// <param name="word">
        /// le mot de recherche
        /// </param>
        /// <param name="node"></param>
        /// <param name="toUpperCase"></param>
        /// <param name="showJoker"></param>
        /// <returns></returns>
        public static IDictionary<int, IList<string>> FinadAnna(string word, TrieNode node, bool toUpperCase = false, bool showJoker = false)
        {
            IDictionary<int, IList<string>> list = new Dictionary<int, IList<string>>();
            foreach (char c in word.SansDoubleChar())
            {
                FindAnnaWorker("", word, c, node, ref list, toUpperCase, showJoker);
            }
            return list;
        }


        private static void FindAnnaWorker(string result, string word, char c, TrieNode node, ref IDictionary<int, IList<string>> list, bool toUpperCase = false, bool showJoker = false)
        {
            if (node.Contains(c))
            {
                result += c;
                var child = node.GetChild(c);
                if (child.IsEnd)
                {
                    if (list.ContainsKey(result.LengthWithoutDelimiter()) == false)
                    {
                        list[result.LengthWithoutDelimiter()] = new List<string>();
                    }
                    //list[result.Length].Add(result);
                    list[result.LengthWithoutDelimiter()].Add(toUpperCase == true ? result.ToUpperInvariant() : result);
                }
                //node = node.GetChild(c);
                word = word.Remove(word.IndexOf(c), 1);
                foreach (char car in word.SansDoubleChar())
                {
                    FindAnnaWorker(result, word, car, child, ref list, toUpperCase, showJoker);
                }
            }
            else
            {

                if (c.IsJoker())
                {
                    //var result2 = result;
                    word = word.Remove(word.IndexOf(c), 1);
                    foreach (char c2 in Alphabet)
                    {
                        var result2 = result;
                        if (node.Contains(c2))
                        {
                            if (showJoker)
                            {
                                result2 += "*" + c2 + "*";
                            }
                            else
                            {
                                result2 += c2;
                            }

                            var child = node.GetChild(c2);
                            if (child.IsEnd)
                            {
                                if (list.ContainsKey(result2.LengthWithoutDelimiter()) == false)
                                {
                                    list[result2.LengthWithoutDelimiter()] = new List<string>();
                                }
                                list[result2.LengthWithoutDelimiter()].Add(toUpperCase == true ? result2.ToUpperInvariant() : result2);
                            }
                            foreach (char car in word.SansDoubleChar())
                            {
                                FindAnnaWorker(result2, word, car, child, ref list, toUpperCase, showJoker);
                            }
                        }
                    }
                }
            }
        }

        public static Dictionary<int, List<string>> FindAllPossibleWord(string word, TrieNode node, bool toUpperCase = false, bool showJoker = false, Range range = null, string mustContainCar = "", string debutMot = "")
        {
            var list = new Dictionary<int, List<string>>();
            if (range == null)
            {
                range = new Range(2, 7);
            }
            range.CheckRangeValues();
            //word = StringExtensions.SortString(word);
            // var sansDoubleChar = StringExtensions.SortString(word.SansDoubleChar());
            var sansDoubleChar = word.SansDoubleChar();
            var dejaCherche = new HashSet<char>();
            foreach (char letter in sansDoubleChar)
            {
                var restemot = StringExtensions.RemoveChar(word, letter);
                FindAllPossibleWordWorker(letter, false, debutMot, restemot, node, range, ref list, toUpperCase, showJoker, new HashSet<char>(dejaCherche), mustContainCar: mustContainCar);
                //dejaCherche.Add(letter);
            }
            return list;
        }

        public static void FindAllPossibleWordWorker(char letter, bool letterIsJoker, string mot, string resteMot, TrieNode rootNode, Range range,
            ref Dictionary<int, List<string>> result, bool toUpperCase = false, bool showJoker = false, HashSet<char> dejaCherche = null, string mustContainCar = "")
        {

            if (StringExtensions.IsNullOrEmptyString(resteMot))//dernier char du mot
            {
                if (letter.IsScrabbleLetter())
                {
                    #region MyRegion
                    if (rootNode.Contains(letter))
                    {
                        var childNode = rootNode.GetChildOrNull(letter);
                        if (childNode.IsEnd)
                        {
                            AddToResult(mot, letter, letterIsJoker, range, ref result, toUpperCase, showJoker, mustContainCar);
                        }
                    }
                    #endregion
                }
                else
                {
                    if (IsJoker(letter))
                    {
                        foreach (char car in Alphabet)
                        {
                            FindAllPossibleWordWorker(car, true, mot, resteMot, rootNode, range, ref result, toUpperCase: toUpperCase, showJoker: showJoker, mustContainCar: mustContainCar);
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
                            AddToResult(mot, letter, letterIsJoker, range, ref result, toUpperCase, showJoker, mustContainCar);
                        }

                        mot = SetMot(letter, letterIsJoker, mot, showJoker, mustContainCar);

                        var dejaCherche2 = new HashSet<char>();
                        var sansDoubleChar = resteMot.SansDoubleChar();
                        foreach (char car in sansDoubleChar)
                        {
                            var restemot2 = StringExtensions.RemoveChar(resteMot, car);
                            FindAllPossibleWordWorker(car, false, mot, restemot2, childNode, range, ref result, toUpperCase: toUpperCase,
                                showJoker: showJoker, dejaCherche: new HashSet<char>(dejaCherche2), mustContainCar: mustContainCar);
                            //dejaCherche2.Add(car);
                        }

                    }
                    #endregion
                }
                else
                {
                    if (IsJoker(letter))
                    {
                        var dejaCherche2 = new HashSet<char>();
                        foreach (char car in Alphabet)
                        {
                            if (dejaCherche != null)
                            {
                                if (dejaCherche.Contains(car))
                                {

                                }
                                else
                                {
                                    FindAllPossibleWordWorker(car, true, mot, resteMot, rootNode, range, ref result, toUpperCase: toUpperCase, showJoker: showJoker,
                                        dejaCherche: new HashSet<char>(dejaCherche2), mustContainCar: mustContainCar);
                                    //dejaCherche2.Add(car);
                                }
                            }
                            else
                            {
                                FindAllPossibleWordWorker(car, true, mot, resteMot, rootNode, range, ref result,
                                    toUpperCase: toUpperCase, showJoker: showJoker, mustContainCar: mustContainCar);
                            }

                        }
                    }
                }
            }
        }

        private static string SetMot(char letter, bool letterIsJoker, string mot, bool showJoker, string mustContainCar = null)
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
                if (showJoker)
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

        public static void AddToResult(string mot, char letter, bool letterIsJoker, Range range, ref Dictionary<int, List<string>> result, bool toUpperCase = false, bool showJoker = false, string mustContainCar = "")
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
                if (showJoker)
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
                        result[wordLength].Add(toUpperCase ? mot.ToUpperInvariant() : mot);
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
                    result[wordLength].Add(toUpperCase ? mot.ToUpperInvariant() : mot);
                }
            }


        }


        public static IDictionary<int, IList<string>> FindAnnaPos(string theWord, TrieNode node)
        {
            IDictionary<int, IList<string>> result = new Dictionary<int, IList<string>>();
            FindAnnaPosWorker("", theWord, ref result, node, theWord.Length);
            return result;
        }

        private static void FindAnnaPosWorker(string res, string word, ref IDictionary<int, IList<string>> list, TrieNode node, int wordLength)
        {
            if (String.IsNullOrEmpty(word) == false)
            {
                var car = word[0];
                if (node.Contains(car))
                {
                    res += car;
                    var child = node.GetChild(car);
                    if (child.IsEnd && res.Length == wordLength)
                    {
                        if (list.ContainsKey(res.Length) == false)
                        {
                            list[res.Length] = new List<string>();
                        }
                        list[res.Length].Add(res);
                    }
                    word = word.Remove(word.IndexOf(car), 1);
                    FindAnnaPosWorker(res, word, ref list, child, wordLength);
                }
                else
                {
                    if (car == '?')
                    {
                        var res2 = res;
                        word = word.Remove(word.IndexOf(car), 1);
                        foreach (char c in Alphabet)
                        {
                            res2 = res;
                            if (node.Contains(c))
                            {
                                res2 += c;
                                var child = node.GetChild(c);
                                if (child.IsEnd && res2.Length == wordLength)
                                {
                                    if (list.ContainsKey(res2.Length) == false)
                                    {
                                        list[res2.Length] = new List<string>();
                                    }
                                    list[res2.Length].Add(res2);
                                }
                                FindAnnaPosWorker(res2, word, ref list, child, wordLength);
                            }
                        }
                    }
                }
            }
        }

        public static IDictionary<int, IList<string>> WordContain(string subString, IList<string> wordList)
        {
            if (subString.IsNullOrEmptyString())
            {
                return null;
            }
            else
            {
                IDictionary<int, IList<string>> list = new Dictionary<int, IList<string>>();
                foreach (string word in wordList)
                {
                    // Console.WriteLine(word);
                    if (word.Contains(subString))
                    {
                        if (list.ContainsKey(word.Length) == false)
                        {
                            list[word.Length] = new List<string>();
                        }
                        list[word.Length].Add(word); ;
                    }
                }
                return list;
            }

        }

        public static IDictionary<int, List<string>> WordStartWith(string subString, IList<string> wordList)
        {
            if (subString.IsNullOrEmptyString())
            {
                return null;
            }
            else
            {
                IDictionary<int, List<string>> list = new Dictionary<int, List<string>>();
                foreach (string word in wordList)
                {
                    // Console.WriteLine(word);
                    if (word.StartsWith(subString))
                    {
                        if (list.ContainsKey(word.Length) == false)
                        {
                            list[word.Length] = new List<string>();
                        }
                        list[word.Length].Add(word); ;
                    }
                }
                return list;
            }

        }

        public static IDictionary<int, IList<string>> WordendWith(string subString, IList<string> wordList)
        {
            if (subString.IsNullOrEmptyString())
            {
                return null;
            }
            else
            {
                IDictionary<int, IList<string>> list = new Dictionary<int, IList<string>>();
                foreach (string word in wordList)
                {
                    // Console.WriteLine(word);
                    if (word.EndsWith(subString))
                    {
                        if (list.ContainsKey(word.Length) == false)
                        {
                            list[word.Length] = new List<string>();
                        }
                        list[word.Length].Add(word); ;
                    }
                }
                return list;
            }

        }


        public static bool IsValidAlphabetWord(this string str)
        {
            return Regex.Match(str, @"[a-z]+").Length == str.Length;
        }

        public static bool IsValidAlphabetWord2(this string str)
        {
            foreach (char c in str)
            {
                if (c.IsScrabbleLetter() == false)
                {
                    return false;
                }
            }
            return true;
        }

        public static IDictionary<int, IList<string>> FinadComplexAnaPos(string pattern, string tirage, IList<string> listemot)
        {
            IDictionary<int, IList<string>> list = new Dictionary<int, IList<string>>();
            foreach (string word in listemot)
            {
                if (IsvalidAna(word, pattern, tirage))
                {
                    if (list.ContainsKey(word.Length) == false)
                    {
                        list[word.Length] = new List<string>();
                    }
                    list[word.Length].Add(word);
                }
            }
            return list;
        }

        public static bool IsvalidAna(string mot, string pattern, string tirage)
        {
            if (Regex.IsMatch(mot, pattern) && mot.IsAnnagramOf(tirage))
                return true;
            else
                return false;
        }

        /// <summary>
        /// verifie si une chaine peut etre un annagramme d'une autre chaine 
        /// </summary>
        /// <param name="str1"></param>
        /// <param name="str2"></param>
        /// <returns></returns>
        public static bool IsAnnagramOf(this string str1, string str2)
        {
            if (str1.Length > str2.Length)
            {
                return false;
            }
            else
            {

                int ind = -10;
                foreach (char c in str1)
                {
                    ind = str2.IndexOf(c);
                    if (ind >= 0)
                    {
                        str2 = str2.Remove(ind, 1);
                    }
                    else
                    {
                        ind = str2.IndexOf('?');
                        if (ind >= 0)
                        {
                            str2 = str2.Remove(ind, 1);
                        }
                        else return false;
                    }
                }
                return true;
            }
        }
        public static string ConvertToRegex(string str)
        {
            return str.Replace("*", "[a-z]*").Replace("?", ".");

        }
    }
}
