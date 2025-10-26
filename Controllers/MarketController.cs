using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SuperMarket.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Market")]
    public class MarketController : ControllerBase
    {
        private readonly IMarketService _marketService;

        public MarketController(IMarketService marketService)
        {
            _marketService = marketService;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> CreateProduct(ProductDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var product = await _marketService.CreateProductAsync(model);

            if (product == null)
                return BadRequest($"Field to add {model.Name}");

            return Ok(product);
        }

        [HttpPut("[action]/{id:int}")]
        public async Task<IActionResult> UpdateProduct(int id, ProductDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var product = await _marketService.UpdateProductAsync(id, model);

            if (product == null)
                return BadRequest($"Field to update {model.Name}");

            return Ok(product);
        }

        [HttpDelete("[action]/{id:int}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            bool isDeleted = await _marketService.RemoveProductAsync(id);

            if (!isDeleted)
                return BadRequest($"Field to remove");

            return Ok("Deleted Successfully");
        }
    }
}
