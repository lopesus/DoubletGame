using System.Collections.Generic;
using System.Linq;

class WordSearch
{
    public string Word { get; private set; }
    public List<string> Permutations { get; private set; }
    public List<string> AdjacentWords { get; private set; }
    public List<string> OnePositionWords { get; private set; }

    public WordSearch(string word, HashSet<string> dictionary)
    {
        Word = word;
        Permutations = WordSearchUtils.GeneratePermutations(word, dictionary);
        AdjacentWords = WordSearchUtils.GenerateAdjacentWords(word, dictionary);
        OnePositionWords = WordSearchUtils.GenerateOnePositionWords(word, dictionary);
    }
}