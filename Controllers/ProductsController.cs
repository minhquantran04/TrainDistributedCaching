using System.Text.Json;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging.Abstractions;
using TrainDistributedCaching.Models;

namespace TrainDistributedCaching.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IDistributedCache _cache;

        // logger để ghi log ra console hoặc file
        private readonly ILogger<ProductsController> _logger;
        public ProductsController(AppDbContext context, IDistributedCache cache, ILogger<ProductsController> logger)
        {
            _context = context;
            _cache = cache;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct()
        {
            const string cacheKey = "products_all";
            //lấy data từ cache
            var cachedData = await _cache.GetStringAsync(cacheKey);
            if (cachedData != null)
            {
                _logger.LogInformation(" Data is get from distributed cache(SQL)");
                var cachedProduct = JsonSerializer.Deserialize<Product>(cachedData);
                return Ok(cachedProduct);
            }
            // 2. Nếu không có cache → truy vấn DB
            var products = await _context.Products.ToListAsync();
            // 3. Lưu vào cache
            var options = new DistributedCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(5)); // hết hạn nếu không dùng 5 phút

            var jsonData = JsonSerializer.Serialize(products);
            await _cache.SetStringAsync(cacheKey, jsonData, options);

            _logger.LogInformation(" Lấy từ DB và lưu vào Distributed Cache");
            return Ok(products);

        }
    }
}