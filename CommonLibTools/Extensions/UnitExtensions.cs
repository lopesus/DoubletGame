namespace CommonLibTools.Extensions
{
    public  static class UnitExtensions
    {
        public static double ToPouce(this string s)
        {
            if (s.IsNullOrEmptyString())
            {
                return 0;
            }
            var tokens = s.Trim().Split(new char[] { '"' });
            double val = 0;
            double.TryParse(tokens[0], out val);
            return val;

        }

        public static double ToGigaOctet(this string s)
        {
            if (s.IsNullOrEmptyString())
            {
                return 0;
            }
            var tokens = s.Trim().Split();
            double val = 0;
            double.TryParse(tokens[0], out val);
            if (tokens.Length == 1)
            {

                return val;
            }
            else
            {
                if (tokens[1].Trim().ToLower() == "mo")
                {
                    return val / 1024;
                }
                return val;
            }
        }

        public static double ToGigaHertz(this string s)
        {
            if (s.IsNullOrEmptyString())
            {
                return 0;
            }
            var tokens = s.Trim().Split();
            double val = 0;
            double.TryParse(tokens[0], out val);
            if (tokens.Length == 1)
            {

                return val;
            }
            else
            {
                if (tokens[1].Trim().ToLower() == "mhz")
                {
                    return val / 1000;
                }
                return val;
            }
        }
    }
}
