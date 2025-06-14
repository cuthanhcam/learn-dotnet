using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Concurrent;

public class Solver
{
    private Field _field;
    private Solution _bestSolution;
    private int _bestPairs;
    private int _initialPairs;
    private Dictionary<string, int> _memoization = new Dictionary<string, int>();
    
    public Solver(Field field)
    {
        _field = field;
        _bestSolution = new Solution();
        _initialPairs = field.CountPairs();
        _bestPairs = _initialPairs;
    }
    
    // Chuyển trạng thái lưới thành chuỗi để lưu vào bộ nhớ đệm
    private string GetFieldState(Field field)
    {
        var state = new char[field.Size * field.Size];
        int index = 0;
        
        for (int i = 0; i < field.Size; i++)
        {
            for (int j = 0; j < field.Size; j++)
            {
                state[index++] = (char)('0' + field.Entities[i, j]);
            }
        }
        
        return new string(state);
    }
    
    // Hàm chính để giải bài toán
    public Solution Solve(int maxDepth = 10, int timeLimit = 290)
    {
        DateTime startTime = DateTime.Now;
        
        // Thử nghiệm với chiến lược tham lam
        GreedySearch(maxDepth);
        
        // Nếu còn thời gian, thử tìm kiếm cục bộ ngẫu nhiên
        TimeSpan elapsed = DateTime.Now - startTime;
        if (elapsed.TotalSeconds < timeLimit)
        {
            LocalSearchWithRestart(maxDepth, timeLimit - (int)elapsed.TotalSeconds);
        }
        
        return _bestSolution;
    }
    
    // Tìm kiếm tham lam - luôn chọn thao tác tốt nhất tại mỗi bước
    private void GreedySearch(int maxDepth)
    {
        Field currentField = new Field(_field);
        Solution currentSolution = new Solution();
        int currentPairs = _initialPairs;
        
        for (int depth = 0; depth < maxDepth; depth++)
        {
            var bestRotation = FindBestRotation(currentField);
            
            // Nếu không có thao tác nào cải thiện, dừng lại
            if (bestRotation.improvement <= 0)
                break;
            
            // Thực hiện thao tác tốt nhất
            currentField.Rotate(bestRotation.x, bestRotation.y, bestRotation.n);
            currentSolution.Operations.Add(new Operation 
            { 
                X = bestRotation.x, 
                Y = bestRotation.y, 
                N = bestRotation.n 
            });
            
            currentPairs += bestRotation.improvement;
            
            // Cập nhật giải pháp tốt nhất nếu cần
            if (currentPairs > _bestPairs)
            {
                _bestPairs = currentPairs;
                _bestSolution = new Solution
                {
                    Operations = currentSolution.Operations.ToList()
                };
            }
        }
    }
    
    // Tìm thao tác xoay tốt nhất tại mỗi bước
    private (int x, int y, int n, int improvement) FindBestRotation(Field field)
    {
        var bestRotation = (x: -1, y: -1, n: -1, improvement: int.MinValue);
        var possibleRotations = field.GetAllPossibleRotations();
        
        // Sử dụng Parallel.ForEach để đánh giá các thao tác song song
        var results = new ConcurrentBag<(int x, int y, int n, int improvement)>();
        
        Parallel.ForEach(possibleRotations, rotation =>
        {
            string fieldState = GetFieldState(field);
            string rotationKey = $"{fieldState}_{rotation.x}_{rotation.y}_{rotation.n}";
            
            int improvement;
            // Kiểm tra cache trước khi tính toán
            if (_memoization.TryGetValue(rotationKey, out improvement))
            {
                results.Add((rotation.x, rotation.y, rotation.n, improvement));
            }
            else
            {
                improvement = field.EvaluateRotation(rotation.x, rotation.y, rotation.n);
                _memoization[rotationKey] = improvement;
                results.Add((rotation.x, rotation.y, rotation.n, improvement));
            }
        });
        
        foreach (var result in results)
        {
            if (result.improvement > bestRotation.improvement)
            {
                bestRotation = result;
            }
        }
        
        return bestRotation;
    }
    
