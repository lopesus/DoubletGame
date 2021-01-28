using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Bson.Serialization.Attributes;

namespace WiktionaireParser.Models
{

    public class ValidWord
    {
        public IDictionary<int, List<string>> Dictionary { get; set; }

        public ValidWord(IDictionary<int, List<string>> allPossibleWord)
        {
            Dictionary = allPossibleWord;
        }

        public override string ToString()
        {
            var builder=new StringBuilder();
            foreach (var pair in Dictionary.OrderByDescending(p=>p.Key))
            {
                builder.AppendLine($"{pair.Key} letters words - {pair.Value.Count}");
                foreach (var word in pair.Value)
                {
                    builder.Append($"{word.ToLowerInvariant()} ");
                }

                builder.AppendLine();
                builder.AppendLine();
            }

            return builder.ToString();
        }
    }
    public class Anagram
    {
        [BsonId]
        public string Key { get; set; }

        public int Count { get; set; }
        public List<string> AnagramList { get; set; }

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