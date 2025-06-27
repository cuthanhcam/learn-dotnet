
namespace Part8Collection
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var list = new List<string>();

            ReadString(list);
            Print(list);
            Sort(list);
            Print(list);
        }

        private static void Sort(List<string> list)
        {
            for (int i = 0 ; i < list.Count - 1; i++)
            {
                for (int j = i + 1; j < list.Count; j++)
                {
                    if (list[i].CompareTo(list[j]) > 0)
                    {
                        var temp = list[i];
                        list[i] = list[j];
                        list[j] = temp;
                    }

                }
            }
        }

        private static void Print(List<string> list)
        {
            foreach (var item in list)
            {
                Console.WriteLine(item);
            }

            Console.WriteLine();
        }

        private static void ReadString(List<string> list)
        {
            string? s;
            do
            {
                s = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(s))
                {
                    list.Add(s);
                }
            } while (!string.IsNullOrWhiteSpace(s));
        }
    }
}
