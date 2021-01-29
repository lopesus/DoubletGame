using System;
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

        public int SpaceBehind { get; set; }
        public int SpaceInFront { get; set; }

        public CrossWordWord ParentWord { get; set; }

        public bool ExcludedFromMaze { get; set; }
        public CrossWordCell(Coord coord)
        {
            this.Coord = coord;
            IsEmpty = true;
        }

        public CrossWordCell( char car, Coord cellCoord, CrossWordDirection direction, CrossWordWord crossWordWord)
        {
            Letter = car.ToString();
            Coord = cellCoord;
            Direction = direction;
            ParentWord = crossWordWord;
            IsEmpty = false;
        }

        public void CopyFrom(CrossWordCell from)
        {
            Letter = from.Letter;
            IsEmpty = from.IsEmpty;
            Direction = from.Direction;
            ParentWord = from.ParentWord;
        }
    }
}