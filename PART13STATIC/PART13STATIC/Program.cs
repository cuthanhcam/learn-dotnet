namespace PART13STATIC
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Person person = new Person
            {
                Id = 1,
                Name = "John Doe"
            };

            person.Print();
        }
    }
}
