using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Helpers;
using Image.Help;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ShopCarApi.Entities;
using ShopCarApi.ViewModels;
using WebElectra.Entities;
using WebElectra.Helpers;

namespace ShopCarApi.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class ClientsController : ControllerBase
    {
        private readonly EFDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _env;



        public ClientsController(IHostingEnvironment env,
            IConfiguration configuration,
            EFDbContext context)
        {
            _configuration = configuration;
            _env = env;
            _context = context;
        }

        [HttpGet]
        public IActionResult ClientList()
        {
            var query = _context.Clients.AsQueryable();

            string path = "images";
            var clients = query.Select(p => new ClientVM
            {
                Id = p.Id,
                Name = p.Name,
                Email = p.Email,
                Phone = p.Phone,
                UniqueName = p.UniqueName,
                Image = $"{path}/{p.UniqueName}/300_{p.UniqueName}.jpg"

            }).ToList();
            return Ok(clients);
        }
        [HttpGet("search")]
        public IActionResult ClientList(ClientVM client)
        {
            var queryUsers = _context.Clients.AsQueryable();

            var query = _context.Clients.AsQueryable();
            if (!String.IsNullOrEmpty(client.Email))
            {
                query = query.Where(e => e.Email.Contains(client.Email));
            }
            if (!String.IsNullOrEmpty(client.Name))
            {
                query = query.Where(e => e.Name.Contains(client.Name));
            }
            if (!String.IsNullOrEmpty(client.Phone))
            {
                query = query.Where(e => e.Phone.Contains(client.Phone));
            }

            //var queryResult = (from user in query
            //                   where user.UserName.Contains(employee.Name)
            //                   where user.Email.Contains(employee.Email)
            //                   select new UserVM { Name = user.UserName, Email = user.Email }).ToList();
            string path = "images";
            var resultClients = (from c in query
                             select
                             new ClientVM
                             {
                                 Id = c.Id,
                                 Image = $"{path}/{c.UniqueName}/300_{c.UniqueName}.jpg",
                                 Email = c.Email,
                                 Phone = c.Phone,
                                 Name = c.Name,
                                 UniqueName = c.UniqueName,
                             }).ToList();
            var clients = resultClients.ToList();
            return Ok(clients);
        }

        [HttpPost]
        public IActionResult Create([FromBody]ClientAddVM client)
        {
            
            if (!ModelState.IsValid)
            {
                var errors = CustomValidator.GetErrorsByModel(ModelState);
                return BadRequest(errors);
            }
            string dirName = "images";
            string dirPathSave = Path.Combine(dirName, client.UniqueName);
            if (!Directory.Exists(dirPathSave))
            {
                Directory.CreateDirectory(dirPathSave);
            }
            var bmp = client.Image.FromBase64StringToImage();
            var imageName = client.UniqueName;
            string fileSave = Path.Combine(dirPathSave, $"{imageName}");

            var bmpOrigin = new System.Drawing.Bitmap(bmp);
            string[] imageNames = {$"50_"+ imageName + ".jpg" ,
                    $"100_" + imageName + ".jpg",
                     $"300_" + imageName + ".jpg",
                   $"600_" + imageName + ".jpg",
                    $"1280_"+ imageName + ".jpg"};

            Bitmap[] imageSave = { ImageWorker.CreateImage(bmpOrigin, 50, 50),
                    ImageWorker.CreateImage(bmpOrigin, 100, 100),
                    ImageWorker.CreateImage(bmpOrigin, 300, 300),
                    ImageWorker.CreateImage(bmpOrigin, 600, 600),
                    ImageWorker.CreateImage(bmpOrigin, 1280, 1280)};

            for (int i = 0; i < imageNames.Count(); i++)
            {
                var imageSaveEnd = System.IO.Path.Combine(dirPathSave, imageNames[i]);
                imageSave[i].Save(imageSaveEnd, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            var str = client.Phone;
            var regex = @"\+38\d{1}\(\d{2}\)\d{3}\-\d{2}\-\d{2}";
            var str2 = client.Name;
            var regex2 = @"^[A-Za-z-а-яА-Я]+$";
            var str3 = client.Email;
            var regex3 = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";
            var match = Regex.Match(str, regex);
            var match2 = Regex.Match(str2, regex2);
            var match3 = Regex.Match(str3, regex3);
            if (!match.Success)
            {
               return BadRequest(new { Phone = "issue" });
            }
            if (!match2.Success)
            {
                return BadRequest(new { Name = "issue" });
            }
            if (!match3.Success)
            {
                return BadRequest(new { Email = "issue" });
            }

            var cl = _context.Clients.FirstOrDefault(p => p.Email == client.Email);           
            if (cl != null)
            {                        
                return BadRequest(new { Email = "Такий email вже існує!" });
            }
            var cli = _context.Clients.FirstOrDefault(p => p.Phone == client.Phone);
            if (cli != null)
            {
                return BadRequest(new { Phone = "Такий номер вже існує!" });
            }
            //var fileDestDir = _env.ContentRootPath;
            //string dirName = _configuration.GetValue<string>("ImagesPath");
            ////Папка де зберігаються фотки
            //string dirPathSave = Path.Combine(fileDestDir, dirName);
            //if (!Directory.Exists(dirPathSave))
            //{
            //    Directory.CreateDirectory(dirPathSave);
            //}
            //var bmp = model.Image.FromBase64StringToImage();
            //var imageName = Path.GetRandomFileName() + ".jpg";
            //string fileSave = Path.Combine(dirPathSave, $"{imageName}");

            //bmp.Save(fileSave, ImageFormat.Jpeg);

            Client c = new Client
            {
                Name = client.Name,
                Phone = client.Phone,
                Email = client.Email,
                UniqueName = client.UniqueName
            };
            _context.Clients.Add(c);
            _context.SaveChanges();
            return Ok(c.Id);
        }

        [HttpDelete]
        public IActionResult Delete([FromBody]ClientDeleteVM client)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var clients = _context.Clients.SingleOrDefault(p => p.Id == client.Id);
            if (clients != null)
            {
                _context.Clients.Remove(clients);
                _context.SaveChanges();
            }
            return Ok();
        }
        [HttpPut]
        public IActionResult Update([FromBody]ClientVM client)
        {
            if (!ModelState.IsValid)
            {
                var errors = CustomValidator.GetErrorsByModel(ModelState);
                return BadRequest(errors);
            }
            var str = client.Phone;
            var regex = @"\+38\d{1}\(\d{2}\)\d{3}\-\d{2}\-\d{2}";
            var str2 = client.Name;
            var regex2 = @"^[A-Za-z-а-яА-Я]+$";
            var str3 = client.Email;
            var regex3 = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";
            var match = Regex.Match(str, regex);
            var match2 = Regex.Match(str2, regex2);
            var match3 = Regex.Match(str3, regex3);
            if (!match.Success)
            {
                return BadRequest(new { Phone = "issue" });
            }
            if (!match2.Success)
            {
                return BadRequest(new { Name = "issue" });
            }
            if (!match3.Success)
            {
                return BadRequest(new { Email = "issue" });
            }
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var cl = _context.Clients.FirstOrDefault(p => p.Email == client.Email);
            if (cl != null)
            {
                return BadRequest(new { Email = "Такий email вже існує!" });
            }
            var cli = _context.Clients.FirstOrDefault(p => p.Phone == client.Phone);
            if (cli != null)
            {
                return BadRequest(new { Phone = "Такий номер вже існує!" });
            }
            var prod = _context.Clients.SingleOrDefault(p => p.Id == client.Id);
            if (prod != null)
            {
                prod.Name = client.Name;
                prod.Email = client.Email;
                prod.Phone = client.Phone;
                _context.SaveChanges();
            }
            return Ok();
        }
    }
}