using System;

namespace CommonLibTools.Libs
{
    /// <summary>
    /// y axis goes down, x axis right
    /// </summary>
    public class Coord : IEquatable<Coord>
    {
        public int Row;
        public int Col;





        public Coord(int row, int col)
        {
            Row = row;
            Col = col;
        }

        public Coord Add(int x, int y)
        {
            return new Coord(Col + x, Row + y);
        }

        public Coord Copy()
        {
            return new Coord(Row, Col);
        }
        public override string ToString()
        {
            return string.Format("R:{0} C:{1}", Row, Col);
        }
        /// <summary>
        /// this is the euclidian distance without the sqrt operation,  for perf reason
        /// </summary>
        /// <returns></returns>
        public int GetSimpleDistance()
        {
            return Col * Col + Row * Row;
        }
        public static float Distance(Coord start, Coord end)
        {
            var xx = Math.Pow(start.Col - end.Col, 2);
            var yy = Math.Pow(start.Row - end.Row, 2);
            return (float)Math.Sqrt(xx + yy);
        }

        public static bool OnSameLine(Coord start, Coord end)
        {
            if (start.Col == end.Col) return true;
            if (start.Row == end.Row) return true;
            return false;
        }

        ////https://www.kartable.fr/ressources/mathematiques/methode/determiner-les-coordonnees-du-symetrique-d-un-point-par-rapport-a-un-autre/3538
        //public static Coord Symetrique(Coord a, Coord i)
        //{
        //    var x = 2 * i.Col - a.Col;
        //    var y = 2 * i.Row - a.Row;
        //    return new Coord(x, y);
        //}

        public Coord GetNextCoord(CrossWordDirection direction)
        {
            switch (direction)
            {
                case CrossWordDirection.Horizontal:
                    return GetRightCoord();
                case CrossWordDirection.Vertical:
                    return GetDownCoord();
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        } 
        
        public Coord GetPreviousCoord(CrossWordDirection direction)
        {
            switch (direction)
            {
                case CrossWordDirection.Horizontal:
                    return GetLeftCoord();
                case CrossWordDirection.Vertical:
                    return GetUpCoord();
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }
        public Coord GetUpCoord()
        {
            return new Coord(this.Row - 1, this.Col);
        }

        public Coord GetDownCoord()
        {
            return new Coord(this.Row + 1, this.Col);
        }

        public Coord GetLeftCoord()
        {
            return new Coord(this.Row, this.Col - 1);
        }
        public Coord GetRightCoord()
        {
            return new Coord(this.Row, this.Col + 1);
        }
        public Coord GetDiag1Coord()
        {
            return new Coord(this.Row - 1, this.Col - 1);
        }

        public Coord GetDiag2Coord()
        {
            return new Coord(this.Row - 1, this.Col + 1);
        }
        public Coord GetDiag3Coord()
        {
            return new Coord(this.Row + 1, this.Col + 1);
        }
        public Coord GetDiag4Coord()
        {
            return new Coord(this.Row + 1, this.Col - 1);
        }


        public bool Equals(Coord other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Col == other.Col && Row == other.Row;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Coord)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Col * 397) ^ Row;
            }
        }
    }
}