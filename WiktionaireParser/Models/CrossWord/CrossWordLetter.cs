using CommonLibTools;
using PathFindingModel;

namespace WiktionaireParser.Models.CrossWord
{
    public class CrossWordLetter
    {
        public Coord Coord;
        public string Letter { get; set; }
        public CrossWordDirection Direction { get; set; }
        public CrossWord ParentWord { get; set; }


        public CrossWordLetter(char car, Coord cellCoord, CrossWordDirection direction, CrossWord crossWord)
        {
            Letter = car.ToString();
            Coord = cellCoord;
            Direction = direction;
            ParentWord = crossWord;
        }
    }
}