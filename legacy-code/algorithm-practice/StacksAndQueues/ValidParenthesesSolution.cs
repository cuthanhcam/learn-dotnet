using System.Collections.Generic;

namespace StacksAndQueues
{
    public class ValidParenthesesSolution
    {
        public bool IsValid(string s)
        {
            Stack<char> stack = new Stack<char>();

            foreach (char c in s)
            {
                if (c == '(' || c == '{' || c == '[')
                {
                    stack.Push(c);
                }
                else
                {
                    if (stack.Count == 0) return false;
                    char top = stack.Pop();
                    if ((c == ')' && top != '(') ||
                        (c == '}' && top != '{') ||
                        (c == ']' && top != '['))
                    {
                        return false;
                    }
                }
            }

            return stack.Count == 0;
        }

        // Use Dictionary to map closing brackets to opening brackets
        // private static readonly Dictionary<char, char> brackets = new Dictionary<char, char>
        // {
        //     { ')', '(' }, { '}', '{' }, { ']', '[' }
        // };
        // public bool IsValid(string s)
        // {
        //     Stack<char> stack = new Stack<char>();
        //     foreach (char c in s)
        //     {
        //         if (brackets.ContainsValue(c)) stack.Push(c);
        //         else if (brackets.ContainsKey(c) && (stack.Count == 0 || stack.Pop() != brackets[c])) return false;
        //     }
        //     return stack.Count == 0;
        // }
    }
}