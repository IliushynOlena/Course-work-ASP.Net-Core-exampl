using System.Collections.Generic;
using System.Linq;
using WebServerCursova.Entities;
using WebServerCursova.Helpers;
using WebServerCursova.ViewModels;

namespace WebServerCursova.Services
{
    public class CategoriesService
    {
        public static IList<CategoryVM> GetCategoryList(EFDbContext context)
        {
            var categoryList = context.Categories
                 .Select(c => new CategoryVM
                 {
                     Id = c.Id,
                     Name = c.Name,
                     ParentId = c.ParentId
                 })
                 .ToList();

            var categoryTree = BuildTreeFromNode.BuildTree(categoryList);
            return categoryTree;
        }
    }
}
