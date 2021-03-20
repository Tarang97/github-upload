using ASP.NETCoreWebAPI.Models;
using ASPNETCoreWebAPI.Helper;
using ASPNETCoreWebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASP.NETCoreWebAPI.Controllers
{
    [ApiVersion("1.0")]
    //[Route("v{v:apiVersion}/products")]
    [Route("products")]
    [ApiController]
    [Authorize]
    public class ProductsV1_0Controller : ControllerBase
    {
        private readonly AppDbContext _ctx;

        public ProductsV1_0Controller(AppDbContext ctx)
        {
            _ctx = ctx;

            _ctx.Database.EnsureCreated();  // This ensure that database has been created.
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts([FromQuery] ProductQueryParameter queryParameter)
        {
            IQueryable<Product> products = _ctx.Products;

            // Checks if Minimum price and Maximum price is Not Null,
            // And both of them are selected based on the price labeled on products
            if (queryParameter.MinPrice != null &&
                queryParameter.MaxPrice != null)
            {
                products = products.Where(
                    p => p.Price >= queryParameter.MinPrice &&
                         p.Price <= queryParameter.MaxPrice
                    );
            }

            // Checks for product with based on SKU
            if (!string.IsNullOrEmpty(queryParameter.Sku))
            {
                products = products.Where(p => p.Sku == queryParameter.Sku);
            }

            // Checks for available Product based on search criteria
            if (!string.IsNullOrEmpty(queryParameter.Name))
            {
                products = products.Where(
                    p => p.Name.ToLower().Contains(queryParameter.Name.ToLower())
                    );
            }

            // Sort the products based on Id and also with the OrderType
            if (!string.IsNullOrEmpty(queryParameter.SortBy))
            {
                if (typeof(Product).GetProperty(queryParameter.SortBy) != null)
                {
                    products = products.OrderByCustom(queryParameter.SortBy, queryParameter.SortOrder);
                }
            }

            products = products
                .Skip(queryParameter.Size * (queryParameter.Page - 1))
                .Take(queryParameter.Size);

            return Ok(await products.ToListAsync());
        }

        // Gets the product based on ID but, also checks whether product exists or not.
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _ctx.Products.FindAsync(id);
            if (product == null)
                return NotFound();
            return Ok(product);
        }

        // Creates a new Product from the Body of the request and returns the newly created product.
        [HttpPost]
        public async Task<ActionResult<Product>> AddProduct([FromBody] Product product)
        {
            _ctx.Products.Add(product);
            await _ctx.SaveChangesAsync();

            return CreatedAtAction(
                "GetProduct",
                new { id = product.Id },
                product
                );
        }

        // Updates the product by getting the ID from route and from the Modified Body of the request
        // and checks if products exists, handles Concurrency control and updates the product with No Response.
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct([FromRoute] int id, [FromBody] Product product)
        {
            if(id != product.Id)
            {
                return BadRequest();
            }

            // This will change the existing data based on the Modified state from the Model Props
            _ctx.Entry(product).State = EntityState.Modified;

            try
            {
                await _ctx.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (_ctx.Products.Find(id) == null)
                    return NotFound();

                throw;
            }

            return NoContent();
        }

        // Find and Delete the product by ID and return the Deleted Product Detail.
        [HttpDelete("{id}")]
        public async Task<ActionResult<Product>> DeleteProduct(int id)
        {
            var product = await _ctx.Products.FindAsync(id);

            if (product == null) return NotFound();

            _ctx.Products.Remove(product);
            await _ctx.SaveChangesAsync();

            return product;
        }
    }

    [ApiVersion("2.0")]
    //[Route("v{v:apiVersion}/products")]
    [Route("products")]
    [ApiController]
    public class ProductsV2_0Controller : ControllerBase
    {
        private readonly AppDbContext _ctx;

        public ProductsV2_0Controller(AppDbContext ctx)
        {
            _ctx = ctx;

            _ctx.Database.EnsureCreated();  // This ensure that database has been created.
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts([FromQuery] ProductQueryParameter queryParameter)
        {
            IQueryable<Product> products = _ctx.Products.Where(p => p.IsAvailable == true);

            // Checks if Minimum price and Maximum price is Not Null,
            // And both of them are selected based on the price labeled on products
            if (queryParameter.MinPrice != null &&
                queryParameter.MaxPrice != null)
            {
                products = products.Where(
                    p => p.Price >= queryParameter.MinPrice &&
                         p.Price <= queryParameter.MaxPrice
                    );
            }

            // Checks for product with based on SKU
            if (!string.IsNullOrEmpty(queryParameter.Sku))
            {
                products = products.Where(p => p.Sku == queryParameter.Sku);
            }

            // Checks for available Product based on search criteria
            if (!string.IsNullOrEmpty(queryParameter.Name))
            {
                products = products.Where(
                    p => p.Name.ToLower().Contains(queryParameter.Name.ToLower())
                    );
            }

            // Sort the products based on Id and also with the OrderType
            if (!string.IsNullOrEmpty(queryParameter.SortBy))
            {
                if (typeof(Product).GetProperty(queryParameter.SortBy) != null)
                {
                    products = products.OrderByCustom(queryParameter.SortBy, queryParameter.SortOrder);
                }
            }

            products = products
                .Skip(queryParameter.Size * (queryParameter.Page - 1))
                .Take(queryParameter.Size);

            return Ok(await products.ToListAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _ctx.Products.FindAsync(id);
            if (product == null)
                return NotFound();
            return Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult<Product>> AddProduct([FromBody] Product product)
        {
            _ctx.Products.Add(product);
            await _ctx.SaveChangesAsync();

            return CreatedAtAction(
                "GetProduct",
                new { id = product.Id },
                product
                );
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct([FromRoute] int id, [FromBody] Product product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }

            // This will change the existing data based on the Modified state from the Model Props
            _ctx.Entry(product).State = EntityState.Modified;

            try
            {
                await _ctx.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (_ctx.Products.Find(id) == null)
                    return NotFound();

                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Product>> DeleteProduct(int id)
        {
            var product = await _ctx.Products.FindAsync(id);

            if (product == null) return NotFound();

            _ctx.Products.Remove(product);
            await _ctx.SaveChangesAsync();

            return product;
        }
    }
}
