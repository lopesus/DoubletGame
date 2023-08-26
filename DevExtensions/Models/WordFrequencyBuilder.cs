using System;
using System.Collections.Generic;
using System.Linq;
using CommonLibTools;
using CommonLibTools.Libs;

namespace WiktionaireParser.Models
{
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

        public void AddWord(string word,int count)
        {
            if (word == null) return;
            if (WordDico.ContainsKey(word))
            {
                WordDico[word] += count;
            }
            else
            {
                WordDico[word] = count;
            }

            AllWordCount+=count;

            if (WordDico[word] > MostFrequentWordCount)
            {
                MostFrequentWordCount = WordDico[word];
            }
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
            //foreach (var car in word.ToCharArray())
            //{
            //    if (LetterDico.ContainsKey(car))
            //    {
            //        LetterDico[car] += 1;
            //    }
            //    else
            //    {
            //        LetterDico[car] = 1;
            //    }
            //    LetterCount++;
            //}
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

        public List<WordFrequency> GetFrequencyLists()
        {
            var result=new List<WordFrequency>();
            foreach (var pair in WordDico)
            {
                result.Add(new WordFrequency(pair.Key,pair.Value,AllWordCount));
            }

            return result.OrderByDescending(f=>f.Count).ToList();
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