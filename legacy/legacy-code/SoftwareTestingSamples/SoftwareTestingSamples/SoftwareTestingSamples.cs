using System;

namespace SoftwareTestingSamples
{
    /// <summary>
    /// Giải phương trình bậc hai: ax² + bx + c = 0
    /// </summary>
    public static class QuadraticSolver
    {
        /// <summary>
        /// Kết quả giải phương trình
        /// </summary>
        public struct Result
        {
            public bool IsQuadratic;      // a != 0
            public int RootCount;         // 0, 1, 2
            public double X1, X2;         // nghiệm (nếu có)
            public string Message;        // thông báo (nếu vô nghiệm hoặc a=0)
        }

        /// <summary>
        /// Giải phương trình bậc hai với hệ số a, b, c
        /// </summary>
        public static Result Solve(double a, double b, double c)
        {
            var res = new Result();

            if (a == 0.0)
            {
                res.IsQuadratic = false;
                res.Message = "Không phải phương trình bậc hai";
                res.RootCount = 0;
                return res;
            }

            res.IsQuadratic = true;
            double delta = b * b - 4 * a * c;

            if (delta > 0.0)
            {
                double sqrtD = Math.Sqrt(delta);
                res.X1 = (-b + sqrtD) / (2 * a);
                res.X2 = (-b - sqrtD) / (2 * a);
                res.RootCount = 2;
            }
            else if (Math.Abs(delta) < 1e-15)  // xem như Δ = 0
            {
                res.X1 = -b / (2 * a);
                res.RootCount = 1;
            }
            else
            {
                res.RootCount = 0;
                res.Message = "Phương trình vô nghiệm";
            }

            return res;
        }
    }

    /// <summary>
    /// Phân loại tam giác theo ba cạnh x, y, z
    /// </summary>
    public static class TriangleClassifier
    {
        public enum TriangleType
        {
            Equilateral,
            Isosceles,
            Scalene,
            NotATriangle
        }

        /// <summary>
        /// Xác định loại tam giác
        /// </summary>
        public static TriangleType Classify(double x, double y, double z)
        {
            // Kiểm đầu vào: phải > 0
            if (x <= 0 || y <= 0 || z <= 0)
                return TriangleType.NotATriangle;

            // Kiểm bất đẳng thức tam giác
            if (x + y <= z || x + z <= y || y + z <= x)
                return TriangleType.NotATriangle;

            // Tam giác đều
            if (Math.Abs(x - y) < 1e-12 && Math.Abs(y - z) < 1e-12)
                return TriangleType.Equilateral;

            // Tam giác cân
            if (Math.Abs(x - y) < 1e-12 ||
                Math.Abs(x - z) < 1e-12 ||
                Math.Abs(y - z) < 1e-12)
                return TriangleType.Isosceles;

            // Còn lại tam giác thường
            return TriangleType.Scalene;
        }
    }
}