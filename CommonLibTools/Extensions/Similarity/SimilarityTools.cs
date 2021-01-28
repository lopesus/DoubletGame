using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CommonLibTools.Extensions.Similarity
{
    public static class SimilarityTools
    {
       
        public static double TauxSimilariteBasic(List<string> list1, List<string> list2)
        {
            var union = list1.Concat(list2);
            var inter = list1.Intersect(list2);

            var intersect = new List<string>();
            var collection1 = list1.Where(e => inter.Contains(e)).ToList();
            intersect.AddRange(collection1);
            var collection2 = list2.Where(e => inter.Contains(e)).ToList();
            intersect.AddRange(collection2);

            var icount = (float)intersect.Count();
            var ucount = union.Count();
            float res = icount / ucount;
            return res;
        }

        public static double TauxSimilariteBasic(Dictionary<string, int> dict1, int nbreMot1, Dictionary<string, int> dict2, int nbreMot2)
        {
            if (dict1 != null && dict2 != null)
            {
                var inter = dict1.Keys.Intersect(dict2.Keys);
                int ninter = 0;
                foreach (string s in inter)
                {
                    ninter += dict1[s];
                    ninter += dict2[s];
                }
                float tauxSimilariteBasic = (float)ninter / (nbreMot1 + nbreMot2);
                if (tauxSimilariteBasic>1)
                {
                    XTools.PrintObject(tauxSimilariteBasic);
                }
                return tauxSimilariteBasic;
            }
            //throw new ArgumentNullException("dict1 ou dict2 null");
            return 0;
        }

        public static SimilarityInfos CalculateBasicSimilarity(string text1, string text2)
        {

            text1 = text1.Replace("«", " ").Replace("»", " ").Replace("’", " ").Replace("\"", " ");
            string pattern = @"[\s\.,:'\(\)\?-]";
            var token1 = Regex.Split(text1, pattern);

            text2 = text2.Replace("«", " ").Replace("»", " ").Replace("’", " ").Replace("\"", " ");
            var token2 = Regex.Split(text2, pattern);

            //var token1 = text1.Split(@" \n\r".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            //var token2 = text2.Split(@" \n\r".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            var dict1 = new Dictionary<string, int>();
            var dict2 = new Dictionary<string, int>();
            var allWords = new HashSet<string>();

            for (int i = 0; i < token1.Length; i++)
            {
                var key = token1[i]/*.SansAccent()*/.Trim();
                if (key.IsNotNullOrEmptyString())
                {
                    allWords.Add(key);
                    if (dict1.ContainsKey(key))
                    {
                        dict1[key] = dict1[key] + 1;
                    }
                    else
                    {
                        dict1[key] = 1;
                    }
                }
            }

            for (int i = 0; i < token2.Length; i++)
            {
                var key = token2[i]/*.SansAccent()*/.Trim();
                if (key.IsNotNullOrEmptyString())
                {
                    allWords.Add(key);
                    if (dict2.ContainsKey(key))
                    {
                        dict2[key] = dict2[key] + 1;
                    }
                    else
                    {
                        dict2[key] = 1;
                    }
                }
            }

            double commonWord = 0;
            double totalWord = 0;
            foreach (var key in allWords)
            {
                if (dict1.ContainsKey(key)) totalWord += dict1[key];
                if (dict2.ContainsKey(key)) totalWord += dict2[key];


                if (dict1.ContainsKey(key) && dict2.ContainsKey(key))
                {
                    commonWord += dict1[key] + dict2[key];
                }
            }

            var infos = new SimilarityInfos
            {
                CommonWord = commonWord,
                TotalWord = totalWord,
                AllWords = allWords,
                Dictionary1 = dict1,
                Dictionary2 = dict2
            };
            infos.TauxDeSimilarite = commonWord / totalWord;
            return infos;
        }
    }
}
