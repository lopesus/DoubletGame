using System.Collections.Generic;
using System.Linq;

namespace WiktionaireParser.Models
{
    public class AnagramBuilder
    {
        public Dictionary<string, Anagram> Dico;

        public AnagramBuilder()
        {
            Dico = new Dictionary<string, Anagram>();
        }

        public AnagramBuilder(List<Anagram> anagrams)
        {
            Dico = anagrams.ToDictionary(a => a.Key);
        }

        public void Add(string key, string word)
        {
            if (Dico.ContainsKey(key) == false)
            {
                Dico[key] = new Anagram(key);
            }

            Dico[key].AddWord(word);

        }

        public int GetCountFor(string key)
        {
            if (Dico.ContainsKey(key))
            {
                return Dico[key]?.Count ?? 0;
            }

            return 0;
        }

        public List<Anagram> GetAnagramsList()
        {
            return Dico.Values.ToList();
        }

        public Anagram GetAnagramFor(string key)
        {
            if (key != null)
            {
                if (Dico.ContainsKey(key)) return Dico[key];
            }

            return null;
        }
    }
}