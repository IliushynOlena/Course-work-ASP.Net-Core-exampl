using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ShopCarApi.ViewModels
{
    public class ClientDataGridVM
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }
        public string UniqueName { get; set; }

        public string Email { get; set; }

    }
    public class ClientVM
    {

        public int Id { get; set; }

        [Required(ErrorMessage = "Поле не може бути пустим")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Поле не може бути пустим")]
        public string Phone { get; set; }
        public string UniqueName { get; set; }
        public string Image { get; set; }
        [Required(ErrorMessage = "Поле не може бути пустим")]
        public string Email { get; set; }

    }

    public class ClientAddVM
    {
        [Required(ErrorMessage = "Поле не може бути пустим")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Поле не може бути пустим")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "Поле не може бути пустим")]
        public string UniqueName { get; set; }
        [Required(ErrorMessage = "Виберіть фото клієнта!")]
        public string Image { get; set; }
        [Required(ErrorMessage = "Поле не може бути пустим")]
        public string Email { get; set; }
    }
    public class ClientDeleteVM
    {
        public int Id { get; set; }
    }
}
