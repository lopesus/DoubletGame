using System;
using System.Collections.Generic;
using System.Linq;

namespace CommonLibTools.Libs.Extensions
{
    public static class ListExtensions
    {
        public static Random Random { get; set; }

        static ListExtensions()
        {
            Random = new Random();
        }

        public static List<string> RemovePluralForm(this List<string> list)
        {
            Dictionary<string,bool> singular = new Dictionary<string, bool>();
            var final=new List<string>();

            foreach (var mot in list.OrderBy(w=>w.Length))
            {
                //check french plural form
                var lowerInvariant = mot.ToLowerInvariant();
                if (lowerInvariant.EndsWith("s"))
                {
                    var singularForm = lowerInvariant.TrimEnd("s");
                    if (singular.ContainsKey(singularForm) == false)
                    {
                        final.Add(mot);

                    }
                }
                else
                {
                    singular[lowerInvariant] = true;
                    final.Add(mot);
                }
            }

            return final;
        }
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = Random.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static T PickRandom<T>(this IList<T> list)
        {
            if (list != null && list.Count>0)
            {
                var index = GetRandomInt(0, list.Count);
                return list[index];
            }

            return default;
        }

        public static int GetRandomInt(int min, int max)
        {
            return Random.Next(min, max);
            //return new Random(Guid.NewGuid().ToString().GetHashCode()).Next(min, max);
        }
    }
}