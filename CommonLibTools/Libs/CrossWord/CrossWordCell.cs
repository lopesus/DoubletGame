using System;
using System.Collections.Generic;
using System.Linq;

namespace CommonLibTools.Libs.CrossWord
{
    public class CrossWordCell
    {
        public Coord Coord;
        /// <summary>
        /// translated coord repere orthonorme , repere start at 0,0, 
        /// </summary>
        public Coord OrthoCoord;
        public string Letter { get; set; }
        public string HintLetter { get; set; }
        public bool IsEmpty { get; set; }
        public bool IsRevealed { get; set; }
        public CrossWordDirection Direction { get; set; }

        public int SpaceBefore { get; set; }
        public int SpaceAfter { get; set; }

        public List<CrossWord> ParentWord { get; set; }

        public bool ExcludedFromMaze { get; set; }
        public CrossWordCell(Coord coord)
        {
            ParentWord = new List<CrossWord>();
            this.Coord = coord;
            IsEmpty = true;
            Letter = "";
        }

        public CrossWordCell(char car, Coord cellCoord, CrossWordDirection direction, CrossWord crossWord)
        {
            Letter = car.ToString();
            Coord = cellCoord;
            Direction = direction;
            ParentWord = new List<CrossWord>();
            ParentWord.Add(crossWord);
            IsEmpty = false;
        }

        public void CopyFrom(CrossWordCell cell)
        {
            Letter = cell.Letter;
            IsEmpty = cell.IsEmpty;
            Direction = cell.Direction;
            SpaceBefore = cell.SpaceBefore;
            SpaceAfter = cell.SpaceAfter;
            ExcludedFromMaze = cell.ExcludedFromMaze;
            ParentWord = cell.ParentWord.ToList();
        }
        public void SetLetter(CrossWordLetter letter)
        {
            Letter = letter.Letter;
            IsEmpty = false;
            Direction = letter.Direction;
            ParentWord.Add(letter.ParentWord);
            if (ParentWord.Count >= 2)
            {
                Console.WriteLine();
            }
        }

        public override string ToString()
        {
            return $"{Coord} -  {Letter} - {Direction}, behind:{SpaceBefore} - after:{SpaceAfter}";
        }
    }
}