using WebServerCursova.Entities;
using WebServerCursova.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebServerCursova.Helpers;

namespace WebServerCursova.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        public readonly string secretePhrase = "this is the secrete phrase";

        private readonly EFDbContext _context;
        // створює юзера
        private readonly UserManager<DbUser> _userManager;
        // логінить юзера
        private readonly SignInManager<DbUser> _signInManager;

        private readonly RoleManager<DbRole> _roleManager;

        public AccountController(UserManager<DbUser> userManager, SignInManager<DbUser> signInManager, RoleManager<DbRole> roleManager, EFDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
        }


        /////////////////////////////////////////////////////////////////////////////////////////
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginVM model)
        {
            // логіним юзера, який прийшов параметром як модель
            var result = await _signInManager
                .PasswordSignInAsync(model.Email, model.Password, false, false);

            if (!result.Succeeded)
            {
                return BadRequest(new { invalid = "Юзера не знайдено" });
            }
            // якщо все ок - шукаємо юзера по емейлу в базі
            var user = await _userManager.FindByEmailAsync(model.Email);
            // і логінимо його
            await _signInManager.SignInAsync(user, isPersistent: false);

            user = GetUserProfile(user);
            // видаємо юзеру токен авторизації
            return Ok(
                new
                {
                    token = CreateTokenJWT(user)
                });
        }

        private DbUser GetUserProfile(DbUser user)
        {
            foreach (var profile in _context.UserProfiles)
            {
                if (profile.DbUserId == user.Id)
                {
                    user.UserProfile = profile;
                    break;
                }
            }

            return user;
        }


        /////////////////////////////////////////////////////////////////////////////////////////
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] string userEmail)
        {
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null)
            {
                return BadRequest();
            }
            await _signInManager.SignOutAsync();

            return Ok();
        }


        /////////////////////////////////////////////////////////////////////////////////////////
        [HttpPost("registration")]
        public async Task<IActionResult> Registration([FromBody] RegistrationVM model)
        {
            List<string> err = new List<string>();

            // перевіряємо модель на валідність
            if (!ModelState.IsValid)
            {
                var errors = CustomValidator.GetErrorsByModel(ModelState);
                return BadRequest(errors);
            }

            // створюємо роль адмін
            string roleAdmin = "Admin";

            // шукаємо роль в базі. Якщо немає - додаємо
            var role = _roleManager.FindByNameAsync(roleAdmin).Result;
            if (role == null)
            {
                role = new DbRole
                {
                    Name = roleAdmin
                };

                var addRoleResult = _roleManager.CreateAsync(role).Result;
            }

            // шукаємо юзера в базі по імейлу. якщо немає - додаємо
            var user = _userManager.FindByNameAsync(model.Email).Result;
            if (user == null)
            {
                user = new DbUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                };

                var result = _userManager.CreateAsync(user, model.Password).Result;
                // якщо додало - додаємо роль
                if (result.Succeeded)
                {
                    result = _userManager.AddToRoleAsync(user, roleAdmin).Result;
                    // логінимо юзера
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    // передаємо модель в БД      
                    UserProfile up = new UserProfile
                    {
                        DbUserId = user.Id,
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Phone = model.Phone
                    };

                    user.UserProfile = up;

                    _context.UserProfiles.Add(up);
                    _context.SaveChanges();

                    return Ok(
                   new
                   {
                       token = CreateTokenJWT(user)
                   });
                }
            }
            else
            {
                err.Add("Така пошта вже зареєстрована!");
            }

            return BadRequest(err);
        }


        // метод, який буде видавати JWT-токен
        private string CreateTokenJWT(DbUser user)
        {
            var claims = new List<Claim>()
            {
                new Claim ("Id", user.Id.ToString()),
                new Claim ("Email", user.Email),
            };

            if (user.UserProfile != null)
            {
                claims.Add(new Claim("FirstName", user.UserProfile.FirstName));
                claims.Add(new Claim("LastName", user.UserProfile.LastName));
                claims.Add(new Claim("Phone", user.UserProfile.Phone));
            }

            // отримуємо всі ролі про юзера
            var roles = _userManager.GetRolesAsync(user).Result;
            foreach (var role in roles)
            {
                claims.Add(new Claim("Role", role));
            }

            // шифруємо токен 
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretePhrase));
            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            var jwt = new JwtSecurityToken(
                signingCredentials: signingCredentials,
                claims: claims,
                expires: DateTime.Now.AddHours(1));

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
    }
}
