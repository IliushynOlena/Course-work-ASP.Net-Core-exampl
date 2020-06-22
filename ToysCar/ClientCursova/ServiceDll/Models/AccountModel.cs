namespace ServiceDll.Models
{
    public class AccountModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
    public class UserModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Role { get; }
    }
}
