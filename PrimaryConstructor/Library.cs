using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualBasic;

namespace PrimaryConstructor
{
    public class Library
    {
        private List<Book> books = new();

        public void AddBook(Book book) => books.Add(book);

        public void DisplayAllBooks()
        {
            if (books.Count == 0)
            {
                Console.WriteLine("No books in the library.");
                return;
            }
            Console.WriteLine("Books in the library:");
            foreach (var book in books)
            {
                book.PrintInfo();
            }
        }

        public void SearchByTitle(string keyword)
        {
            var results = books
                .Where(b => b.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (results.Count == 0)
            {
                Console.WriteLine($"No books found with title containing '{keyword}'.");
                return;
            }

            Console.WriteLine($"Books found with title containing '{keyword}':");
            foreach (var book in results)
            {
                book.PrintInfo();
            }
        }

        public void SearchByAuthor(string keyword)
        {
            var results = books
                .Where(b => b.Author.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (results.Count == 0)
            {
                Console.WriteLine($"No books found by author containing '{keyword}'.");
                return;
            }

            Console.WriteLine($"Books found by author containing '{keyword}':");
            foreach (var book in results)
            {
                book.PrintInfo();
            }
        }

        public void BorrowBook(string title)
        {
            var book = books.FirstOrDefault(b =>
                b.Title.Equals(title, StringComparison.OrdinalIgnoreCase));

            if (book == null)
            {
                Console.WriteLine($"Book '{title}' not found.");
                return;
            }

            if (!book.IsAvailable)
            {
                Console.WriteLine($"Book '{title}' is already borrowed.");
                return;
            }

            book.IsAvailable = false;
            Console.WriteLine($"You have borrowed '{title}'.");
        }    

        public void ReturnBook(string title)
        {
            var book = books.FirstOrDefault(b =>
                b.Title.Equals(title, StringComparison.OrdinalIgnoreCase));

            if (book == null)
            {
                Console.WriteLine($"Book '{title}' not found.");
                return;
            }

            if (book.IsAvailable)
            {
                Console.WriteLine($"Book '{title}' is not borrowed.");
                return;
            }

            book.IsAvailable = true;
            Console.WriteLine($"You have returned '{title}'.");
        }
    }
}