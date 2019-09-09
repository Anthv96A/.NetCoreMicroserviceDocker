using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CategoryApi.DBContexts;
using CategoryApi.Model;

namespace CategoryApi.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly ProductContext _dbContext;

        public ProductRepository(ProductContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task DeleteProductAsync(int id)
            => await UpdateDB(async() => await _dbContext.Products.FindAsync(id), result => _dbContext.Products.Remove(result));

        public async Task<Product> GetProductByIdAsync(int id)
            => await _dbContext.Products.FindAsync(id);

        public async Task<IEnumerable<Product>> GetProductsAsync()
            => await Task.FromResult(_dbContext.Products.AsEnumerable());

        public async Task InsertProductAsync(Product product)
            => await UpdateDB(async() => await _dbContext.Products.AddAsync(product));

        public async Task UpdateProductAsync(Product product)
            => await UpdateDB(async() => await _dbContext.Products.FindAsync(product.Id), _ => _dbContext.Products.Update(product));

        private async Task UpdateDB<T>(Func<Task<T>> funcToRun) where T : class
            => await UpdateDB(funcToRun, null);

        private async Task UpdateDB<T>(Func<Task<T>> funcToRun, Action<T> ToComplete) where T : class
        {
            T value = await funcToRun();

            if(value != null)
            {
                ToComplete?.Invoke(value);
                _dbContext.SaveChanges();
            }
        }
    }
}
