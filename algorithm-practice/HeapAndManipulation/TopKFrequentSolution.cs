using System.Collections.Generic;

namespace HeapAndManipulation
{
    public class TopKFrequentSolution
    {
        public int[] TopKFrequent(int[] nums, int k)
        {
            Dictionary<int, int> freq = new Dictionary<int, int>();
            foreach (int num in nums)
            {
                freq[num] = freq.GetValueOrDefault(num, 0) + 1;
            }
            
            PriorityQueue<int, int> minHeap = new PriorityQueue<int, int>();
            foreach (var pair in freq)
            {
                minHeap.Enqueue(pair.Key, pair.Value);
                if (minHeap.Count > k)
                {
                    minHeap.Dequeue();
                }
            }
            
            int[] result = new int[k];
            for (int i = k - 1; i >= 0; i--)
            {
                result[i] = minHeap.Dequeue();
            }
            
            return result;
        }
    }
}