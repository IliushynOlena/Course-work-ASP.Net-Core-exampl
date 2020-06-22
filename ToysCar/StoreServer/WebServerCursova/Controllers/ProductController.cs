using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using WebServerCursova.Entities;
using WebServerCursova.Helpers;
using WebServerCursova.Services;
using WebServerCursova.ViewModels;

namespace WebServerCursova.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly EFDbContext _context;
        //доступ до файла app.setting
        private readonly IConfiguration _configuration;
        //отримати доступ до сервера
        private readonly IHostingEnvironment _env;

        private readonly string dirPathSave;

        private readonly string kNamePhotoDefault = "Empty.jpg";


        public ProductController(IHostingEnvironment env, IConfiguration configuration, EFDbContext context)
        {
            _configuration = configuration;
            _env = env;
            _context = context;
            dirPathSave = ImageHelper.GetImageFolder(_env, _configuration);
        }

        #region HttpGetId
        [HttpGet("{id}")]
        public IActionResult GetProductById(int id)
        {
            var model = _context.Products
                .Select(p => new ProductPutVM
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    PhotoName = p.PhotoName
                })
                .SingleOrDefault(p => p.Id == id);
            if (model == null)
            {
                return NotFound(new { invalid = "Not fount by id" });
            }
            return Ok(model);
        }
        #endregion

        #region HttpGet
        [HttpGet]
        public IActionResult GetProducts()
        {
            var model = _context.Products
                .Select(p => new ProductGetVM
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    PhotoName = p.PhotoName
                }).ToList();

            return Ok(model);
        }
        #endregion

        #region HttpGetByCategory
        [HttpGet("GetByCategory")]
        public IActionResult GetProductsByCategory(int value)
        {
            var model = _context.Products.Where(t => t.CategoryId == value)
            .Select(p => new ProductGetVM
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                PhotoName = p.PhotoName
            }).ToList();

            return Ok(model);
        }

        #endregion

        #region HttpGetByFilter
        [HttpGet("GetByFilter")]
        public IActionResult FilterData(List<int> value)
        {
            var filters = FiltersService.GetFilterList(_context);
            var productsByFilter = GetProductsByFilter(value, filters);
            return Ok(productsByFilter);
        }

        private List<ProductGetVM> GetProductsByFilter(List<int> values, List<FilterVM> filterList)
        {
            var query = _context.Products.AsQueryable();
            //проходимо цикл по назвам фільтра
            foreach (var fName in filterList)
            {
                int count = 0; //кількість співпадінь в даній групі фільтрів
                var predicate = PredicateBuilder.False<DbProduct>();
                //проходимо цикл по значенням назв фільтрів
                foreach (var fValue in fName.Children)
                {
                    for (int i = 0; i < values.Count; i++)
                    {

                        var idValue = fValue.Id;
                        //перевірка на співпадіння(додаємо до запита "OR" якщо є співпадіння в однакових групах)
                        if (idValue == values[i])
                        {
                            predicate = predicate
                                .Or(p => p.Filters
                                    .Any(f => f.FilterValueId == idValue));
                            count++;
                        }
                    }
                }
                //додаємо до запита "AND" якщо є співпадіння в різних групах
                if (count > 0)
                {
                    query = query.Where(predicate);
                }
            }
            //формуємо новий список(відфільтрований)
            var result = query.Select(p => new ProductGetVM
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                PhotoName = p.PhotoName
            }).ToList();

            return result;
        }
        #endregion

        #region HttpPost
        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public IActionResult Create([FromBody]ProductPostVM model)
        {
            List<string> err = new List<string>();

            // перевіряємо модель на валідність
            if (!ModelState.IsValid)
            {
                var errors = CustomValidator.GetErrorsByModel(ModelState);
                return BadRequest(errors);
            }

            // зберігаємо фото
            var bmp = model.PhotoBase64.FromBase64StringToImage();
            if (bmp != null)
            {
                model.PhotoName = Path.GetRandomFileName() + ".jpg";

                string imageNamePath = Path.Combine(dirPathSave, model.PhotoName);
                var image = ImageHelper.CreateImage(bmp, 300, 300);
                image.Save(imageNamePath, ImageFormat.Jpeg);
            }
            else
            {
                model.PhotoName = kNamePhotoDefault;
            }

            // передаємо модель в БД
            DbProduct prod = new DbProduct
            {
                Name = model.Name,
                Price = model.Price,
                DateCreate = DateTime.Now,
                PhotoName = model.PhotoName,
                CategoryId = model.CategoryId
            };

            _context.Products.Add(prod);
            _context.SaveChanges();


            int fValueIdType = model.FilterIdType;
            int fNameIdType = _context.FilterNameGroups
                .SingleOrDefault(v => v.FilterValueId == model.FilterIdType).FilterNameId;

          
            int valuePrice = 0;
            if(model.Price < 50)
            {
                valuePrice = 1;
            }
            else if (model.Price >= 50 && model.Price < 100)
            {
                valuePrice = 2;
            }
            else if (model.Price >= 100 && model.Price < 300)
            {
                valuePrice = 3;
            }
            else if (model.Price >= 300 && model.Price < 700)
            {
                valuePrice = 4;
            }
            else if (model.Price >= 700)
            {
                valuePrice = 5;
            }

            Filter filter = new Filter { FilterNameId = 1, FilterValueId = valuePrice, ProductId = prod.Id };
            var f = _context.Filters.SingleOrDefault(p => p == filter);
            if (f == null)
            {
                _context.Filters.Add(filter);
                _context.SaveChanges();
            }

            filter = new Filter { FilterNameId = fNameIdType, FilterValueId = fValueIdType, ProductId = prod.Id };
            f = _context.Filters.SingleOrDefault(p => p == filter);
            if (f == null)
            {
                _context.Filters.Add(filter);
                _context.SaveChanges();
            }

            return Ok(prod.Id);
        }
        #endregion

        #region HttpDelete
        [HttpDelete]
        public IActionResult Delete([FromBody]ProductDeleteVM model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var fullProduct = _context.Products.SingleOrDefault(p => p.Id == model.Id);
            if (fullProduct != null)
            {
                //видаляємо фото(якщо не за замовчуванням)
                if (fullProduct.PhotoName != kNamePhotoDefault && fullProduct.PhotoName != null)
                {
                    string imageNamePath = Path.Combine(dirPathSave, fullProduct.PhotoName);
                    System.IO.File.Delete(imageNamePath);
                }
                //видаляємо продукт
                _context.Products.Remove(fullProduct);
                _context.SaveChanges();
            }

            return Ok(fullProduct.Id);
        }
        #endregion

        #region HttpPut
        [HttpPut]
        public IActionResult EditSave([FromBody]ProductPutVM newModel)
        {
            List<string> err = new List<string>();

            // перевіряємо нову модель на валідність
            if (!ModelState.IsValid)
            {
                var errors = CustomValidator.GetErrorsByModel(ModelState);
                return BadRequest(errors);
            }

            //// дістаємо стару модель
            var product = _context.Products
                .SingleOrDefault(p => p.Id == newModel.Id);

            if (product != null)
            {
                // якщо вибрали нове фото
                if (newModel.PhotoBase64 != "")
                {
                    product.PhotoName = Path.GetRandomFileName() + ".jpg";

                    var bmp = newModel.PhotoBase64.FromBase64StringToImage();

                    var imageNamePath = Path.Combine(dirPathSave, product.PhotoName);
                    var image = ImageHelper.CreateImage(bmp, 200, 200);
                    image.Save(imageNamePath, ImageFormat.Jpeg);
                }

                product.Name = newModel.Name;
                product.Price = newModel.Price;
                product.DateCreate = DateTime.Now;
                product.CategoryId = newModel.CategoryId;

                _context.SaveChanges();
            }
            else
            {
                return NotFound();
            }

            return Ok(newModel.Id);
        }
        #endregion
    }
}
