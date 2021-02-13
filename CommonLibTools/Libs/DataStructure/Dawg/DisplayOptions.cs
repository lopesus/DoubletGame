namespace CommonLibTools.Libs.DataStructure.Dawg
{
    public class DisplayOptions
    {
        public bool ToUppercase { get; set; }
        public bool ShowJoker { get; set; }
        public bool ShowCrossingLetter { get; set; }

        public DisplayOptions(bool toUppercase, bool showJoker, bool showCrossingLetter)
        {
            ToUppercase = toUppercase;
            ShowJoker = showJoker;
            ShowCrossingLetter = showCrossingLetter;
        }

        public DisplayOptions()
        {
            ToUppercase = true;
            ShowJoker = true;
            ShowCrossingLetter = true;
        }
    }
}