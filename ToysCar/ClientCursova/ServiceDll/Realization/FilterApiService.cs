using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ServiceDll.Interfaces;
using ServiceDll.Models;

namespace ServiceDll.Realization
{
    public class FilterApiService : IFilterService
    {
        private readonly string _url = "https://localhost:44329/api/filters";
        public List<FilterModel> GetFilters()
        {
            WebClient client = new WebClient
            {
                Encoding = Encoding.UTF8
            };

            string data = client.DownloadString(_url);

            var list = JsonConvert.DeserializeObject<List<FilterModel>>(data);
            return list;
        }

        public Task<List<FilterModel>> GetFiltersAsync()
        {
            return Task.Run(() => GetFilters());
        }
    }
}