    // Tìm kiếm cục bộ với khởi động lại ngẫu nhiên
    private void LocalSearchWithRestart(int maxDepth, int timeLimit)
    {
        DateTime startTime = DateTime.Now;
        Random random = new Random();
        
        while ((DateTime.Now - startTime).TotalSeconds < timeLimit)
        {
            // Khởi tạo trạng thái ngẫu nhiên
            Field currentField = new Field(_field);
            Solution currentSolution = new Solution();
            
            // Thực hiện một số thao tác ngẫu nhiên để tạo điểm khởi đầu
            int numRandomMoves = random.Next(1, 5);
            for (int i = 0; i < numRandomMoves; i++)
            {
                var possibleRotations = currentField.GetAllPossibleRotations();
                var randomRotation = possibleRotations[random.Next(possibleRotations.Count)];
                
                currentField.Rotate(randomRotation.x, randomRotation.y, randomRotation.n);
                currentSolution.Operations.Add(new Operation 
                { 
                    X = randomRotation.x, 
                    Y = randomRotation.y, 
                    N = randomRotation.n 
                });
            }
            
            // Tiến hành tìm kiếm tham lam từ điểm khởi đầu này
            for (int depth = numRandomMoves; depth < maxDepth; depth++)
            {
                int currentPairs = currentField.CountPairs();
                var bestRotation = FindBestRotation(currentField);
                
                // Nếu không có thao tác nào cải thiện, dừng lại
                if (bestRotation.improvement <= 0)
                    break;
                
                // Thực hiện thao tác tốt nhất
                currentField.Rotate(bestRotation.x, bestRotation.y, bestRotation.n);
                currentSolution.Operations.Add(new Operation 
                { 
                    X = bestRotation.x, 
                    Y = bestRotation.y, 
                    N = bestRotation.n 
                });
                
                // Cập nhật giải pháp tốt nhất nếu cần
                int newPairs = currentField.CountPairs();
                if (newPairs > _bestPairs)
                {
                    _bestPairs = newPairs;
                    _bestSolution = new Solution
                    {
                        Operations = currentSolution.Operations.ToList()
                    };
                }
            }
            
            // Kiểm tra thời gian còn lại
            if ((DateTime.Now - startTime).TotalSeconds > timeLimit)
                break;
        }
    }
    
    // Nâng cao: Tìm kiếm cục bộ có hướng dẫn
    public void GuidedLocalSearch(int maxDepth, int timeLimit)
    {
        DateTime startTime = DateTime.Now;
        Field currentField = new Field(_field);
        Solution currentSolution = new Solution();
        int currentPairs = _initialPairs;
        
        // Ưu tiên các thao tác xoay lưới con nhỏ hơn trước
        var entityFrequency = new Dictionary<int, int>();
        
        // Đếm tần suất xuất hiện của mỗi loại entity
        for (int i = 0; i < currentField.Size; i++)
        {
            for (int j = 0; j < currentField.Size; j++)
            {
                int entity = currentField.Entities[i, j];
                if (!entityFrequency.ContainsKey(entity))
                    entityFrequency[entity] = 0;
                entityFrequency[entity]++;
            }
        }
        
        // Ưu tiên các entity xuất hiện nhiều nhất
        var priorityEntities = entityFrequency.OrderByDescending(kv => kv.Value)
                                            .Take(3)
                                            .Select(kv => kv.Key)
                                            .ToList();
        
        while ((DateTime.Now - startTime).TotalSeconds < timeLimit && currentSolution.Operations.Count < maxDepth)
        {
            var possibleRotations = currentField.GetAllPossibleRotations();
            
            // Sắp xếp các thao tác theo kích thước lưới con (nhỏ đến lớn)
            // và ưu tiên các khu vực có entity ưu tiên
            possibleRotations = possibleRotations
                .OrderBy(r => r.n)
                .ThenByDescending(r => ContainsPriorityEntity(currentField, r.x, r.y, r.n, priorityEntities))
                .ToList();
            
            bool improved = false;
            
            foreach (var rotation in possibleRotations)
            {
                int improvement = currentField.EvaluateRotation(rotation.x, rotation.y, rotation.n);
                
                if (improvement > 0)
                {
                    currentField.Rotate(rotation.x, rotation.y, rotation.n);
                    currentSolution.Operations.Add(new Operation 
                    { 
                        X = rotation.x, 
                        Y = rotation.y, 
                        N = rotation.n 
                    });
                    
                    currentPairs += improvement;
                    
                    if (currentPairs > _bestPairs)
                    {
                        _bestPairs = currentPairs;
                        _bestSolution = new Solution
                        {
                            Operations = currentSolution.Operations.ToList()
                        };
                    }
                    
                    improved = true;
                    break;
                }
            }
            
            // Nếu không có cải thiện, thực hiện một thao tác ngẫu nhiên
            if (!improved)
            {
                Random random = new Random();
                var randomRotation = possibleRotations[random.Next(possibleRotations.Count)];
                
                currentField.Rotate(randomRotation.x, randomRotation.y, randomRotation.n);
                currentSolution.Operations.Add(new Operation 
                { 
                    X = randomRotation.x, 
                    Y = randomRotation.y, 
                    N = randomRotation.n 
                });
                
                currentPairs = currentField.CountPairs();
            }
        }
    }
    
    // Kiểm tra xem lưới con có chứa entity ưu tiên không
    private bool ContainsPriorityEntity(Field field, int x, int y, int n, List<int> priorityEntities)
    {
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                if (priorityEntities.Contains(field.Entities[y + i, x + j]))
                    return true;
            }
        }
        return false;
    }
}