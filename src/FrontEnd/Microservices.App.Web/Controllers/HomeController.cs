using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microservices.App.Web.Models;
using Microservices.App.Web.Service.Abstract;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;

namespace Microservices.App.Web.Controllers;

public class HomeController : Controller
{
    private readonly IProductService productService;
    private readonly ILogger<HomeController> _logger;

    public HomeController(IProductService productService,ILogger<HomeController> logger)
    {
        this.productService = productService;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        List<ProductDto> products = new();

        var response = await this.productService.GetAllProductsAsync();

        if (response != null && response.IsSuccess)
        {
            products = JsonConvert.DeserializeObject<List<ProductDto>>(response.Result.ToString());
        }
        else
        {
            TempData["error"] = response?.Message;
        }

        return View(products);
    }

    [Authorize]
    public async Task<IActionResult> ProductDetails(int productId)
    {
        ProductDto product = new();

        var response = await this.productService.GetProductByIdAsync(productId);

        if (response != null && response.IsSuccess)
        {
            product = JsonConvert.DeserializeObject<ProductDto>(response.Result.ToString());
        }
        else
        {
            TempData["error"] = response?.Message;
        }

        return View(product);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}