namespace PART14Interface
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var reader = new DatabaseReadable();

            Run(reader);
        }

        static void Run(IReadable reader)
        {
            reader.WriteName(); // Call the extension method

            IReadable.WriteName(reader); // Call the static method

            Console.WriteLine(reader.Name);

            int n = reader.ReadInt();

            string s = reader.ReadString();

            Console.WriteLine($"Int: {n} and string: {s}");
        }
    }
}
