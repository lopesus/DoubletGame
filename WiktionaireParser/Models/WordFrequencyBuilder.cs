using System;
using System.Collections.Generic;
using System.Windows.Media.Animation;
using CommonLibTools;
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
                Frequency = Count / (float)AllWordCount;
            }

        }

        [BsonId]
        public string Key { get; set; }
        public long Count { get; set; }
        public long AllWordCount { get; set; }
        public float Frequency { get; set; }
    }
    public class WordFrequencyBuilder
    {
        public Dictionary<string, long> WordDico { get; set; }
        public Dictionary<char, long> LetterDico { get; set; }
        public long AllWordCount { get; set; }
        public LoaderOptimization LetterCount { get; set; }
        public long MostFrequentWordCount { get; set; }

        //stats
        public float MaxFrequency { get; set; }
        public float MinFrequency { get; set; }
        public float AverageFrequency { get; set; }

        public WordFrequencyBuilder()
        {
            WordDico = new Dictionary<string, long>();
            LetterDico = new Dictionary<char, long>();
        }

        public void AddWord(string word)
        {
            if (word == null) return;
            if (WordDico.ContainsKey(word))
            {
                WordDico[word] += 1;
            }
            else
            {
                WordDico[word] = 1;
            }

            AllWordCount++;

            if (WordDico[word] > MostFrequentWordCount)
            {
                MostFrequentWordCount = WordDico[word];
            }

            //count letter 
            foreach (var car in word.ToCharArray())
            {
                if (LetterDico.ContainsKey(car))
                {
                    LetterDico[car] += 1;
                }
                else
                {
                    LetterDico[car] = 1;
                }
                LetterCount++;
            }
        }

        public WordFrequency GetWordFrequency(string word)
        {
            if (word.IsEmptyString() == false && WordDico.ContainsKey(word))
            {
                var val = WordDico[word];

                return new WordFrequency(word, val, AllWordCount);
            }
            else
            {
                return null;
            }
        }

        public void CheckAllWord(Dictionary<string, bool> correctWordDico)
        {
            List<Tuple<string, long>> toRemove = new List<Tuple<string, long>>();
            foreach (var pair in WordDico)
            {
                if (correctWordDico.ContainsKey(pair.Key) == false)
                {
                    toRemove.Add(new Tuple<string, long>(pair.Key, pair.Value));
                }
            }

            //remove them
            foreach (var tuple in toRemove)
            {
                WordDico.Remove(tuple.Item1);
                AllWordCount -= tuple.Item2;
            }

            MostFrequentWordCount = 0;

            foreach (var value in WordDico.Values)
            {
                if (value > MostFrequentWordCount)
                {
                    MostFrequentWordCount = value;
                }
            }
        }
    }
}