using System.Collections.Generic;

namespace LdapPoc.Ldap
{
    public class AuthResult
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public IEnumerable<string> Groups { get; set; }
        public bool Success { get; set; }
        public string AuthMessage { get; set; }
    }
}
