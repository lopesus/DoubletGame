using System;

namespace CommonLibTools.Libs
{
    /// <summary>
    /// y axis goes down, x axis right
    /// </summary>
    public class CoordSimple 
    {
        public int R;
        public int C;

        public CoordSimple(int r, int c)
        {
            R = r;
            C = c;
        }

        public CoordSimple(Coord coord)
        {
            R = coord.Row;
            C = coord.Col;
        }
    }
}