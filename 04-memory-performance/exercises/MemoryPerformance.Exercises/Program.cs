using MemoryPerformance.Exercises;

const string words = "API api cache GC cache";
const string numbers = "10,20,30";
const string key = "  order-2026-ready  ";

Console.WriteLine("Memory & Performance Exercises");
Console.WriteLine($"Unique words: {string.Join(", ", AllocationExercises.UniqueWords(words))}");
Console.WriteLine($"Parsed ints: {string.Join(", ", SpanExercises.ParseThreeNumbers(numbers))}");
Console.WriteLine($"Normalized: {PooledBufferExercises.NormalizeKey(key)}");
