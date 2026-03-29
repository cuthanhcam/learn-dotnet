namespace Part3Array
{
    internal class Program
    {
        static void Main()
        {
            Array1();
            Array2();
            Array3();
            Array4();
        }

        static void Array1()
        {
            int[] arr = { 1, 2, 3, 4, 5 };
            Console.WriteLine($"Length of arr: {arr.Length}");
            for (int i = 0; i < arr.Length; i++)
            {
                Console.WriteLine(arr[i]);
            }
        }

        static void Array2()
        {
            int[,] array2D =
            {
                { 1, 2, 3 },
                { 4, 5, 6 },
                { 7, 8, 9 }
            };

            Console.WriteLine($"Length of array2D: {array2D.Length}");

            foreach (var item in array2D)
            {
                Console.WriteLine(item);
            }
        }

        static void Array3()
        {
            int[,,] array3D =
            {
                {
                    { 1, 2, 3 },
                    { 4, 5, 6 }
                },
                {
                    { 7, 8, 9 },
                    { 10, 11, 12 }
                }
            };

            Console.WriteLine($"Length of array3D: {array3D.Length}");
        }

        static void Array4()
        {
            int[][] jaggedArray = new int[3][];
            jaggedArray[0] = new int[] { 1, 2, 3 };
            jaggedArray[1] = new int[] { 4, 5 };
            jaggedArray[2] = new int[] { 6, 7, 8, 9 };
            Console.WriteLine($"Length of jaggedArray: {jaggedArray.Length}");
            foreach (var subArray in jaggedArray)
            {
                Console.WriteLine($"Sub-array length: {subArray.Length}");
                foreach (var item in subArray)
                {
                    Console.WriteLine(item);
                }
            }
        }
    }
}
