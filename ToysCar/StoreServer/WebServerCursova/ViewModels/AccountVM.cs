using System.ComponentModel.DataAnnotations;

namespace WebServerCursova.ViewModels
{
    public class LoginVM
    {
        [EmailAddress(ErrorMessage = "Має бути пошта!")]
        [Required(ErrorMessage = "Поле не може бути порожнім")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Поле не може бути порожнім")]
        public string Password { get; set; }
    }
    public class RegistrationVM
    {
        [Required(ErrorMessage = "Поле FirstName не може бути порожнім")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Поле LastName не може бути порожнім")]
        public string LastName { get; set; }

        [EmailAddress(ErrorMessage = "Має бути пошта!")]
        [Required(ErrorMessage = "Поле Email не може бути порожнім")]
        public string Email { get; set; }

        [RegularExpression(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?!.*\s).{6,24}$",
            ErrorMessage = "Пароль має мати більше 6 символів і містити цифру, велику і малу літеру")]
        [Required(ErrorMessage = "Поле Password не може бути порожнім")]
        public string Password { get; set; }

        [RegularExpression(@"^[+]\d{12}$",
            ErrorMessage = "В телефоні має бути 12 цифр")]
        [Required(ErrorMessage = "Поле Phone не може бути порожнім")]
        public string Phone { get; set; }
    }
}
