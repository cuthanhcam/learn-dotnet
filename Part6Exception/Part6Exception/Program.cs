namespace Part6Exception
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.Write("Enter a number: ");

                int n = int.Parse(Console.ReadLine());
                int x = 10 / n;

                Console.WriteLine(x);
                throw new Exception("This is a custom exception message.");
            }
            catch (DivideByZeroException ex)
            {
                Console.WriteLine("Loi chia cho 0");
            }
            catch (FormatException ex)
            {
                Console.WriteLine("Loi dinh dang");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Loi khac: " + ex.Message);
            }
            finally
            {
                Console.WriteLine("Ket thuc chuong trinh");
            }
        }
    }
}
