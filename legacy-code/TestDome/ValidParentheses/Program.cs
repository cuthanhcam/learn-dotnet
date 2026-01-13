namespace ValidParentheses
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string input = "({[]})";
            Console.WriteLine(IsValidParentheses(input));
        }

        public static bool IsValidParentheses(string s)
        {
            Stack<char> stack = new Stack<char>();
            var map = new Dictionary<char, char>
            {
                { ')', '(' },
                { '}', '{' },
                { ']', '['}
            };

            foreach (var c in s)
            {
                if (map.ContainsKey(c))
                {
                    if (stack.Count == 0 || stack.Pop() != map[c])
                    {
                        return false;
                    }
                }
                else
                {
                    stack.Push(c);
                }
            }
            return true;
        }
    }
}
