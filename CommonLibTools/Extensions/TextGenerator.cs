using System;
using System.Collections.Generic;

namespace CommonLibTools.Extensions
{
    public static class TextGenerator
    {
        public static Random Random;

        static TextGenerator()
        {
            Random = new Random();
        }


        public static string CreateTexte(List<string> lines, int minPara = 1, int maxPara = 5)
        {
            var texte = "";
            int nombreparagraf = Random.Next(minPara, maxPara);
            for (int j = 0; j < nombreparagraf; j++)
            {
                var tmp = CreateParagraf(lines);
                texte += tmp + "\n";
            }
            return texte;
        }

        public static string CreateTexteForNewsArticle(List<string> lines, int minPara = 1, int maxPara = 5)
        {
            var texte = "";
            int nombreparagraf = Random.Next(minPara, maxPara);
            for (int j = 0; j < nombreparagraf; j++)
            {
                var tmp = CreateParagraf(lines);
                texte += "<p>" + tmp + "</p>";
            }
            return texte;
        }


        public static string CreateParagraf(List<string> lines, bool empty = false)
        {
            if (empty)
            {
                var rand = Random.Next(0, 20);
                if (rand > 15)
                {
                    return "";
                }
            }
            var paragraphes = "";

            for (int k = 0; k < Random.Next(3, 10); k++)
            {
                paragraphes = paragraphes + lines[Random.Next(0, lines.Count)] + ".";
            }
            return paragraphes;
        }

        public static string CreateTitle(List<string> lines)
        {

            var paragraphes = "";

            for (int k = 0; k < Random.Next(1, 4); k++)
            {
                paragraphes = paragraphes + lines[Random.Next(0, lines.Count)] + ".";
            }
            return paragraphes;
        }
    }
}