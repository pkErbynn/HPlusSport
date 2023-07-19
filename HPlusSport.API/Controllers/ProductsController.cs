using HPlusSport.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HPlusSport.API.Controllers
{
    [ApiVersion("1.0")]
    //[Route("api/[controller]")]
    //[Route("api/{v:apiVersion}/products")]
    [Route("api/products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ShopContext _context;

        public ProductsController(ShopContext context)
        {
            _context = context;

            _context.Database.EnsureCreated();
        }

        [HttpGet]
        public async Task<ActionResult> GetAllProducts([FromQuery]ProductFilterQueryParams queryParams)
        {
            IQueryable<Product> products = _context.Products;

            // apply filter
            if (queryParams.MinPrice != null)
            {
                products = products.Where(p => p.Price >= queryParams.MinPrice);
            }

            if (queryParams.MaxPrice != null)
            {
                products = products.Where(p => p.Price <= queryParams.MaxPrice);
            }

            // apply search
            if (!String.IsNullOrEmpty(queryParams.Name))
            {
                products = products.Where(p => p.Name.ToLower().Contains(queryParams.Name.ToLower()));
            }

            if (!String.IsNullOrEmpty(queryParams.Sku))
            {
                products = products.Where(p => p.Sku.ToLower().Contains(queryParams.Name.ToLower()));
            }

            if (!String.IsNullOrEmpty(queryParams.SearchTerm))  // generic cross-property search 
            {
                products = products.Where(p =>
                    p.Name.ToLower().Contains(queryParams.SearchTerm.ToLower()) ||
                    p.Sku.ToLower().Contains(queryParams.SearchTerm.ToLower())
                );
            }

            // apply sort
            if (!String.IsNullOrEmpty(queryParams.SortBy))
            {
                if (typeof(Product).GetProperty(queryParams.SortBy) != null)
                {
                    products = products.OrderByCustom(queryParams.SortBy, queryParams.SortOrder);
                }
            }

            // pagination applied last after all filter done
            var skippedProducts = (queryParams.Page - 1) * queryParams.Size;
            products = products.Skip(skippedProducts).Take(queryParams.Size);   // apply pagination

            return Ok(await products.ToArrayAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
            /*
            if (!ModelState.IsValid) {
                return BadRequest();
            }
            */
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                "GetProduct",
                new { id = product.Id },
                product);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> PutProduct(int id, Product product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Products.Any(p => p.Id == id))
                {
                    return NotFound();
                } 
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Product>> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return product;
        }

        [HttpPost]
        [Route("Delete")]
        public async Task<ActionResult> DeleteMultiple([FromQuery]int[] ids)
        {
            var products = new List<Product>();
            foreach (var id in ids)
            {
                var product = await _context.Products.FindAsync(id);

                if (product == null)
                {
                    return NotFound();
                }

                products.Add(product);
            }

            _context.Products.RemoveRange(products);
            await _context.SaveChangesAsync();

            return Ok(products);
        }
    }
}


