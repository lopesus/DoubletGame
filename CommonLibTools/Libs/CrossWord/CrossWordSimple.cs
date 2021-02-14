namespace CommonLibTools.Libs.CrossWord
{
    public class CrossWordSimple
    {
        public CoordSimple C;
        public CrossWordDirection D { get; set; }
        public string W { get; set; }

        public override string ToString()
        {
            return $"{C.R};{C.C};{D};{W}";
        }
    }
}