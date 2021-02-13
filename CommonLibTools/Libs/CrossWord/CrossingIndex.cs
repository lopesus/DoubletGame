namespace CommonLibTools.Libs.CrossWord
{
    public class CrossingIndex
    {
        public int Index { get; set; }
        public CrossWordCell Anchor { get; set; }
        public CrossWordDirection Direction { get; set; }

        public Coord StartCoord { get; set; }

        public CrossingIndex(int index, CrossWordCell crossWordCell, CrossWordDirection crossWordDirection,int beforeIndex,int afterIndex)
        {
            Index = index;
            Anchor = crossWordCell;
            Direction = crossWordDirection;

            var anchorCoord = Anchor.Coord;
            switch (Direction)
            {
                case CrossWordDirection.Horizontal:
                    StartCoord= new Coord(anchorCoord.Row, anchorCoord.Col - beforeIndex);
                    break;

                case CrossWordDirection.Vertical:
                    StartCoord= new Coord(anchorCoord.Row - beforeIndex, anchorCoord.Col);
                    break;
            }

        }

        public override string ToString()
        {
            return $"ind: {Index} -  Anchor: {Anchor}";
        }
    }
}