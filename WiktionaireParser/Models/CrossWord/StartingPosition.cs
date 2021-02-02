using CommonLibTools;
using PathFindingModel;

namespace WiktionaireParser.Models.CrossWord
{
    public class StartingPosition
    {
        public Coord Coord { get; set; }
        public CrossWordDirection Direction { get; set; }

        public StartingPosition(Coord coord, CrossWordDirection direction)
        {
            Coord = coord;
            Direction = direction;
        }
    }
}