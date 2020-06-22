using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ShopCarApi.ViewModels
{
    public class ModelVM
    {
        public int Id { get; set; }

        public MakeVM Make { get; set; }
        [Required(ErrorMessage = "Поле не може бути пустим")]
        public string Name { get; set; }
    }
    public class ModelAddVM
    {
        [Required(ErrorMessage = "Поле не може бути пустим")]
        public MakeVM Make { get; set; }
        [Required(ErrorMessage = "Поле не може бути пустим")]
        public string Name { get; set; }
    }
    public class ModelDeleteVM
    {
        public int Id { get; set; }
    }
}
