using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamicProgramming
{
    public class CombinationSumSolution
    {
        public IList<IList<int>> CombinationSum(int[] candidates, int target)
        {
            IList<IList<int>> result = new List<IList<int>>();
            Backtrack(candidates, target, 0, new List<int>(), result);
            return result;
        }
        
        private void Backtrack(int[] candidates, int target, int start, List<int> current, IList<IList<int>> result)
        {
            if (target == 0)
            {
                result.Add(new List<int>(current));
                return;
            }
            if (target < 0) return;
            
            for (int i = start; i < candidates.Length; i++)
            {
                current.Add(candidates[i]);
                Backtrack(candidates, target - candidates[i], i, current, result);
                current.RemoveAt(current.Count - 1);
            }
        }
    }
}