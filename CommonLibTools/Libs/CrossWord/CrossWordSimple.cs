using Newtonsoft.Json;

namespace CommonLibTools.Libs.CrossWord
{
    public class CrossWordSimple
    {
        [JsonProperty(PropertyName = "C")]
        public CoordSimple Coord;

        [JsonProperty(PropertyName = "D")]

        public CrossWordDirection Direction { get; set; }

        [JsonProperty(PropertyName = "W")]

        public string Word { get; set; }

        public CrossWordSimple()
        {
            
        }
        public override string ToString()
        {
            return $"{Coord.R};{Coord.C};{Direction};{Word}";
        }
    }
}