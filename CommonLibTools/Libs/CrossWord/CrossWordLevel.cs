using System.Collections.Generic;
using System.Text;

namespace CommonLibTools.Libs.CrossWord
{
    public class CrossWordLevel
    {

        public int R { get; set; }

        public int C { get; set; }
        public string L { get; set; }
        public List<CrossWordSimple> W { get; set; }

        public CrossWordLevel(int r, int c,string levelLeters, List<CrossWord> allWords)
        {
            R = r;
            C = c;
            L = levelLeters;
            W = new List<CrossWordSimple>();
            foreach (CrossWord crossWord in allWords)
            {
                CrossWordSimple crossWordSimple=new CrossWordSimple()
                {
                    W = crossWord.Word,
                    C = new CoordSimple(crossWord.Coord),
                    D = crossWord.Direction,
                };
                W.Add(crossWordSimple);
            }
        }

        public CrossWordLevel(GenGrid genGrid, int currentLevel)
        {
            R = genGrid.NumRow;
            C = genGrid.NumCol;
            L = genGrid.Letters;
            W = new List<CrossWordSimple>();

            foreach (CrossWord crossWord in genGrid.FitWordList)
            {
                CrossWordSimple crossWordSimple = new CrossWordSimple()
                {
                    W = crossWord.Word,
                    C = new CoordSimple(crossWord.Coord),
                    D = crossWord.Direction,
                };
                W.Add(crossWordSimple);
            }
        }

        //public override string ToString()
        //{
        //    var builder=new StringBuilder();
        //    foreach (var word in AllWords)
        //    {
        //        builder.Append($"{word}:");
        //    }
        //    return builder.ToString();
        //}
    }
}