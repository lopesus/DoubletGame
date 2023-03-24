using CommonLibTools.Libs;
using MongoDB.Bson.Serialization.Attributes;

namespace WiktionaireParser.Models
{
    public class WordFrequency
    {
        public WordFrequency(string word, long count, long totalCount)
        {
            Key = word;
            Count = count;
            AllWordCount = totalCount;
            if (totalCount == 0)
            {
                Frequency = 0;
            }
            else
            {
                Frequency = Count*100 / (float)AllWordCount;
            }

            ZipfFrequency = LetterFrequency.GetZipfFrequency(Count, AllWordCount);
        }

        public override string ToString()
        {
            return $"{Key} - {Count}";
        }

        [BsonId]
        public string Key { get; set; }
        public long Count { get; set; }
        public long AllWordCount { get; set; }
        public float Frequency { get; set; }
        public double ZipfFrequency { get; set; }
    }
}