using System.Collections.Generic;
using System.Linq;

namespace WiktionaireParser.Models
{
    public class AnagramBuilder
    {
        private Dictionary<string, Anagram> dico;

        public AnagramBuilder()
        {
            dico = new Dictionary<string, Anagram>();
        }

        public void Add(string key, string word)
        {
            if (dico.ContainsKey(key) == false)
            {
                dico[key] = new Anagram(key);
            }

            dico[key].AddWord(word);

        }

        public int GetCountFor(string key)
        {
            if (dico.ContainsKey(key))
            {
                return dico[key]?.Count ?? 0;
            }

            return 0;
        }

        public List<Anagram> GetAnagramsList()
        {
            return dico.Values.ToList();
        }
    }
}