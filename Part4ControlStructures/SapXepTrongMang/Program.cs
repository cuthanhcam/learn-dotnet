namespace SapXepTrongMang
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var rand = new Random();

            var arr = new int[10];

            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = rand.Next(1, 100);
            }

            foreach (var item in arr)
            {
                Console.Write($"{item}\t");
            }

            Console.WriteLine("\n\nSap xep mang tang dan:");

            for (int i = 0; i < arr.Length - 1; i++)
            {
                for (int j = i + 1; j < arr.Length; j++)
                {
                    if (arr[i] > arr[j])
                    {
                        var temp = arr[i];
                        arr[i] = arr[j];
                        arr[j] = temp;
                    }
                }
            }

            foreach (var item in arr)
            {
                Console.Write($"{item}\t");
            }
        }
    }
}
