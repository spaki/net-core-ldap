namespace LdapPoc.Models
{
    public class UserModel
    {
        public string User { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
        public string ReturnUrl { get; set; }
        public string Message { get; set; }
    }
}
