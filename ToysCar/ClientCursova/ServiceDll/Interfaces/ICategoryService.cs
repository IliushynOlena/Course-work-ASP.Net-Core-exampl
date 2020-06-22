using ServiceDll.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServiceDll.Interfaces
{
    interface ICategoryService
    {
        List<CategoryModel> GetCategories();
        Task<List<CategoryModel>> GetCategoriesAsync();
    }
}
