namespace CommonLibTools.Libs
{
    public static class GenericUtils
    {
        public static bool IsEven(this int num)
        {
            return num % 2 == 0;
        }

        public static (int quotient, int reste) IntegerDivision(this int num1, int num2)
        {
            var quot = num1 / num2;
            var remainder = num1 % num2;

            return (quot, remainder);
        }
    }
}