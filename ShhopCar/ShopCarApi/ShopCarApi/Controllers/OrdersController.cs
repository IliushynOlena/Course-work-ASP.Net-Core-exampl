using Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ShopCarApi.Entities;
using ShopCarApi.ViewModels;
using System.Linq;
using WebElectra.Entities;

namespace ShopCarApi.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly EFDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _env;

        public OrdersController(IHostingEnvironment env,
            IConfiguration configuration,
            EFDbContext context)
        {
            _configuration = configuration;
            _env = env;
            _context = context;
        }
        [HttpGet]
        public IActionResult OrderList()
        {

            var order = _context.Orders.Select(
                p => new OrderVM
                {
                    Car =
                   p.Car.Name,
                    Client = p.Client.Name
                }).ToList();
            return Ok(order);
        }
        [HttpPost]
        public IActionResult Create([FromBody]OrderAddVM model)
        {
            if (!ModelState.IsValid)
            {
                var errors = CustomValidator.GetErrorsByModel(ModelState);
                return BadRequest(errors);
            }           
            {
                Order m = new Order
                {
                    CarId = model.Car.Id,
                    Price = float.Parse(model.Car.Price.ToString()),
                    ClientId=model.Client.Id,
                    Date=model.Date                    
                };
                _context.Orders.Add(m);
                _context.SaveChanges();
                return Ok("Дані добалено");
            }
        }

    }
}