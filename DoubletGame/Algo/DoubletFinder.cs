using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonLibTools;
using CommonLibTools.Libs;

namespace DoubletGame.Algo
{
    public static class DoubletTools
    {
        
    }

    public class DoubletFinder
    {
        public List<string> WordList { get; set; }
        public Dictionary<string, List<string>> DoubletDico { get; set; }

        public string Alphabet = "abcdefghijklmnopqrstuvwxyz";

        public DoubletFinder(List<string> list)
        {
            WordList = list;
            DoubletDico = new Dictionary<string, List<string>>();
            foreach (var word in WordList)
            {
                DoubletDico[word] = new List<string>();
            }
        }

        public void FindDoublet()
        {
            //Parallel.ForEach(WordList, (word) =>
            //{

            //});

            foreach (var word in WordList)
            {

                for (int pos = 0; pos < word.Length; pos++)
                {
                    var pos1 = pos;
                    for (int index = 0; index < Alphabet.Length; index++)
                    {
                        var newChar = Alphabet[index];
                        var newWord = word.ReplaceAt(pos1, newChar);
                        if (newWord != word && DoubletDico.ContainsKey(newWord) == true)
                        {
                            if (word == "tuto")
                            {
                                Console.WriteLine();
                            }
                            DoubletDico[word].Add(newWord);
                        }
                    }
                }

                var bag = DoubletDico[word];
                //DoubletDico[word]=new ConcurrentBag<string>();
                Console.WriteLine($"{bag}");
            }
        }

        public List<string> GetDoubletFor(string word)
        {
            if (word != null && DoubletDico.ContainsKey(word))
            {
                return DoubletDico[word].ToList();
            }

            return null;
        }

        public new ConcurrentBag<DoubletResult> GetWordsInBetween(string source, string dest, int maxMove = 10)
        {
            var exclusionList = new Dictionary<string, bool>();
            var resultList = new ConcurrentBag<DoubletResult>();

            var result = new DoubletResult();
            Worker(source, dest, result, maxMove, exclusionList, resultList);

            return resultList;
        }

        void Worker(string source, string dest, DoubletResult result, int maxMove, Dictionary<string, bool> exclusionList,
            ConcurrentBag<DoubletResult> resultList)
        {
            var toExclude = new Dictionary<string, bool>(exclusionList);

            var adjacents = GetDoubletFor(source);

            toExclude.Add(source, false);

            result.AddWord(source);

            var remainingMove = maxMove - 1;
            if (source == "OAT" || source == "MAT")
            {
                Console.WriteLine();
            }

            ParallelOptions options = new ParallelOptions();
            //options.MaxDegreeOfParallelism = 1;

            if (adjacents != null)
                Parallel.ForEach(adjacents, options, (adjacent) =>
                {
                    DoubletResult nextResult = new DoubletResult(result);
                    if (adjacent == dest)
                    {
                        nextResult.AddWord(adjacent);
                        resultList.Add(nextResult);
                    }
                    else
                    {
                        if (remainingMove > 0)
                        {
                            if (toExclude.ContainsKey(adjacent) == false)
                            {
                                //nextResult.Add(adjacent);
                                Worker(adjacent, dest, nextResult, remainingMove, toExclude, resultList);
                            }
                        }
                    }
                });
        }
    }
}
