using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ShopCarApi.Entities;
using ShopCarApi.Helpers;
using ShopCarApi.ViewModels;
using WebElectra.Entities;

namespace ShopCarApi.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class FiltersController : ControllerBase
    {
        private readonly EFDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _env;

        public FiltersController(IHostingEnvironment env,
            IConfiguration configuration,
            EFDbContext context)
        {
            _configuration = configuration;
            _env = env;
            _context = context;
        }
        [HttpGet]
        public IActionResult FilterData()
        {
            var filters = GetListFilters(_context);
            return Ok(filters);
        }
        [HttpGet("GetMakeByModels")]
        public IActionResult GetMakeByModels(int id)
        {
            var makeAndModels = (from g in _context.MakesAndModels
                                 select g).Where(g=>g.FilterValueId==id).LastOrDefault();
            if(makeAndModels!=null)
            return Ok(makeAndModels.FilterMakeId);
            return Ok();
        }

        [HttpGet("GetModelsByMake")]
        public IActionResult GetModelsByMake(int id)
        {
            var makeAndModels = (from g in _context.MakesAndModels
                                 select g).AsQueryable();          
            var valueFilters = (from g in _context.FilterValues
                                select g).AsQueryable();
            var make = (from g in _context.Makes
                        select g).AsQueryable();
            var nameFilters = (from g in _context.FilterNames
                               select g).AsQueryable();
            var filters = GetListFilters(_context);
            var models = (from f in _context.MakesAndModels
                          where f.FilterMakeId == id
                          group f by new 
                          {
                              Id = f.FilterMakeId,
                              Name = f.FilterMakeOf.Name,
                          } into g
                          select new FNameViewModel
                          {
                              Id = g.Key.Id,
                              Name = g.Key.Name,
                              Children = (from value in g
                                          select new FValueViewModel
                                          {
                                              Id = value.FilterValueId,
                                              Name = value.FilterValueOf.Name
                                          }) 
                                         .OrderBy(l => l.Name).ToList()
                          }).ToList();                       
                return Ok(models);
        }
        private List<FNameViewModel> GetListFilters(EFDbContext context)
        {
            var queryName = from f in context.FilterNames.AsQueryable()
                            select f;
            var queryGroup = from g in context.FilterNameGroups.AsQueryable()
                             select g;

            //Отримуємо загальну множину значень
            var query = from u in queryName
                        join g in queryGroup on u.Id equals g.FilterNameId into ua
                        from aEmp in ua.DefaultIfEmpty()
                        select new
                        {
                            FNameId = u.Id,
                            FName = u.Name,
                            FValueId = aEmp != null ? aEmp.FilterValueId : 0,
                            FValue = aEmp != null ? aEmp.FilterValueOf.Name : null,
                        };

            //Групуємо по іменам і сортуємо по спаданю імен
            var groupNames = (from f in query
                              group f by new
                              {
                                  Id = f.FNameId,
                                  Name = f.FName
                              } into g
                              //orderby g.Key.Name
                              select g).OrderByDescending(g => g.Key.Name);

            //По групах отримуємо
            var result = from fName in groupNames
                         select
                         new FNameViewModel
                         {
                             Id = fName.Key.Id,
                             Name = fName.Key.Name,
                             Children = (from v in fName
                                         group v by new FValueViewModel
                                         {
                                             Id = v.FValueId,
                                             Name = v.FValue
                                         } into g
                                         select g.Key)
                                         .OrderBy(l => l.Name).ToList()
                         };

            return result.ToList();
        }      
    }
}