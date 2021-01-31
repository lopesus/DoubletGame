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
            Letter = "";
        }

        public CrossWordCell( char car, Coord cellCoord, CrossWordDirection direction, CrossWordWord crossWordWord)
        {
            Letter = car.ToString();
            Coord = cellCoord;
            Direction = direction;
            ParentWord = crossWordWord;
            IsEmpty = false;
        }

        public void CopyFrom(CrossWordCell fromCell)
        {
            Letter = fromCell.Letter;
            IsEmpty = fromCell.IsEmpty;
            Direction = fromCell.Direction;
            ParentWord = fromCell.ParentWord;
            ExcludedFromMaze = fromCell.ExcludedFromMaze;

            SpaceBehind = fromCell.SpaceBehind;
            SpaceInFront = fromCell.SpaceInFront;
            
        }

        public override string ToString()
        {
            return $"{Coord} -  {Letter} - {Direction}, behind:{SpaceBehind} - after:{SpaceInFront}";
        }
    }
}