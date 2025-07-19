
namespace NFind
{
    public class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("FIND: Parameter format not correct");
                return;
            }

            var fileOptions = BuildOptions(args);
        }

        public static object BuildOptions(string[] args)
        {
            throw new NotImplementedException();
        }
    }
}
