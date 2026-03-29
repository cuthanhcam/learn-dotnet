namespace Part6Exception
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                try
                {
                    Console.Write("Enter a number: ");

                    int n = int.Parse(Console.ReadLine());
                    int x = 10 / n;

                    Console.WriteLine(x);
                }
                catch (DivideByZeroException ex)
                {
                    Console.WriteLine("Loi chia cho 0");
                    throw new Exception("This is a custom exception message.");
                }
                catch (FormatException ex)
                {
                    Console.WriteLine("Loi dinh dang, ban phai nhap vao mot so nguyen");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                finally
                {
                    Console.WriteLine("Ket thuc chuong trinh");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
