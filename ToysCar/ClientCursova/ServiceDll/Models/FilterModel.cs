using System.Collections.Generic;

namespace ServiceDll.Models
{
    public class FilterValueModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class FilterModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<FilterValueModel> Children { get; set; }
    }
}
