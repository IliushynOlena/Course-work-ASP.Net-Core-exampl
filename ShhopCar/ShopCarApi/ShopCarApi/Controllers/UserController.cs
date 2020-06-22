using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ShopCarApi.Entities;
using ShopCarApi.ViewModels;
using WebElectra.Entities;

namespace ShopCarApi.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly EFDbContext _context;
        private readonly UserManager<DbUser> _userManager;
        private readonly SignInManager<DbUser> _signInManager;
        private readonly RoleManager<DbRole> _roleManager;

        public UserController(EFDbContext context,
          UserManager<DbUser> userManager,
          SignInManager<DbUser> signInManager, RoleManager<DbRole> roleManager)
        {
            _userManager = userManager;
            _context = context;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public IActionResult UserList()
        {

            var _user = _context.Users.Select(
                p => new UserVM
                {
                    Id = p.Id,
                    Name = p.UserName,
                    Email = p.Email

                }).ToList();
            return Ok(_user);
        }

        [HttpGet("search")]
        public IActionResult UserList(UserVM employee)
        {
            var query = _context.Users.AsQueryable();
            if (!String.IsNullOrEmpty(employee.Email))
            {
                query = query.Where(e => e.Email.Contains(employee.Email));
            }
            if (!String.IsNullOrEmpty(employee.Name))
            {
                query = query.Where(e => e.UserName.Contains(employee.Name));
            }

            var users = query.Select(p => new UserVM
            {
                Id = p.Id,
                Name = p.UserName,
                Email = p.Email

            }).ToList();
            return Ok(users);
        }

        [HttpDelete]
        public IActionResult Delete([FromBody] UserDeleteVM duser)
        {
            if (!ModelState.IsValid)
            {
                var errors = CustomValidator.GetErrorsByModel(ModelState);
                return BadRequest(errors);
            }
            var user = _context.Users.SingleOrDefault(p => p.Id == duser.Id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
            return Ok();
        }

        [HttpPut]
        public IActionResult Update([FromBody] UserUpdateVM user)
        {
            if (!ModelState.IsValid)
            {
                var errors = CustomValidator.GetErrorsByModel(ModelState);
                return BadRequest(errors);
            }

            var str_name = user.Name;
            var name_regex = @"^[A-Za-z-а-яА-Я]+$";

            var str_email = user.Email;
            var email_regex = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";

            var match_name = Regex.Match(str_name, name_regex);

            var match_email = Regex.Match(str_email, email_regex);

            if (!match_name.Success)
            {
                return BadRequest(new { Name = "Неправильний формат поля." });
            }

            if (!match_email.Success)
            {
                return BadRequest(new { Email = "Неправильний формат поля." });
            }

            var emp = _context.Users.SingleOrDefault(p => p.Id == user.Id);
            if (emp != null)
            {

                emp.UserName = user.Name;
                emp.Email = user.Email;
                _context.SaveChanges();
                return Ok("Дані оновлено");

            }
            return BadRequest(new { Email = "Помилка оновлення" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]UserLoginVM model)
        {
            if (!ModelState.IsValid)
            {
                var errors = CustomValidator.GetErrorsByModel(ModelState);
                return BadRequest(errors);
            }
            var result = await _signInManager
                .PasswordSignInAsync(model.Name, model.Password,
                false, false);

            if (!result.Succeeded)
            {
                return BadRequest(new { Password = "Не правильно введені дані!" });
            }

            var user = await _userManager.FindByNameAsync(model.Name);
            await _signInManager.SignInAsync(user, isPersistent: false);

            return Ok(
            new
            {
                token = CreateTokenJwt(user)
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]UserRegisterVM model)
        {
            if (!ModelState.IsValid)
            {
                var errors = CustomValidator.GetErrorsByModel(ModelState);
                return BadRequest(errors);
            }
            var str_name = model.Name;
            var name_regex = @"^[A-Za-z-а-яА-Я]+$";

            var str_email = model.Email;
            var email_regex = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";

            var match_name = Regex.Match(str_name, name_regex);

            var match_email = Regex.Match(str_email, email_regex);

            if (!match_name.Success)
            {
                return BadRequest(new { Name = "Неправильний формат поля." });
            }

            if (!match_email.Success)
            {
                return BadRequest(new { Email = "Неправильний формат поля." });
            }

            string roleName = "Employee";
            var role = _roleManager.FindByNameAsync(roleName).Result;
            if (role == null)
            {
                role = new DbRole { Name = roleName };
            }
            var userEmail = model.Email;
            //var user = _userManager.FindByEmailAsync(userEmail).Result;
            if (_userManager.FindByEmailAsync(userEmail).Result != null)
            {
                return BadRequest(new { Email = "Така електронна пошта вже існує!" });
            }
            var user = new DbUser
            {
                Email = userEmail,
                UserName = model.Name
            };
            //user.UserRoles = new List<DbRole>();
            var result = _userManager.CreateAsync(user, model.Password).Result;
            if (!result.Succeeded)
            {
                return BadRequest(new { Password = "Не правильно введені дані!" });
            }
            result = _userManager.AddToRoleAsync(user, roleName).Result;

            if (!result.Succeeded)
            {
                return BadRequest(new { Password = "Проблеми при створенні користувача!" });
            }

            return Ok(
            new
            {
                token = CreateTokenJwt(user)
            });
        }


        private string CreateTokenJwt(DbUser user)
        {
            var roles = _userManager.GetRolesAsync(user).Result;
            var claims = new List<Claim>()
            {
                //new Claim(JwtRegisteredClaimNames.Sub, user.Id)
                new Claim("id", user.Id.ToString()),
                new Claim("name", user.UserName)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim("roles", role));
            }

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("11-sdfasdf-22233222222"));
            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            var jwt = new JwtSecurityToken(
                signingCredentials: signingCredentials,
                claims: claims,
                expires: DateTime.Now.AddHours(1));

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

    }
}