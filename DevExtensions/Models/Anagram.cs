using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace WiktionaireParser.Models
{
    public class Anagram
    {
        [BsonId]
        public string Key { get; set; }

        public int Count { get; set; }
        public List<string> AnagramList { get; set; }=new List<string>();

        private Dictionary<string, bool> existDico;

        public Anagram(string key)
        {
            Key = key;
            AnagramList = new List<string>();
            existDico=new Dictionary<string, bool>();
        }

        public void AddWord(string word)
        {
            if (existDico.ContainsKey(word)==false)
            {
                existDico[word] = true;
                AnagramList.Add(word);
                Count = AnagramList.Count;
            }
            
        }
    }
}