using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using WebElectra.Entities;

namespace ShopCarApi.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class CarsImagesController : ControllerBase
    {
        private readonly EFDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _env;

        public CarsImagesController(IHostingEnvironment env,
                  IConfiguration configuration,
                  EFDbContext context)
        {
            _configuration = configuration;
            _env = env;
            _context = context;
        }

        [HttpGet]
        public IActionResult GetImagesBySize(string path, string size)
        {
            var getImages = new List<string>();
            string dirName = "images";
            string dirPathSave = Path.Combine(dirName, path);
            var imgs=Directory.GetFiles(dirPathSave);

            foreach (var item in imgs)
            {
                if (item.Contains(size))
                {

                    getImages.Add(item);
                }
            }
            if(getImages.Count!=0)
            return Ok(getImages);
            return BadRequest();
        }
    }
}