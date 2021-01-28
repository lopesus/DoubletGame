using System;
using System.Collections.Generic;
using CommonLibTools.Extensions;

namespace CommonLibTools.DataStructure.Dawg
{
    /// <summary>
    /// 
    /// </summary>
    public static class Utils
    {
        public static string Alphabet = "abcdefghijklmnopqrstuvwxyz";

        static int[] _bitcounts; // Lookup table
        static void InitializeBitcounts()
        {
            _bitcounts = new int[65536];
            int position1 = -1;
            int position2 = -1;
            //
            // Loop through all the elements and assign them.
            //
            for (int i = 1; i < 65536; i++, position1++)
            {
                //
                // Adjust the positions we read from.
                //
                if (position1 == position2)
                {
                    position1 = 0;
                    position2 = i;
                }
                _bitcounts[i] = _bitcounts[position1] + 1;
            }
        }

        // <summary>
        /// Fonction de conversion de chaîne accentué en chaîne sans accent
        /// </summary>
        /// <param name="chaine">La chaine à convertir</param>
        /// <returns>string</returns>

        public static string FillWith(this string chaine,char car)
        {
            return  new string(car,chaine.Length);
        }

        //public static string SansAccent00(this string chaine)
        //{
        //    // Déclaration de variables
        //    string accent = "ÀÁÂÃÄÅàáâãäåÒÓÔÕÖØòóôõöøÈÉÊËèéêëÌÍÎÏìíîïÙÚÛÜùúûüÿÑñÇç";
        //    string sansAccent = "AAAAAAaaaaaaOOOOOOooooooEEEEeeeeIIIIiiiiUUUUuuuuyNnCc";

        //    // Conversion des chaines en tableaux de caractères
        //    char[] tableauSansAccent = sansAccent.ToCharArray();
        //    char[] tableauAccent = accent.ToCharArray();

        //    // Pour chaque accent
        //    for (int i = 0; i < accent.Length; i++)
        //    {
        //        // Remplacement de l'accent par son équivalent sans accent dans la chaîne de caractères
        //        chaine = chaine.Replace(tableauAccent[i].ToString(), tableauSansAccent[i].ToString());
        //    }

        //    // Retour du résultat
        //    return chaine;
        //}

      
        public static string Reverse(this string text)
        {
            if (text == null) return null;

            // this was posted by petebob as well 
            char[] array = text.ToCharArray();
            Array.Reverse(array);
            return new string(array);
        }


        //public static string SansDoubleChar(this string aString)
        //{
        //    IDictionary<char, char> dico = new Dictionary<char, char>();
        //    var res = "";
        //    if (string.IsNullOrEmpty(aString) == false)
        //    {
        //        foreach (char c in aString)
        //        {
        //            if (dico.ContainsKey(c) == false)
        //            {
        //                dico.Add(c, c);
        //                res += c;
        //            }
        //        }
        //        return res;
        //    }
        //    return aString;
        //}

        public static IList<string> GenAnnagramme(string theString)
        {
            IList<string> result = new List<string>();

            if (string.IsNullOrEmpty(theString) == false)
            {
                foreach (char c in theString.SansDoubleChar())
                {
                    //char c = theString[i];
                    var res = string.Empty;
                    genAn(res, c, theString, ref result);
                }
            }
            //genAn("",theString[0], theString, ref result);
            return result;
        }
        private static void genAn(string res, char car, string word, ref IList<string> list)
        {

            res += car;
            list.Add(res.ToString());
            //Console.WriteLine(res.ToString());
            word = word.Remove(word.IndexOf(car), 1);

            foreach (char c in word)
            {
                //var newWord = word.Remove(c, 1);
                genAn(res, c, word, ref list);
            }


        }

        public static IDictionary<string, short> FindAnnagramme(string theString, TrieNode node)
        {
            IDictionary<string, short> result = new Dictionary<string, short>();

            /* if (string.IsNullOrEmptyString(theString) == false)
             {
                 foreach (char c in theString.SansDoubleChar())
                 {
                     //char c = theString[i];
                     var res = string.Empty;
                     FindAnnagrammeWorker(res, c, theString, ref result,node);
                 }
             }*/
            FindAnnagrammeWorker("", theString[0], theString, ref result, node);
            return result;
        }
        private static void FindAnnagrammeWorker(string res, char car, string word, ref IDictionary<string, short> list, TrieNode node)
        {

            if (node.Contains(car))
            {
                res += car;
                var child = node.GetChild(car);
                if (child.IsEnd)
                {
                    list[res.ToString()] = 0;
                }
                //Console.WriteLine(res.ToString());
                word = word.Remove(word.IndexOf(car), 1);
                foreach (char c in word)
                {
                    //var newWord = word.Remove(c, 1);
                    FindAnnagrammeWorker(res, c, word, ref list, child);
                }
            }
        }

        public static IDictionary<string, short> FindWordWithJolly(string theWord, TrieNode node)
        {
            IDictionary<string, short> result = new Dictionary<string, short>();
            FindWordWithJollyWorker("", theWord, ref result, node);
            return result;
        }

        private static void FindWordWithJollyWorker(string res, string word, ref IDictionary<string, short> list, TrieNode node)
        {
            if (string.IsNullOrEmpty(word) == false)
            {
                var car = word[0];
                if (node.Contains(car))
                {
                    res += car;
                    var child = node.GetChild(car);
                    if (child.IsEnd)
                    {
                        list[res] = 0;
                    }
                    //Console.WriteLine(res.ToString());
                    word = word.Remove(word.IndexOf(car), 1);
                    FindWordWithJollyWorker(res, word, ref list, child);
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
                                if (child.IsEnd)
                                {
                                    list[res2] = 0;
                                }
                                FindWordWithJollyWorker(res2, word, ref list, child);
                            }
                        }
                    }
                }
            }
        }
        public static IList<string> RallongePossible(string mot, Trie trie)
        {
            var node = trie.GetLastNode(mot);
            var re = new List<string>();
            if (node.ChildNodes == null)
            {
                return re;
            }
            else
            {
                foreach (char car in node.ChildNodes.Keys)
                {
                    if (node.GetChild(car).IsEnd)
                    {
                        re.Add(mot + car);
                    }
                }
                return re;

            }

        }
        /// <summary>
        /// cherche tous les caracteres tels que mot?mot2 soit valide
        /// </summary>
        /// <param name="mot">The mot.</param>
        /// <param name="mot2">The mot2.</param>
        /// <param name="trie">The trie.</param>
        /// <returns></returns>
       
        public static IList<string> RallongePossible(string mot, Trie trie, string tirage)
        {
            //HashSet<char> set=new HashSet<char>();
            var re = new List<string>();
            if (!string.IsNullOrEmpty(tirage))
            {
                var node = trie.GetLastNode(mot);
                if (node.ChildNodes == null)
                {
                    return re;
                }
                else
                {
                    foreach (char car in node.ChildNodes.Keys)
                    {
                        if (tirage.Contains(car.ToString()))
                            if (node.GetChild(car).IsEnd)
                            {
                                re.Add(mot + car);
                            }
                    }
                    return re;

                }
            }
            return re;


        }
        public static IList<string> WordsWithSubString(string subString, IList<string> wordList)
        {
            if (subString.IsNotNullOrEmptyString())
            {
                return null;
            }
            else
            {
                IList<string> re = new List<string>();
                foreach (string word in wordList)
                {
                    // Console.WriteLine(word);
                    if (word.Contains(subString))
                    {
                        re.Add(word);
                    }
                }
                return re;
            }

        }
    }
}
