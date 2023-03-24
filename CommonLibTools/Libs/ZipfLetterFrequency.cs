using System;

namespace CommonLibTools.Libs
{
    public static class LetterFrequency
    {
        /// <summary>
        /// frequency per millions
        /// </summary>
        /// <param name="wordCount"></param>
        /// <param name="numberOfWord"></param>
        /// <returns></returns>
        public static double GetZipfFrequency(float wordCount, long numberOfWord)
        {
            //prevent 0 to be used in log function
            wordCount += 1;
            float perMillions = numberOfWord / 1000000f;
            var millions = wordCount / perMillions;
            var log10 = Math.Log10(millions);
            var zipfFrequency = log10 + 3;
            return Math.Round(zipfFrequency,3);
        }
    }
}