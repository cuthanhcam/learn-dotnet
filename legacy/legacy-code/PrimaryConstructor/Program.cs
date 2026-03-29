using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrimaryConstructor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var library = new Library();

            // Mock data for testing
            library.AddBook(new Book("Clean Code", "Robert C. Martin", 2008));
            library.AddBook(new Book("The Pragmatic Programmer", "Andrew Hunt", 1999));
            library.AddBook(new Book("Design Patterns", "Erich Gamma", 1994));
            library.AddBook(new Book("Domain-Driven Design", "Eric Evans", 2003));
            library.AddBook(new Book("Refactoring", "Martin Fowler", 1999));
            library.AddBook(new Book("Introduction to Algorithms", "Thomas H. Cormen", 2009));
            library.AddBook(new Book("Head First Design Patterns", "Eric Freeman", 2004));

            bool running = true;

            while (running)
            {
                Console.WriteLine("Library Menu:");
                Console.WriteLine("1. List all books");
                Console.WriteLine("2. Search for a book");
                Console.WriteLine("3. Search by author");
                Console.WriteLine("4. Add a book");
                Console.WriteLine("5. Mượn sách");
                Console.WriteLine("6. Trả sách");
                Console.WriteLine("0. Exit");
                Console.Write("Choose an option: ");

                string? input = Console.ReadLine();
                Console.WriteLine();

                switch (input)
                {
                    case "1":
                        library.DisplayAllBooks();
                        break;
                    case "2":
                        Console.Write("Enter title keyword: ");
                        string? titleKeyword = Console.ReadLine();
                        library.SearchByTitle(titleKeyword ?? "");
                        break;
                    case "3":
                        Console.Write("Enter author keyword: ");
                        string? authorKeyword = Console.ReadLine();
                        library.SearchByAuthor(authorKeyword ?? "");
                        break;
                    case "4":
                        Console.Write("Enter book title: ");
                        string? newTitle = Console.ReadLine();

                        Console.Write("Enter author name: ");
                        string? newAuthor = Console.ReadLine();

                        Console.Write("Enter publication year: ");
                        bool isValidYear = int.TryParse(Console.ReadLine(), out int newYear);

                        if (!string.IsNullOrEmpty(newTitle) && !string.IsNullOrEmpty(newAuthor) && isValidYear)
                        {
                            library.AddBook(new Book(newTitle, newAuthor, newYear));
                            Console.WriteLine("Book added successfully.");
                        }
                        else
                        {
                            Console.WriteLine("Invalid input. Please try again.");
                        }
                        break;
                    case "5":
                        Console.Write("Enter the title of the book to borrow: ");
                        string? borrowTitle = Console.ReadLine();
                        library.BorrowBook(borrowTitle ?? "");
                        break;

                    case "6":
                        Console.Write("Enter the title of the book to return: ");
                        string? returnTitle = Console.ReadLine();
                        library.ReturnBook(returnTitle ?? "");
                        break;
                    case "0":
                        running = false;
                        Console.WriteLine("Exiting the program.");
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
        }
    }
}