using System;
using System.Collections.Generic;
using CommonLibTools;
using PathFindingModel;

namespace WiktionaireParser.Models.CrossWord
{
    public class CrossWordCell
    {
        public Coord Coord;
        public string Letter { get; set; }
        public bool IsEmpty { get; set; }
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

        public CrossWordCell( char car, Coord cellCoord, CrossWordDirection direction, CrossWord crossWord)
        {
            Letter = car.ToString();
            Coord = cellCoord;
            Direction = direction;
            ParentWord = new List<CrossWord>();
            ParentWord.Add(crossWord);
            IsEmpty = false;
        }

        public void SetLetter(CrossWordLetter letter)
        {
            Letter = letter.Letter;
            IsEmpty = false;
            Direction = letter.Direction;
            ParentWord.Add(letter.ParentWord);
            if (ParentWord.Count>=2)
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