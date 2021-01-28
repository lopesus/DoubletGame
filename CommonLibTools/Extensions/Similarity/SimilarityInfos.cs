using System.Collections.Generic;

namespace CommonLibTools.Extensions.Similarity
{
    public class SimilarityInfos
    {
        public double TauxDeSimilarite { get; set; }
        public double CommonWord { get; set; }
        public double TotalWord { get; set; }
        public HashSet<string> AllWords { get; set; }
        public Dictionary<string, int> Dictionary1 { get; set; }
        public Dictionary<string, int> Dictionary2 { get; set; }

        public override string ToString()
        {
            return string.Format("TauxDeSimilarite: {0}\nTotalWords: {1}\nCommonWord: {2}", TauxDeSimilarite, TotalWord, CommonWord);
        }
    }
}