using DSA.Core.Algorithms.Searching;

namespace DSA.Tests.Algorithms
{
    public class BinarySearchTests
    {
        [Fact]
        public void BinarySearch_ShouldReturnCorrectIndex_WhenTargetExists()
        {
            int[] nums = { 1, 3, 5, 7, 9, 11, 13 };

            int result = BinarySearch.BinarySearchIterative(nums, 9);

            Assert.Equal(4, result);
        }

        [Fact]
        public void BinarySearch_ShouldReturnMinusOne_WhenTargetDoesntExists()
        {
            int[] nums = { 1, 3, 5, 7, 9, 11, 13 };

            int result = BinarySearch.BinarySearchRecursive(nums, 6);

            Assert.Equal(-1, result);
        }

        [Fact]
        public void Search_ShouldReturnMinusOne_WhenArrayIsEmpty()
        {
            int[] nums = Array.Empty<int>();

            int result = BinarySearch.BinarySearchIterative(nums, 1);

            Assert.Equal(-1, result);
        }

        [Fact]
        public void SearchRecursive_ShouldWorkCorrectly()
        {
            int[] nums = { 2, 4, 6, 8, 10 };

            int result = BinarySearch.BinarySearchRecursive(nums, 10);

            Assert.Equal(4, result);
        }
    }
}
