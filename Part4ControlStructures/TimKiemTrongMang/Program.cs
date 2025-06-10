namespace TimKiemTrongMang
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var rand = new Random();
            int i = 0;
            var arr = new int[10];

            for (i = 0; i < arr.Length; i++)
            {
                arr[i] = rand.Next() % 1000;
            }
            
            foreach (var item in arr)
            {
                Console.Write($"{item}\t");
            }

            Console.WriteLine();
            Console.Write("Nhap so can tim: ");
            var s = Console.ReadLine();
            int n = int.Parse(s);

            i = 0;
            while (i < arr.Length && n != arr[i])
            {
                i++;
            }

            if (i < arr.Length)
            {
                Console.WriteLine($"Tim thay {n} o vi tri {i}");
            }
            else
            {
                Console.WriteLine($"Khong tim thay {n} trong mang");
            }
        }
    }
}
