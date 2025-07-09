
namespace linq2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var students = GetStudents();
            Print(students.Where(s => s.YoB == 2000));
            Console.WriteLine("----");
            Print(students.OrderByDescending(s => s.YoB).OrderBy(s => s.Name));

            foreach (var student in students.OrderBy(s => s.Name).Select(s => s.YoB))
            {
                Console.WriteLine(student);
            }
        }

        static void Print(IEnumerable<Student> students)
        {
            foreach (var student in students)
            {
                Print(student);
            }
        }

        private static void Print(Student student)
        {
            Console.WriteLine($"Name: {student.Name}, City: {student.City}, Year of Birth: {student.YoB}");
        }

        static IEnumerable<Student> GetStudents()
        {
            return new Student[]
            {
                new Student { Name = "Alice", City = "New York", YoB = 2000 },
                new Student { Name = "Bob", City = "Los Angeles", YoB = 1999 },
                new Student { Name = "Charlie", City = "Chicago", YoB = 2001 },
                new Student { Name = "David", City = "Houston", YoB = 2000 }
            };

        }
    }
}
