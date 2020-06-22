using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ShopCarApi.ViewModels
{

    public class MakeVM
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
    public class MakeAddVM
    {
        [Required(ErrorMessage = "Поле не може бути пустим")]
        public string Name { get; set; }
    }
    public class MakeDeleteVM
    {
        public int Id { get; set; }
    }


}
