namespace LdapPoc.Ldap
{
    public interface IAuthService
    {
        AuthResult Login(string username, string password);
    }
}
