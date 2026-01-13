namespace FirstUniqueChar
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string input = "aabbcdde";
            Console.WriteLine(FirstUniqueChar(input));
        }

        public static char FirstUniqueChar(string s)
        {
            var charCount = new Dictionary<char, int>();

            foreach (var c in s)
            {
                if (charCount.ContainsKey(c))
                {
                    charCount[c]++;
                }
                else
                {
                    charCount[c] = 1;
                }
            }

            foreach (var c in s)
            {
                if (charCount[c] == 1)
                {
                    return c;
                }
            }

            return '_';
        }
    }
}
