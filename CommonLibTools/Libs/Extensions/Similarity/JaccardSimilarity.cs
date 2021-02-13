using System.Collections.Generic;
using System.Linq;

namespace CommonLibTools.Libs.Extensions.Similarity
{
    public  class JaccardSimilarity
    {
        public static double Similarity<T>(HashSet<T> set1, HashSet<T> set2)
        {
            int intersectionCount = set1.Intersect(set2).Count();
            int unionCount = set1.Union(set2).Count();

            return (1.0 * intersectionCount) / unionCount;
        }
    }
}