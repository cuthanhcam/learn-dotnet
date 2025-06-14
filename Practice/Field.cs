using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

public class Field
{
    public int Size { get; private set; }
    public int[,] Entities { get; private set; }
    
    // Constructor từ JSON input
    public Field(string jsonInput)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        
        var input = JsonSerializer.Deserialize<Input>(jsonInput, options);
        Size = input.Problem.Field.Size;
        
        // Chuyển đổi từ danh sách 2D sang mảng 2D
        Entities = new int[Size, Size];
        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                Entities[i, j] = input.Problem.Field.Entities[i][j];
            }
        }
    }
    
    // Constructor tạo bản sao
    public Field(Field other)
    {
        Size = other.Size;
        Entities = new int[Size, Size];
        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                Entities[i, j] = other.Entities[i, j];
            }
        }
    }
    
    // Đếm số lượng cặp (pairs)
    public int CountPairs()
    {
        int pairs = 0;
        
        // Đếm cặp theo hàng ngang
        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size - 1; j++)
            {
                if (Entities[i, j] == Entities[i, j + 1])
                {
                    pairs++;
                }
            }
        }
        
        // Đếm cặp theo hàng dọc
        for (int i = 0; i < Size - 1; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                if (Entities[i, j] == Entities[i + 1, j])
                {
                    pairs++;
                }
            }
        }
        
        return pairs;
    }
    
    // Thực hiện thao tác xoay
    public void Rotate(int x, int y, int n)
    {
        // Kiểm tra tính hợp lệ của thao tác
        if (x < 0 || y < 0 || x + n > Size || y + n > Size || n < 2)
        {
            throw new ArgumentException("Invalid rotation parameters");
        }
        
        // Tạo mảng tạm để lưu trữ giá trị xoay
        int[,] temp = new int[n, n];
        
        // Sao chép vào mảng tạm
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                temp[i, j] = Entities[y + i, x + j];
            }
        }
        
        // Xoay 90 độ theo chiều kim đồng hồ
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                Entities[y + i, x + j] = temp[n - 1 - j, i];
            }
        }
    }
    
    // Đánh giá ảnh hưởng của một thao tác xoay
    public int EvaluateRotation(int x, int y, int n)
    {
        // Tạo bản sao của field
        Field copy = new Field(this);
        
        // Lấy số lượng cặp trước khi xoay
        int beforePairs = copy.CountPairs();
        
        // Thực hiện xoay
        copy.Rotate(x, y, n);
        
        // Lấy số lượng cặp sau khi xoay
        int afterPairs = copy.CountPairs();
        
        // Trả về sự thay đổi
        return afterPairs - beforePairs;
    }
    
    // Lấy danh sách tất cả các thao tác xoay có thể
    public List<(int x, int y, int n)> GetAllPossibleRotations()
    {
        var rotations = new List<(int x, int y, int n)>();
        
        for (int n = 2; n <= Size; n++)
        {
            for (int i = 0; i <= Size - n; i++)
            {
                for (int j = 0; j <= Size - n; j++)
                {
                    rotations.Add((j, i, n));
                }
            }
        }
        
        return rotations;
    }
}

// Lớp hỗ trợ chuyển đổi JSON
public class Input
{
    [JsonPropertyName("startsAt")]
    public long StartsAt { get; set; }
    
    [JsonPropertyName("problem")]
    public Problem Problem { get; set; }
}

public class Problem
{
    [JsonPropertyName("field")]
    public FieldData Field { get; set; }
}

public class FieldData
{
    [JsonPropertyName("size")]
    public int Size { get; set; }
    
    [JsonPropertyName("entities")]
    public List<List<int>> Entities { get; set; }
}

public class Operation
{
    [JsonPropertyName("x")]
    public int X { get; set; }
    
    [JsonPropertyName("y")]
    public int Y { get; set; }
    
    [JsonPropertyName("n")]
    public int N { get; set; }
}

public class Solution
{
    [JsonPropertyName("ops")]
    public List<Operation> Operations { get; set; } = new List<Operation>();
    
    public string ToJson()
    {
        return JsonSerializer.Serialize(this);
    }
}