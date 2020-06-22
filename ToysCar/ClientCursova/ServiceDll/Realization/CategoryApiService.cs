using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ServiceDll.Interfaces;
using ServiceDll.Models;


namespace ServiceDll.Realization
{
    public class CategoryApiService : ICategoryService
    {
        private readonly string _url = "https://localhost:44329/api/category";
        public List<CategoryModel> GetCategories()
        {
            WebClient client = new WebClient
            {
                Encoding = Encoding.UTF8
            };

            string data = client.DownloadString(_url);

            var list = JsonConvert.DeserializeObject<List<CategoryModel>>(data);
            return list;
        }

        public Task<List<CategoryModel>> GetCategoriesAsync()
        {
            return Task.Run(() => GetCategories());
        }
    }
}
