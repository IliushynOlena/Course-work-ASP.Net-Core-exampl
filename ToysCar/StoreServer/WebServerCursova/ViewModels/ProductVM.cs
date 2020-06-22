using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebServerCursova.ViewModels
{
    public class ProductGetVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string PhotoName { get; set; }
    }

    public class ProductPostVM
    {
        [Required(ErrorMessage = "Поле не може бути порожнім")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Поле не може бути порожнім")]
        public decimal Price { get; set; }
        public string PhotoBase64 { get; set; }  // вхідний параметр
        public string PhotoName { get; set; }    // вихідний параметр
        public int FilterIdType { get; set; }
        public int CategoryId { get; set; }
    }

    public class ProductDeleteVM
    {
        [Required(ErrorMessage = "Поле не може бути порожнім")]
        public int Id { get; set; }
    }

    public class ProductPutVM
    {
        [Required(ErrorMessage = "Поле не може бути порожнім")]
        public int Id { get; set; }
        [Required(ErrorMessage = "Поле не може бути порожнім")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Поле не може бути порожнім")]
        public decimal Price { get; set; }
        public string PhotoBase64 { get; set; }  // вхідний параметр
        public string PhotoName { get; set; }    // вихідний параметр
        public int FilterIdType { get; set; }
        public int CategoryId { get; set; }
    }
}
