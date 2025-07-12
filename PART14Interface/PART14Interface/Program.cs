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
            Console.WriteLine(reader.Name);

            int n = reader.ReadInt();

            string s = reader.ReadString();

            Console.WriteLine($"Int: {n} and string: {s}");
        }
    }
}
