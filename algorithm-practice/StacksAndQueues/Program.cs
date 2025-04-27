using System;

namespace StacksAndQueues
{
    class Program
    {
        static void Main(string[] args)
        {
            // Test ValidParentheses
            var validParentheses = new ValidParenthesesSolution();
            Console.WriteLine("Valid Parentheses: " + validParentheses.IsValid("()[]{}")); // Output: True
            Console.WriteLine("Valid Parentheses: " + validParentheses.IsValid("(]")); // Output: False

            // Test MyQueue
            MyQueue queue = new MyQueue();
            queue.Push(1);
            queue.Push(2);
            Console.WriteLine("Queue Peek: " + queue.Peek()); // Output: 1
            Console.WriteLine("Queue Pop: " + queue.Pop()); // Output: 1
            Console.WriteLine("Queue Empty: " + queue.Empty()); // Output: False
        }
    }
}