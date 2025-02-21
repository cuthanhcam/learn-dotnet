using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

// LINQ (Language Integrated Query) is a set of methods that allow you to query data in C#.
// SQL 
// Nguồn dữ liệu: IEnumerable, INumerable<T> (List, Array, Dictionary, ...)
// Cú pháp: from ... in ... where ... select ...

namespace CS030_LINQ
{
    public class Product
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string[] Colors { get; set; }
        public int Brand { get; set; }

        public Product(int id, string name, double price, string[] colors, int brand)
        {
            ID = id;
            Name = name;
            Price = price;
            Colors = colors;
            Brand = brand;
        }
        override public string ToString()
        {
            return $"ID: {ID}, Name: {Name}, Price: {Price}, Colors: {string.Join(", ", Colors)}, Brand: {Brand}";
        }
    }

    public class Brand
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public Brand(int id, string name)
        {
            ID = id;
            Name = name;
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var brands = new List<Brand>()
            {
                new Brand(1, "Apple"),
                new Brand(2, "Samsung"),
                new Brand(3, "Xiaomi"),
                new Brand(4, "Oppo"),
                new Brand(5, "Vivo")
            };

            var products = new List<Product>()
            {
                new Product(1, "iPhone 12", 1000, new string[] { "Black", "White", "Red" }, 1),
                new Product(2, "Galaxy S21", 900, new string[] { "Black", "White", "Red" }, 2),
                new Product(3, "Mi 11", 800, new string[] { "Black", "White", "Red" }, 3),
                new Product(4, "Find X3", 700, new string[] { "Black", "White", "Red" }, 4),
                new Product(5, "X60", 600, new string[] { "Black", "White", "Red" }, 5),
                new Product(6, "iPhone 11", 800, new string[] { "Black", "White", "Red" }, 1),
                new Product(7, "Galaxy S20", 700, new string[] { "Black", "White", "Red" }, 2),
                new Product(8, "Mi 10", 600, new string[] { "Black", "White", "Red" }, 3),
                new Product(9, "Find X2", 500, new string[] { "Black", "White", "Red" }, 4),
                new Product(10, "X50", 400, new string[] { "Black", "White", "Red" }, 5)
            };

            // Select: Lấy ra các phần tử từ một nguồn dữ liệu và chuyển đổi chúng thành một dạng mới
                // Lấy ra các sản phẩm có giá == 700
                var querySelect = from p in products
                            where p.Price == 700
                            select p;

                foreach (var item in querySelect)
                {
                    Console.WriteLine(item);
                }
            
                // Lấy tên và giá theo phương thức delegate
                var resultSelect = products.Select(
                    (p) =>
                    {
                        // return p.Name + " - " + p.Price;
                        return new { Name = p.Name, Price = p.Price }; // Anonymous Type - Kiểu vô danh
                    }
                );

                foreach (var item in resultSelect)
                {
                    Console.WriteLine(item);
                }

            // Where: Lọc các phần tử từ một nguồn dữ liệu dựa trên một điều kiện
                // Lấy ra các sản phẩm có giá > 700 và < 900
                var resultWhere = products.Where(
                    (p) =>
                    {
                        return p.Price > 700 && p.Price < 900;
                    }
                );
                foreach (var item in resultWhere)
                {
                    Console.WriteLine(item);
                }

            // SelectMany: Lấy ra các phần tử từ một nguồn dữ liệu và chuyển đổi chúng thành một dạng mới
                var resultSelectMany = products.SelectMany(
                    (p) =>
                    {
                        return p.Colors;
                    }
                );
                foreach (var item in resultSelectMany)
                {
                    Console.WriteLine(item);
                }
            // Min, max, Sum, Average
                var minPrice = products.Min(p => p.Price);
                var maxPrice = products.Max(p => p.Price);
                var sumPrice = products.Sum(p => p.Price);
                var avgPrice = products.Average(p => p.Price);
                Console.WriteLine($"Min: {minPrice}, Max: {maxPrice}, Sum: {sumPrice}, Avg: {avgPrice}");    
            // Join
                var queryJoin = from p in products
                                join b in brands on p.Brand equals b.ID
                                select new { ProductName = p.Name, BrandName = b.Name };
                foreach (var item in queryJoin)
                {
                    Console.WriteLine(item);
                }

            // GroupJoin
                var queryGroupJoin = from b in brands
                                join p in products on b.ID equals p.Brand into g
                                select new { BrandName = b.Name, Products = g };
                foreach (var item in queryGroupJoin)
                {
                    Console.WriteLine(item.BrandName);
                    foreach (var product in item.Products)
                    {
                        Console.WriteLine(product);
                    }
                }   
            // Take
                var queryTake = products.Take(3);
                foreach (var item in queryTake)
                {
                    Console.WriteLine(item);
                }
            // Skip
                var querySkip = products.Skip(3);
                foreach (var item in querySkip)
                {
                    Console.WriteLine(item);
                }
            // OrderBy, OrderByDescending
                var queryOrderBy = products.OrderBy(p => p.Price);
                foreach (var item in queryOrderBy)
                {
                    Console.WriteLine(item);
                }
                var queryOrderByDescending = products.OrderByDescending(p => p.Price);
                foreach (var item in queryOrderByDescending)
                {
                    Console.WriteLine(item);
                }
            // Reverse
                var queryReverse = products.AsEnumerable().Reverse(); // AsEnumerable() to see the difference
                foreach (var item in queryReverse)
                {
                    Console.WriteLine(item);
                }            
            // GroupBy
                var queryGroupBy = from p in products
                                group p by p.Brand into g
                                select new { Brand = g.Key, Products = g };
                foreach (var item in queryGroupBy)
                {
                    Console.WriteLine(item.Brand);
                    foreach (var product in item.Products)
                    {
                        Console.WriteLine(product);
                    }
                }
            // Distinct
                var queryDistinct = products.SelectMany(p => p.Colors).Distinct();
                foreach (var item in queryDistinct)
                {
                    Console.WriteLine(item);
                }
            // Single, SingeOrDefault
                var querySingle = products.Single(p => p.ID == 1);
                Console.WriteLine(querySingle);
                var querySingleOrDefault = products.SingleOrDefault(p => p.ID == 1);
                Console.WriteLine(querySingleOrDefault);
            // Any
                var queryAny = products.Any(p => p.Price == 700);
                Console.WriteLine(queryAny);
            // All
                var queryAll = products.All(p => p.Price > 0);
                Console.WriteLine(queryAll);
            // Count
                var queryCount = products.Count(p => p.Price == 700);
                Console.WriteLine(queryCount);

            // Ví dụ tổng hợp sử dụng API LINQ
            // In tên sản phẩm, tên thương hiệu có giá (Price) lớn hơn 700 và có màu sắc (Colors) là Black, giá giảm dần
                var query = from p in products
                            join b in brands on p.Brand equals b.ID
                            where p.Price > 700 && p.Colors.Contains("Black")
                            orderby p.Price descending
                            select new { ProductName = p.Name, BrandName = b.Name };
                foreach (var item in query)
                {
                    Console.WriteLine(item);
                }
                
        }
    }
}
