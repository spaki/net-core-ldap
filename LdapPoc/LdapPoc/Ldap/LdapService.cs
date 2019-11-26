using LdapPoc.Settings;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.DirectoryServices;
using System.Linq;

namespace LdapPoc.Ldap
{
    public class LdapService : IAuthService
    {
        private readonly LdapSettings ldapSettings;

        public LdapService(AppSettings appSettings)
        {
            ldapSettings = appSettings.Ldap;
        }

        public AuthResult Login(string username, string password)
        {
            var result = new AuthResult();

            if(string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                result.AuthMessage = "User not found!";
                return result;
            }

            try
            {
                var ldapPath = BuildLdapPath(ldapSettings.LdapServer, ldapSettings.DomainComponents);
                var ldapFormatedUser = $"cn={ldapSettings.LdapUser},{ldapSettings.DomainComponents}";

                using (var adminEntry = new DirectoryEntry(ldapPath, ldapFormatedUser, ldapSettings.LdapPassword, AuthenticationTypes.ServerBind))
                {

                    if (!IsDirectoryEntryValid(adminEntry))
                    {
                        result.AuthMessage = "Fail to access authentication server.";
                        return result;
                    }

                    using (var searcher = new DirectorySearcher(adminEntry))
                    {
                        searcher.Filter = $"(uid={username})";
                        searcher.PropertiesToLoad.Add("*");

                        var searchResult = searcher.FindOne();

                        if (searchResult == null)
                        {
                            result.AuthMessage = "User not found!";
                            return result;
                        }

                        var formatedUsername = $"uid={username},{ldapSettings.DomainComponents}";

                        using (var userEntry = new DirectoryEntry(ldapPath, formatedUsername, password, AuthenticationTypes.ServerBind))
                        {
                            if (!IsDirectoryEntryValid(adminEntry))
                            {
                                result.AuthMessage = "User not found!";
                                return result;
                            }

                            string userData = JsonConvert.SerializeObject(searchResult.Properties);
                            Debug.WriteLine(userData);

                            result.UserId = searchResult.Properties[ldapSettings.Properties.UserId]?.OfType<string>().FirstOrDefault();
                            result.DisplayName = searchResult.Properties[ldapSettings.Properties.DisplayName]?.OfType<string>().FirstOrDefault();
                            result.Email = searchResult.Properties[ldapSettings.Properties.Email]?.OfType<string>().FirstOrDefault();
                            result.Groups = searchResult.Properties[ldapSettings.Properties.Groups]?.OfType<string>();
                            result.Success = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.AuthMessage = $"Authentication error: {ex}";
            }

            return result;
        }

        private string BuildLdapPath(string ldapServer, string domainComponents)
        {
            var result = ldapServer;

            if (!ldapServer.EndsWith("/"))
                result += "/";

            result += domainComponents;

            return result;
        }

        private bool IsDirectoryEntryValid(DirectoryEntry directoryEntry) => directoryEntry?.NativeObject != null;
    }
}
