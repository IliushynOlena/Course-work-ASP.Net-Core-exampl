using ServiceDll.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServiceDll.Interfaces
{
    interface IFilterService
    {
        List<FilterModel> GetFilters();
        Task<List<FilterModel>> GetFiltersAsync();
    }
}
