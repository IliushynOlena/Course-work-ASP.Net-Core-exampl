using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ShopCarApi.ViewModels
{

    public class UserVM
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

    }

    public class UserLoginVM
    {
        [Required(ErrorMessage = "Поле не може бути пустим")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Поле не може бути пустим")]
        public string Password { get; set; }
    }

    public class UserRegisterVM
    {
        [Required(ErrorMessage = "Поле не може бути пустим")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Поле не може бути пустим")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Поле не може бути пустим")]
        public string Password { get; set; }
    }
    public class UserUpdateVM
    {
        [Required(ErrorMessage = "Поле не може бути пустим")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Поле не може бути пустим")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Поле не може бути пустим")]
        public string Email { get; set; }
    }
    public class UserDeleteVM
    {
        [Required(ErrorMessage = "Поле не може бути пустим")]
        public int Id { get; set; }
    }
}
