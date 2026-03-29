using System;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        try
        {
            // Đọc input từ file hoặc stdin
            string jsonInput;
            if (args.Length > 0 && File.Exists(args[0]))
            {
                jsonInput = File.ReadAllText(args[0]);
            }
            else
            {
                jsonInput = Console.In.ReadToEnd();
            }
            
            // Khởi tạo field từ input
            Field field = new Field(jsonInput);
            
            // Khởi tạo và chạy solver
            Solver solver = new Solver(field);
            Solution solution = solver.Solve(maxDepth: 100, timeLimit: 290); // Giới hạn thời gian 290s để có buffer
            
            // In kết quả
            Console.WriteLine(solution.ToJson());
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error: {ex.Message}");
            Console.Error.WriteLine(ex.StackTrace);
            
            // Trả về solution rỗng trong trường hợp lỗi
            Console.WriteLine("{\"ops\":[]}");
        }
    }
}