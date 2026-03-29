namespace ncopy
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var source = "D:\\Workspace\\learn-aspnet\\Part7StreamAndFile\\testFile\\test.txt";
            
            var dest = "D:\\Workspace\\learn-aspnet\\Part7StreamAndFile\\testFile\\test-copy.txt";

            var buffer = new byte[1024];
            using var instream = File.OpenRead(source);
            using var outstream = File.OpenWrite(dest);   

            int n = instream.Read(buffer, 0, buffer.Length);
            while (n > 0)
            {
                Console.WriteLine(n.ToString());

                outstream.Write(buffer, 0, n);

                n = instream.Read(buffer, 0, buffer.Length);
            }

            //instream.Close();
            //outstream.Close();
        }
    }
}
