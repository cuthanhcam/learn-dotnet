namespace Part4ControlStructures
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<string> lists = new List<string> { "apple", "banana", "cherry" };
           
            foreach (var item in lists)
            {
                Console.WriteLine(item);
            }

            var lists2 = new List<string>();
            foreach (var item in lists)
            {
                lists2.Add(item.ToUpper());
            }

            foreach (var item in lists2)
            {
                Console.WriteLine(item);
            }
        }
    }
}
