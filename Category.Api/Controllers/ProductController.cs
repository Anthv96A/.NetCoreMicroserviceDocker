using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;
using CategoryApi.Model;
using CategoryApi.Repository;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CategoryApi.Controllers
{
    [Route("api/[controller]")]
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;

        public ProductController(IProductRepository productRepository)
        {
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        }

        // GET: api/values
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            IEnumerable<Product> products =  await _productRepository.GetProductsAsync();
            return new OkObjectResult(products);
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        { 
            Product product = await _productRepository.GetProductByIdAsync(id);
            return new OkObjectResult(product);
        }

        // POST api/values
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Product value)
            => await VerbWrapper(value, async () => await _productRepository.UpdateProductAsync(value), CreatedAtAction(nameof(Get), new { id = value.Id }, value));

        // PUT api/values/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromBody]Product value)
            => await VerbWrapper(value, async () => await _productRepository.UpdateProductAsync(value), new OkResult());

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _productRepository.DeleteProductAsync(id);
            return new OkResult();
        }

        private async Task<IActionResult> VerbWrapper<T>(T value, Func<Task> funcToRun, IActionResult expectedReturnType) where T: class
        {
            if(value == null)
                return await Task.FromResult(new NoContentResult());

            using (var scope = new TransactionScope())
            {
                await funcToRun();
                scope.Complete();
                return expectedReturnType;
            }
        }   
    }
}
