
using Microsoft.Data.SqlClient;

namespace PART17ADONET
{
    public class Program
    {
        static void Main(string[] args)
        {
            TestDBConnection();
        }

        public static void TestDBConnection()
        {
            using var conn = new SqlConnection("Data Source=JERK\\CAMDB;Initial Catalog=HR;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            conn.Open();    
            
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT employee_id, email FROM employees";

            using var reader = cmd.ExecuteReader();
            {
                while (reader.Read())
                {
                    Console.WriteLine($"Employee ID: {reader.GetInt32(0)}, Email: {reader.GetString(1)}");
                }
            }
           
            conn.Close();
        }
    }
}
