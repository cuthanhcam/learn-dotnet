
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

        public static FindOptions BuildOptions(string[] args)
        {
            var options = new FindOptions();

            foreach (var arg in args)
            {
                if (arg =="/v")
                {
                    options.FindDontConstain = true;
                }
                else if (arg == "/c")
                {
                    options.CountMode = true;
                }
                else if (arg == "/n")
                {
                    options.ShowLineNumbers = true;
                }
                else if (arg == "/i")
                {
                    options.IsCaseSensitive = false;
                }
                else if (arg == "/off" || arg == "/offline")
                {
                    options.SkipOfflineFiles = false;
                }
                else if (arg == "/?")
                {
                    options.HelpMode = true;
                }
                else
                {
                    if (string.IsNullOrEmpty(options.StringToFind))
                    {
                        options.StringToFind = arg;
                    }
                    else if (string.IsNullOrEmpty(options.Path))
                    {
                        options.Path = arg;
                    }
                    else
                    {
                        throw new ArgumentException(arg);
                    }
                }
            }
            return options;
        }
    }
}
