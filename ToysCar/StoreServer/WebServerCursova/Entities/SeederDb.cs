using WebServerCursova.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Collections.Generic;

namespace WebServerCursova.Entities
{
    public class SeederDb
    {
        public static void SeedUsers(LoginVM model, UserManager<DbUser> userManager, RoleManager<DbRole> roleManager)
        {
            string roleName = "Admin";
            var role = roleManager.FindByNameAsync(roleName).Result;
            if (role == null)
            {
                role = new DbRole
                {
                    Name = roleName
                };

                var addRoleResult = roleManager.CreateAsync(role).Result;
            }

            var user = userManager.FindByNameAsync(model.Email).Result;
            if (user == null)
            {
                user = new DbUser
                {
                    Email = model.Email,
                    UserName = model.Email
                };

                var result = userManager.CreateAsync(user, model.Password).Result;
                if (result.Succeeded)
                {
                    result = userManager.AddToRoleAsync(user, roleName).Result;
                }
            }
        }
        public static void SeedData(IServiceProvider services, IHostingEnvironment env, IConfiguration config)
        {
            using (var scope = services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<EFDbContext>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<DbUser>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<DbRole>>();

                SeedUsers(new LoginVM { Email = "bomba@gmail.com", Password = "Qwerty1-" }, userManager, roleManager);

                #region tblProducts - Товари
                SeedProduct(context, new DbProduct
                {
                    Name = "ИГРОВОЙ НАБОР BEYBLADE 2 ЗАПУСКАЛКИ И АРЕНА",
                    Price = 223.16M,
                    PhotoName = "ИГРОВОЙ НАБОР BEYBLADE 2 ЗАПУСКАЛКИ И АРЕНА.jpg",
                    CategoryId = 1
                });                
                SeedProduct(context, new DbProduct
                {
                    Name = "МОТОЦИКЛ M1905 СИН 1ШТ АККУМ.6V-4.5AH, В КОР.662533СМ",
                    Price = 1407.96M,
                    PhotoName = "МОТОЦИКЛ M1905 СИН 1ШТ АККУМ.6V-4.5AH, В КОР.662533СМ.jpg",
                    CategoryId = 6
                });

                #endregion

                #region tblFilterNames - Назви фільтрів

                string[] filterNames = { "Ціна", "Вид" };
                foreach (var itemName in filterNames)
                {
                    var filterName = context.FilterNames.SingleOrDefault(f => f.Name == itemName);
                    if (filterName == null)
                    {
                        context.FilterNames.Add(new FilterName { Name = itemName });
                        context.SaveChanges();
                    }
                }

                #endregion

                #region tblFilterValues - Значення фільтрів

                List<string[]> filterValues = new List<string[]> {
                    new string[] {"Менше 50 грн", "50-100грн", "100-300грн", "300-700грн", "більше 700грн" },
                    new string[] {"Активний відпочинок", "Гра на місці"}
                };

                foreach (var itemValues in filterValues)
                {
                    foreach (var itemValue in itemValues)
                    {
                        var filterValue = context.FilterValues.SingleOrDefault(f => f.Name == itemValue);
                        if (filterValue == null)
                        {
                            context.FilterValues.Add(new FilterValue { Name = itemValue });
                            context.SaveChanges();
                        }
                    }
                }
                #endregion

                #region tblFilterNameGroups - Групування по групах фільтрів

                for (int i = 0; i < filterNames.Length; i++)
                {
                    foreach (var value in filterValues[i])
                    {
                        var nId = context.FilterNames.SingleOrDefault(f => f.Name == filterNames[i]).Id;
                        var vId = context.FilterValues.SingleOrDefault(f => f.Name == value).Id;

                        var group = context.FilterNameGroups.SingleOrDefault(f => f.FilterValueId == vId && f.FilterNameId == nId);
                        if (group == null)
                        {
                            context.FilterNameGroups.Add(new FilterNameGroup
                            {
                                FilterNameId = nId,
                                FilterValueId = vId
                            });
                            context.SaveChanges();
                        }
                    }
                }

                #endregion

                #region tblFilters - Фільтри
                Filter[] filters =
                {
                        new Filter { FilterNameId = 1, FilterValueId = 3, ProductId = 64 },
                        new Filter { FilterNameId = 3, FilterValueId = 14, ProductId = 64 },
   

                        new Filter { FilterNameId = 1, FilterValueId = 5, ProductId = 67 },
                        new Filter { FilterNameId = 3, FilterValueId = 13, ProductId = 67 }
                };

                foreach (var item in filters)
                {
                    var f = context.Filters.SingleOrDefault(p => p == item);
                    if (f == null)
                    {
                        context.Filters.Add(new Filter { FilterNameId = item.FilterNameId, FilterValueId = item.FilterValueId, ProductId = item.ProductId });
                        context.SaveChanges();
                    }
                }
                #endregion

                #region tblCategories - Категорії

                SeedCategory(context, new Category
                {
                    Name = "Запускалки",
                    ParentId = null
                });

                SeedCategory(context, new Category
                {
                    Name = "Конструктори",
                    ParentId = null
                });

                SeedCategory(context, new Category
                {
                    Name = "Транспорт",
                    ParentId = null
                });
                SeedCategory(context, new Category
                {
                    Name = "Літній",
                    ParentId = 3
                });
                SeedCategory(context, new Category
                {
                    Name = "Зимовий",
                    ParentId = 3
                });
                SeedCategory(context, new Category
                {
                    Name = "Велосипеди",
                    ParentId = 4
                });
                SeedCategory(context, new Category
                {
                    Name = "Ролики",
                    ParentId = 4
                });
                SeedCategory(context, new Category
                {
                    Name = "Ковзани",
                    ParentId = 5
                });
                SeedCategory(context, new Category
                {
                    Name = "Снігокати",
                    ParentId = 5
                });

                SeedCategory(context, new Category
                {
                    Name = "Спорттовари",
                    ParentId = null
                });
                SeedCategory(context, new Category
                {
                    Name = "М'ячі",
                    ParentId = 10
                });
                SeedCategory(context, new Category
                {
                    Name = "Боксерські набори",
                    ParentId = 10
                });

                SeedCategory(context, new Category
                {
                    Name = "Зброя",
                    ParentId = null
                });
                #endregion
            }
        }

        public static void SeedProduct(EFDbContext context, DbProduct model)
        {
            var product = context.Products.SingleOrDefault(p => p.Name == model.Name);
            if (product == null)
            {
                product = new DbProduct
                {
                    Name = model.Name,
                    Price = model.Price,
                    DateCreate = DateTime.Now,
                    PhotoName = model.PhotoName
                };
                context.Products.Add(product);
                context.SaveChanges();
            }
        }
        public static void SeedCategory(EFDbContext context, Category model)
        {
            var category = context.Categories.SingleOrDefault(p => p.Name == model.Name);
            if (category == null)
            {
                category = new Category
                {
                    Name = model.Name,
                    ParentId = model.ParentId
                };
                context.Categories.Add(category);
                context.SaveChanges();
            }
        }
    }
}

