using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ShopCarApi.Entities;
using ShopCarApi.ViewModels;
using WebElectra.Entities;

namespace ShopCarApi.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class MakeController : ControllerBase
    {
        private readonly EFDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _env;
        public MakeController(IHostingEnvironment env,
            IConfiguration configuration,
            EFDbContext context)
        {
            _configuration = configuration;
            _env = env;
            _context = context;
        }

        [HttpGet]
        public IActionResult MakeList(string Name)
        {
            if (Name == null)
            {
                var model = _context.Makes.Select(
                    p => new MakeVM
                    {
                        Id = p.Id,
                        Name = p.Name
                    }).ToList();
                return Ok(model);
            }
            else
            {
                var query = _context.Makes.AsQueryable();
                var queryResult = (from make in query
                                   where make.Name.Contains(Name)
                                   select new MakeVM { Id = make.Id, Name = make.Name }).ToList();
       
                return Ok(queryResult);
            }
        }       
        [HttpPost]
        public IActionResult Create([FromBody]MakeAddVM model)
        {
            if (!ModelState.IsValid)
            {
                var errors = CustomValidator.GetErrorsByModel(ModelState);
                return BadRequest(errors);
            }           
            var make = _context.Makes.SingleOrDefault(p => p.Name == model.Name);
            if (make == null)
            {
                Make m = new Make
                { 
                    Name = model.Name
                };
                _context.Makes.Add(m);
                _context.SaveChanges();
                return Ok("Дані добалено");
            }
                return BadRequest(new { name = "Дана марка вже добалена" });         
        }
        [HttpDelete]
        public IActionResult Delete([FromBody]MakeDeleteVM model)
        {
            //if (!ModelState.IsValid)
            //{
            //    var errors = CustomValidator.GetErrorsByModel(ModelState);
            //    return BadRequest(errors);
            //}
            //var make = _context.Makes.SingleOrDefault(p => p.Id == model.Id);
            //if (make != null)
            //{
            //    var models= _context.Models.Where(p => p.MakeId == model.Id).ToList();
            //    if(models!=null)
            //    {
            //        foreach (var item in models)
            //        {
            //            _context.Models.Remove(item);
            //            _context.SaveChanges();
            //        }
            //    }
            //    _context.Makes.Remove(make);
            //    _context.SaveChanges();
            return Ok("Дані видалено");
            //}
        }
        [HttpPut]
        public IActionResult Update([FromBody]MakeVM model)
        {
            if (!ModelState.IsValid)
            {
                var errors = CustomValidator.GetErrorsByModel(ModelState);
                return BadRequest(errors);
            }
            var make = _context.Makes.SingleOrDefault(p => p.Id == model.Id);
            if (make != null)
            {
                make = _context.Makes.SingleOrDefault(p => p.Name == model.Name);
                if (make == null)
                {
                    make.Name = model.Name;
                    _context.SaveChanges();
                    return Ok("Дані оновлено");
                }
            }
            return BadRequest(new { name = "Помилка оновлення" });
        }
    }
}