namespace CommonLibTools.Libs.CrossWord
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