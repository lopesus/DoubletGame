using System.Collections.Generic;

namespace WiktionaireParser.Models
{
    public class WordBoxWord
    {
        public string Word { get; set; }
        public int Frequency { get; set; }
        public List<string> AllPossibleWord { get; set; }
    }
}