using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreDotNet.Examples.Generics
{
    public static class GenericsExample
    {
        public static void Run()
        {
            Console.WriteLine($"{new String('=', 5)} Generics Example {new String('=', 5)}");

            PrintSection("GENERIC CLASS BASICS");
            DemoGenericClass();
        }

        public static void DemoGenericClass()
        {
            // Generic container for any type
            var stringBox = new Box<String> { Value = "Charlie" };
            var intBox = new Box<int> { Value = 13 };
            var listBox = new Box<List<String>> { Value = new List<String> { "a", "b", "c" } };

            Console.WriteLine($"String box: {stringBox.Value}");
            Console.WriteLine($"Int box: {intBox.Value}");
            Console.WriteLine($"List box count: {listBox.Value?.Count ?? 0}");

            var stack = new GenericStack<int>();
            stack.Push(10);
            stack.Push(20);
            stack.Push(30);
            Console.WriteLine($"Stack peek: {stack.Peek()}, Pop: {stack.Pop()}");
        }

        private static void PrintSection(string title)
        {
            Console.WriteLine();
            Console.WriteLine($"--- {title} ---");
        }
    }

    public class Box<T>
    {
        public T? Value { get; set; }
    }

    public class GenericStack<T>
    {
        private readonly List<T> _items = new();

        public void Push(T item) => _items.Add(item);

        public T Pop()
        {
            if (_items.Count == 0)
            {
                throw new InvalidOperationException("Stack is empty");
            }

            var item = _items[_items.Count - 1];
            _items.RemoveAt(_items.Count - 1);

            return item;
        }

        public T Peek()
        {
            if (_items.Count == 0)
            {
                throw new InvalidOperationException("Stack is empty");
            }

            return _items[_items.Count - 1];
        }
    }
}
