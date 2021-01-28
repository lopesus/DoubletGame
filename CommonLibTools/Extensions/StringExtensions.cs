using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace CommonLibTools.Extensions
{
    public static class StringExtensions
    {
        #region BASE CONVERSION
        public static string Base64Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
        public static Dictionary<char, int> Base34CharsToInt = new Dictionary<char, int>()
        {
            {'A',0},{'B',1},
            {'C',2},{'D',3},
            {'E',4},{'F',5},
            {'G',6},{'H',7},
            {'I',8},{'J',9},
            {'K',10},{'L',11},
            {'M',12},{'N',13},
            {'O',14},{'P',15},
            {'Q',16},{'R',17},
            {'S',18},{'T',19},
            {'U',20},{'V',21},
            {'W',22},{'X',23},
            {'Y',24},{'Z',25},
            {'a',26},{'b',27},
            {'c',28},{'d',29},
            {'e',30},{'f',31},
            {'g',32},{'h',33},
            {'i',34},{'j',35},
            {'k',36},{'l',37},
            {'m',38},{'n',39},
            {'o',40},{'p',41},
            {'q',42},{'r',43},
            {'s',44},{'t',45},
            {'u',46},{'v',47},
            {'w',48},{'x',49},
            {'y',50},{'z',51},
            {'0',52},{'1',53},
            {'2',54},{'3',55},
            {'4',56},{'5',57},
            {'6',58},{'7',59},
            {'8',60},{'9',61},
            {'+',62},{'/',63},
        };

        public static Dictionary<int, char> Base64IntToChar = new Dictionary<int, char>()
        {
            {0,'A'},{1,'B'},
            {2,'C'},{3,'D'},
            {4,'E'},{5,'F'},
            {6,'G'},{7,'H'},
            {8,'I'},{9,'J'},
            {10,'K'},{11,'L'},
            {12,'M'},{13,'N'},
            {14,'O'},{15,'P'},
            {16,'Q'},{17,'R'},
            {18,'S'},{19,'T'},
            {20,'U'},{21,'V'},
            {22,'W'},{23,'X'},
            {24,'Y'},{25,'Z'},
            {26,'a'},{27,'b'},
            {28,'c'},{29,'d'},
            {30,'e'},{31,'f'},
            {32,'g'},{33,'h'},
            {34,'i'},{35,'j'},
            {36,'k'},{37,'l'},
            {38,'m'},{39,'n'},
            {40,'o'},{41,'p'},
            {42,'q'},{43,'r'},
            {44,'s'},{45,'t'},
            {46,'u'},{47,'v'},
            {48,'w'},{49,'x'},
            {50,'y'},{51,'z'},
            {52,'0'},{53,'1'},
            {54,'2'},{55,'3'},
            {56,'4'},{57,'5'},
            {58,'6'},{59,'7'},
            {60,'8'},{61,'9'},
            {62,'+'},{63,'/'},
            
        };

        public static string Int32ToBase16(this int val)
        {
            return Convert.ToString(val, 16);
        }

        public static int Base16ToInt32(this string val)
        {
            return Convert.ToInt32(val, 16);
        }

        public static string Int32ToBase64(this int val)
        {
            var finalString = new StringBuilder();

            var base2 = Convert.ToString(val, 2).ToCharArray();
            var temp = new StringBuilder();
            var count = 0;
            for (int i = base2.Length - 1; i >= 0; i--)
            {
                temp.Insert(0, base2[i].ToString());
                count++;
                if (count == 6)
                {
                    var intVal = Convert.ToInt32(temp.ToString(), 2);
                    finalString.Insert(0, Base64IntToChar[intVal].ToString());
                    count = 0;
                    temp = new StringBuilder();
                }
            }
            if (count > 0)
            {
                var intVal = Convert.ToInt32(temp.ToString(), 2);
                finalString.Insert(0, Base64IntToChar[intVal].ToString());
                count = 0;
                temp = new StringBuilder();
            }

            return finalString.ToString();
        }

        public static int Base64ToInt32(this string val)
        {
            double finalVal = 0;
            var tokens = val.ToCharArray();
            var count = 0;
            for (int i = tokens.Length - 1; i >= 0; i--)
            {
                var mul = 1 << (6 * count);// Math.Pow(64, count);
                count++;
                var tempval = mul * Base34CharsToInt[tokens[i]];
                finalVal += tempval;
            }

            return (int)finalVal;
        }


        /// <summary>
        /// Converts the given decimal number to the numeral system with the
        /// specified radix (in the range [2, 36]).
        /// </summary>
        /// <param name="decimalNumber">The number to convert.</param>
        /// <param name="radix">The radix of the destination numeral system (in the range [2, 36]).</param>
        /// <returns></returns>
        public static string DecimalToArbitrarySystem(this int decimalNumber, int radix)
        {
            const int BitsInLong = 64;
            const string Digits = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            if (radix < 2 || radix > Digits.Length)
                throw new ArgumentException("The radix must be >= 2 and <= " + Digits.Length.ToString());

            if (decimalNumber == 0)
                return "0";

            int index = BitsInLong - 1;
            long currentNumber = Math.Abs(decimalNumber);
            char[] charArray = new char[BitsInLong];

            while (currentNumber != 0)
            {
                int remainder = (int)(currentNumber % radix);
                charArray[index--] = Digits[remainder];
                currentNumber = currentNumber / radix;
            }

            string result = new String(charArray, index + 1, BitsInLong - index - 1);
            if (decimalNumber < 0)
            {
                result = "-" + result;
            }

            return result;
        }

        #endregion

        
        //public static string ToJson(this object obj)
        //{
        //    var json = JsonConvert.SerializeObject(obj,Formatting.Indented);
        //    return json;
        //}
        //public static T DeserializeJsonTo<T>(this string json)
        //{
        //    T obj = JsonConvert.DeserializeObject<T>(json);
        //    return obj;
        //}


        public static string ReplaceAtPosition(this string text, int pos, char val)
        {
            var charArray = text.ToCharArray();
            charArray[pos] = val;
            return new string(charArray);
            //.Insert(pos,val);
            //return insert;
        }

        

        public static bool ContainsAnyLetter(this string aString, string letters)
        {
            foreach (char s in letters)
            {
                if (aString.Contains(s.ToString())) return true;
            }
            return false;
        }

        public static string ReplaceWhiteSpaceRegex(this string text)
        {
            if (text != null)
            {
                return Regex.Replace(text, @"\s+", " ");
            }
            return text;
        }

        public static bool ContainsAnyLetter(this string aString, List<char> letters)
        {
            foreach (char s in letters)
            {
                if (aString.Contains(s.ToString())) return true;
            }
            return false;
        }

        public static int CountCharOccurence(this string chaine, char car)
        {
            int cnt = 0;

            if (chaine != null)
            {
                foreach (char c in chaine)
                {
                    if (c == car)
                    {
                        cnt++;
                    }
                }
            }

            return cnt;
        }
        public static string SortStringAlphabetically(this string str)
        {
            if (str != null)
            {
                char[] foo = str.ToCharArray();
                Array.Sort(foo);
                return new string(foo);
            }
            return null;
        }

        public static string SortStringAlphabeticallyReverse(this string str)
        {
            if (str != null)
            {
                char[] foo = str.ToCharArray();
                Array.Sort(foo);
                Array.Reverse(foo);
                return new string(foo);
            }
            return null;
        }
        public static string RemoveChar(this string word, char car)
        {
            var startIndex = word.IndexOf(car);
            if (startIndex >= 0)
            {
                return word.Remove(startIndex, 1);
            }
            return word;
        }

        public static string RemoveCharFromString(this string source, string charToRemove)
        {
            if (source != null)
            {
                if (charToRemove != null)
                {
                    foreach (char car in charToRemove)
                    {
                        source = source.RemoveChar(car);
                    }
                }
            }



            return source;
        }

        public static bool IsWord(this string word)
        {
            var alphabet = "abcdefghijklmnopqrstuvwxyz-";
            var alphabet2 = "abcdefghijklmnopqrstuvwxyz";
            if (word != null)
            {

                word = word.ToLower();//.SansAccent();
                //verification premiere lettre
                if (!alphabet2.Contains(word[0].ToString()))
                {
                    return false;
                }
                foreach (char c in word)
                {
                    if (alphabet.Contains(c.ToString()))
                    {
                        continue;
                    }
                    else
                    {
                        return false;
                    }
                }
                return true;

            }
            return false;
        }
        public static int ToInt(this string s)
        {
            if (s.IsNullOrEmptyString())
            {
                return 0;
            }
            int val = 0;
            Int32.TryParse(s.Trim(), out val);
            return val;
        }

        public static int ToPositiveStringNumber(this string s)
        {
            if (s.IsNullOrEmptyString())
            {
                return 0;
            }
            s = s.Replace(" ", "");
            int val = 0;
            Int32.TryParse(s, out val);
            return val;
        }
        public static string SousChaine(this string s, int longueur)
        {
            if (s.IsNullOrEmptyString())
            {
                return s;
            }
            return s.Substring(0, Math.Min(longueur, s.Length));
        }


        public static bool IsValidEmail(this string email)
        {
            Regex regex = new Regex(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", RegexOptions.None);

            //string str = Convert.ToString(value, CultureInfo.CurrentCulture);
            if (String.IsNullOrEmpty(email))
                return false;

            //Match match = regex.Match(str);
            var r = regex.IsMatch(email);
            return r;
        }

        public static string RemoveAtEnd(this string aString, string stringToRemoveAtEnd)
        {
            return aString.Substring(0, aString.Length - stringToRemoveAtEnd.Length);
        }


        public static bool IsNotNullOrEmptyString(this string aString)
        {
            return !String.IsNullOrEmpty(aString) && !String.IsNullOrWhiteSpace(aString);
        }

        public static bool IsLetter000(this char car)
        {
            return char.IsLetter(car);
        }
        public static bool IsNullOrEmptyString(this string aString)
        {
            return String.IsNullOrEmpty(aString) || String.IsNullOrWhiteSpace(aString);
        }

        public static string PadLeft(this string aString, string padder)
        {
            return padder + aString;
        }


        public static string SansAccent(this string chaine)
        {

            // Déclaration de variables
            string accent = "ÀÁÂÃÄÅàáâãäåÒÓÔÕÖØòóôõöøÈÉÊËèéêëÌÍÎÏìíîïÙÚÛÜùúûüÿÑñÇç";
            string sansAccent = "AAAAAAaaaaaaOOOOOOooooooEEEEeeeeIIIIiiiiUUUUuuuuyNnCc";

            // Conversion des chaines en tableaux de caractères
            char[] tableauSansAccent = sansAccent.ToCharArray();
            char[] tableauAccent = accent.ToCharArray();

            // Pour chaque accent
            for (int i = 0; i < accent.Length; i++)
            {
                // Remplacement de l'accent par son équivalent sans accent dans la chaîne de caractères
                chaine = chaine.Replace(tableauAccent[i].ToString(), tableauSansAccent[i].ToString());
            }

            // Retour du résultat
            return chaine;
        }

        public static string RemoveSpaceChar(this string chaine)
        {
            chaine = chaine.Replace(" ", "-");

            // Retour du résultat
            return chaine.ToLower().Trim();
        }

        static public string RemoveNonAlphaNumericChar(this string s)
        {
            if (!String.IsNullOrEmpty(s))
            {
                StringBuilder sb = new StringBuilder(s.Length);
                foreach (char c in s)
                {
                    sb.Append(Char.IsControl(c) ? ' ' : c);
                }
                s = sb.ToString();
            }
            return s;
        }


        public static string ToValidNameForIdentifier(this string chaine)
        {
            if (chaine == null)
            {
                return null;
            }
            chaine = chaine.Trim();
            // Déclaration de variables
            const string accent = "ÀÁÂÃÄÅàáâãäåÒÓÔÕÖØòóôõöøÈÉÊËèéêëÌÍÎÏìíîïÙÚÛÜùúûüÿÑñÇç";
            const string sansAccent = "AAAAAAaaaaaaOOOOOOooooooEEEEeeeeIIIIiiiiUUUUuuuuyNnCc";

            // Conversion des chaines en tableaux de caractères
            char[] tableauSansAccent = sansAccent.ToCharArray();
            char[] tableauAccent = accent.ToCharArray();

            // Pour chaque accent
            for (int i = 0; i < accent.Length; i++)
            {
                // Remplacement de l'accent par son équivalent sans accent dans la chaîne de caractères
                chaine = chaine.Replace(tableauAccent[i].ToString(), tableauSansAccent[i].ToString());
            }

            //on supprime les espaces;
            string find = @"\s+";
            string replace = " ";
            chaine = Regex.Replace(chaine, find, replace);
            find = @"[\/:*?""<>|.,#]";
            //chaine = Regex.Replace(chaine, find, "");


            chaine = chaine.Replace(",", "-");
            chaine = chaine.Replace("#", "-");
            chaine = chaine.Replace("%", "-");
            chaine = chaine.Replace(".", "-");
            chaine = chaine.Replace(":", "-");
            chaine = chaine.Replace("/", "-");
            chaine = chaine.Replace("\\", "-");
            chaine = chaine.Replace("&", "-");
            chaine = chaine.Replace("*", "-");
            chaine = chaine.Replace("?", "-");
            chaine = chaine.Replace("\"", "-");
            chaine = chaine.Replace("'", "-");
            chaine = chaine.Replace(";", "");
            //chaine = chaine.Replace(" ", "-");

            chaine = Regex.Replace(chaine, "-+", "-");
            // Retour du résultat
            return chaine.ToLower().Trim();
        }






        public static string ToLocationIdentifier(this string chaine)
        {
            if (chaine == null)
            {
                return null;
            }
            chaine = chaine.Trim();

            // Déclaration de variables
            const string accent = "ÀÁÂÃÄÅàáâãäåÒÓÔÕÖØòóôõöøÈÉÊËèéêëÌÍÎÏìíîïÙÚÛÜùúûüÿÑñÇç";
            const string sansAccent = "AAAAAAaaaaaaOOOOOOooooooEEEEeeeeIIIIiiiiUUUUuuuuyNnCc";

            // Conversion des chaines en tableaux de caractères
            char[] tableauSansAccent = sansAccent.ToCharArray();
            char[] tableauAccent = accent.ToCharArray();

            // Pour chaque accent
            for (int i = 0; i < accent.Length; i++)
            {
                // Remplacement de l'accent par son équivalent sans accent dans la chaîne de caractères
                chaine = chaine.Replace(tableauAccent[i].ToString(), tableauSansAccent[i].ToString());
            }

            //on supprime les char , . - _ ;

            chaine = chaine.Replace("-", " ");

            //on reduit  les espaces;
            string find = @"(\s)+";
            string replace = " ";
            chaine = Regex.Replace(chaine, find, replace);


            chaine = chaine.Replace(" ", "-");
            chaine = chaine.Replace(".", "-");

            // Retour du résultat
            return chaine.ToLower().Trim();
        }
        public static string ToEmailIdentifier(this string email)
        {
            return email.Replace('.', '_').Trim().ToLower();
        }

        public static string FormatPhoneNumber(this string tel)
        {
            if (tel != null) return tel.Replace("/", " / ");
            return null;
        }

        public static string Reverse(this string text)
        {
            if (text == null) return null;

            char[] array = text.ToCharArray();
            Array.Reverse(array);
            return new string(array);
        }
        public static string SortCharInString(this string text)
        {
            if (text == null) return null;

            char[] foo = text.ToCharArray();
            Array.Sort(foo);
            return new string(foo);
        }

        /// <summary>
        /// enleve les doubles caracteres d'une chaine de caracteres
        /// </summary>
        /// <param name="aString"></param>
        /// <returns></returns>
        public static string SansDoubleChar(this string aString)
        {
            IDictionary<char, char> dico = new Dictionary<char, char>();
            var res = "";
            if (String.IsNullOrEmpty(aString) == false)
            {
                foreach (char c in aString)
                {
                    if (dico.ContainsKey(c) == false)
                    {
                        dico.Add(c, c);
                        res += c;
                    }
                }
                return res;
            }
            return aString;
        }
        public static string ToAlphabetCharOnly(this string aString)
        {

            var res = "";
            if (String.IsNullOrEmpty(aString) == false)
            {
                foreach (char c in aString)
                {
                    if (Char.IsLetter(c))
                    {
                        res += c;
                    }
                }
                return res;
            }
            return aString;
        }

        public static string GenerateGuid()
        {
            return Guid.NewGuid().ToString().Replace("-", "");
        }

        public static bool IsWordOnly(this string word)
        {
            if (word.Contains("-"))
            {
                return false;
            }
            var alphabet = "abcdefghijklmnopqrstuvwxyz";
            if (word.IsNotNullOrEmptyString())
            {

                word = word.ToLower();//.SansAccent();

                foreach (char c in word)
                {
                    if (!alphabet.Contains(c.ToString()))
                    {
                        return false;
                    }
                }
                return true;

            }
            return false;
        }
    }
}
