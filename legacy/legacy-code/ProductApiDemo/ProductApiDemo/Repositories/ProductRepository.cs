using ProductApiDemo.Models;

namespace ProductApiDemo.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly List<Product> _products = new List<Product>()
        {
            new Product { Id = 1, Name = "Laptop", Price = 999.99m, Description = "High performance laptop" },
            new Product { Id = 2, Name = "Smartphone", Price = 499.99m, Description = "Latest model smartphone" },
            new Product { Id = 3, Name = "Tablet", Price = 299.99m, Description = "Portable tablet with large screen" }
        };

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await Task.FromResult(_products);
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await Task.FromResult(_products.FirstOrDefault(p => p.Id == id));
        }

        public async Task<IEnumerable<Product>> CreateAsync(Product product)
        {
            product.Id = _products.Any() ? _products.Max(p => p.Id) + 1 : 1;
            _products.Add(product);
            return await Task.FromResult(_products);
        }

        public async Task<Product?> UpdateAsync(int id, Product product)
        {
            var existingProduct = _products.FirstOrDefault(p => p.Id == id);
            if (existingProduct == null)
            {
                return null;
            }

            existingProduct.Name = product.Name;
            existingProduct.Price = product.Price;
            existingProduct.Description = product.Description;

            return await Task.FromResult(existingProduct);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return false;
            }

            _products.Remove(product);
            return await Task.FromResult(true);
        } 
    }
}
