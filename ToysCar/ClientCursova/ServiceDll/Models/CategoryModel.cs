using System.Collections.Generic;

namespace ServiceDll.Models
{
    public class CategoryModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? ParentId { get; set; }
        public List<CategoryModel> Children { get; set; }
    }
}
