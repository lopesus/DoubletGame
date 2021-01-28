namespace CommonLibTools.DataStructure.Dawg
{
    public class Range
    {
        public int MinVal { get; set; }
        public int MaxVal { get; set; }

        public Range()
        {
            MinVal = int.MinValue;
            MaxVal = int.MaxValue;
        }

        public Range(int minVal, int maxVal)
        {
            MinVal = minVal;
            MaxVal = maxVal;
            CheckRangeValues();
        }

        public bool IsInRange(int number)
        {
            return number >= MinVal && number <= MaxVal;
        }

        public void CheckRangeValues()
        {
            if (MinVal > MaxVal)
            {
                var temp = MaxVal;
                MaxVal = MinVal;
                MinVal = temp;
            }
        }

        public override string ToString()
        {
            return string.Format("({0},{1})", MinVal, MaxVal);
        }
    }
}