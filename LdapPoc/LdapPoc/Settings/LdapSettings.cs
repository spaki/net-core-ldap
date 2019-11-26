namespace LdapPoc.Settings
{
    public class LdapSettings
    {
        public string LdapServer { get; set; }
        public string DomainComponents { get; set; }
        public string LdapUser { get; set; }
        public string LdapPassword { get; set; }
        public LdapPropertiesSettings Properties { get; set; }
    }
}
