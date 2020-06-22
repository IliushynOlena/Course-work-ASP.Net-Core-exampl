using System.Text;
using WebServerCursova.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Http;
using WebServerCursova.Helpers;

namespace WebServerCursova
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<EFDbContext>(opt => opt
                .UseSqlServer(Configuration
                    .GetConnectionString("DefaultConnection")));

            ////////////////////////////////////////////////////////////////////
            //////////// Прописуємо настройки токена

            //Прописуємо настройки Identity
            services.AddIdentity<DbUser, DbRole>(options => options.Stores.MaxLengthForKeys = 120)
                .AddEntityFrameworkStores<EFDbContext>()
                .AddDefaultTokenProviders();

            //вказуємо, який ключ буде для шифрування токена
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("this is the secrete phrase"));

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredUniqueChars = 0;
            });

            //Прописуємо налаштування для роботи jwt-tokenа
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(cfg =>
            {
                //если равно false, то SSL при отправке токена не используется
                cfg.RequireHttpsMetadata = false;
                cfg.SaveToken = true;
                //параметры валидации токена
                cfg.TokenValidationParameters = new TokenValidationParameters()
                {
                    // установка ключа безопасности, которым подписывается токен
                    IssuerSigningKey = signingKey,
                    // надо ли валидировать ключ безопасности
                    ValidateIssuerSigningKey = true,
                    // будет ли валидироваться потребитель токена
                    ValidateAudience = false,
                    // укзывает, будет ли валидироваться издатель при валидации токена
                    ValidateIssuer = true,
                    // будет ли валидироваться время существования
                    ValidateLifetime = true
                };
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseMvc();
            app.UseHttpsRedirection();
            // дозвіл на авторизацію
            app.UseAuthentication();
            // дозвіл серверу на роботу з файлами
            app.UseStaticFiles();

            ////////////////////////////////////////////////////////////////////
            //////////// Прописуємо настройки роботи з фото

            string dirPathSave = ImageHelper.GetImageFolder(env, this.Configuration);

            // вказуємо серверу як називатиметься папка з фото
            string imageUrl = "/images";

            app.UseStaticFiles(new StaticFileOptions()
            {
                // вказуємо папку
                FileProvider = new PhysicalFileProvider(dirPathSave),
                // вказуємо url
                RequestPath = new PathString(imageUrl)
            });

            SeederDb.SeedData(app.ApplicationServices, env, this.Configuration);
        }
    }
}
