using System;

namespace CommonLibTools.Libs
{
    public static class LetterFrequency
    {
        public static double GetZipfFrequency(float wordCount, long numberOfWord)
        {
            //prevent 0 to be used in log function
            wordCount += 1;
            float perMillions = numberOfWord / 1000000f;
            return Math.Log10(wordCount / perMillions) + 3;
        }
    }
}