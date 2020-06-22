using ServiceDll.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServiceDll.Interfaces
{
    public interface IProductService
    {
        //Task вертає асинхронну задачу, яка працює у вторинному потоці
        List<ProductModel> GetProducts(int categoryId, List<int> filtersId);
        Task<List<ProductModel>> GetProductsAsync(int categoryId, List<int> filtersId);


        Dictionary<string, string> Create(ProductAddModel product);
        Task<Dictionary<string, string>> CreateAsync(ProductAddModel product);

        int Delete(ProductDeleteModel product);
        Task<int> DeleteAsync(ProductDeleteModel product);

        ProductEditModel EditGetById(int id);
        Dictionary<string, string> EditSave(ProductEditModel model);
        Task<Dictionary<string, string>> EditSaveAsync(ProductEditModel product);
    }
}
