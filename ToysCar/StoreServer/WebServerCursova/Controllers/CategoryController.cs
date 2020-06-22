using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using WebServerCursova.Entities;
using WebServerCursova.Services;

namespace WebServerCursova.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly EFDbContext _context;
        //доступ до файла app.setting
        private readonly IConfiguration _configuration;
        //отримати доступ до сервера
        private readonly IHostingEnvironment _env;

        public CategoryController(IHostingEnvironment env, IConfiguration configuration, EFDbContext context)
        {
            _configuration = configuration;
            _env = env;
            _context = context;
        }

        #region HttpGet
        [HttpGet]
        public IActionResult GetFilters()
        {
            var model = CategoriesService.GetCategoryList(_context);
            return Ok(model);
        }
        #endregion
    }
}
