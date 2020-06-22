using System.Collections.Generic;

namespace WebServerCursova.ViewModels
{
    public class FilterValueVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
   
    public class FilterVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<FilterValueVM> Children { get; set; }
    }   
}
