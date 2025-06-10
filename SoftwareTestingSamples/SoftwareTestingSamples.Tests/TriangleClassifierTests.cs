using SoftwareTestingSamples;
using static SoftwareTestingSamples.TriangleClassifier;

namespace SoftwareTestingSamples.Tests
{
    public class UnitTest1
    {
        [Theory]
        [InlineData(3, 3, 3, TriangleType.Equilateral)]
        [InlineData(4, 4, 5, TriangleType.Isosceles)]
        [InlineData(3, 4, 5, TriangleType.Scalene)]
        [InlineData(1, 2, 3, TriangleType.NotATriangle)]
        [InlineData(2, 2, 5, TriangleType.NotATriangle)]
        [InlineData(-1, 2, 2, TriangleType.NotATriangle)]
        [InlineData(0, 1, 1, TriangleType.NotATriangle)]
        [InlineData(1, 10, 2, TriangleType.NotATriangle)]
        public void TestTriangleClassification(double x, double y, double z, TriangleClassifier.TriangleType expected)
        {
            var result = TriangleClassifier.Classify(x, y, z);
            Assert.Equal(expected, result);
        }
    }
}