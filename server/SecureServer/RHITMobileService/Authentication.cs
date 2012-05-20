using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.DirectoryServices;

namespace RHITMobile.Secure
{
    class Authentication
    {
        private LdapAuthentication _ldap;

        private Dictionary<string, DateTime> _activeTokens;
        
        public Authentication()
        {
            _ldap = new LdapAuthentication();
            _activeTokens = new Dictionary<string, DateTime>();
        }

        private void RemoveExpiredTokens()
        {
            DateTime now = DateTime.UtcNow;
            var toRemove = _activeTokens.Where(kvp => now >= kvp.Value).Select(kvp => kvp.Key).ToArray();
            foreach (string authToken in toRemove)
            {
                _activeTokens.Remove(authToken);
            }
        }

        public bool IsAuthenticated(string authToken)
        {
            lock (this)
            {
                RemoveExpiredTokens();

                if (!_activeTokens.ContainsKey(authToken))
                    return false;

                return true;
            }
        }

        public int ActiveUsers
        {
            get
            {
                lock (this)
                {
                    RemoveExpiredTokens();

                    return _activeTokens.Count;
                }
            }
        }

        public void Invalidate(string authToken)
        {
            lock (this)
            {
                _activeTokens.Remove(authToken);
            }
        }

        public string AuthenticateUser(string username, string password, out DateTime expiration)
        {
            if (!_ldap.Authenticate(username, password))
            {
                expiration = DateTime.MinValue;
                return null;
            }

            lock (this)
            {
                string authToken = Guid.NewGuid().ToString("N");
                expiration = DateTime.UtcNow.AddHours(1);
                _activeTokens.Add(authToken, expiration);
                return authToken;
            }
        }
    }

    class LdapAuthentication
    {
        private string _path;

        public LdapAuthentication()
        {
            _path = "LDAP://DC=rose-hulman,DC=edu";
        }

        public bool Authenticate(string username, string password)
        {
            string domain = "ROSE-HULMAN.EDU";
            string domainAndUsername = domain + @"\" + username;
            DirectoryEntry entry = new DirectoryEntry(_path, domainAndUsername, password);

            try
            {
                //Bind to the native AdsObject to force authentication.
                object obj = entry.NativeObject;

                return obj != null;
            }
            catch (Exception ex)
            {
                WindowsService.Instance.Logger.Error("Failed to authenticate user", ex);
                return false;
            }
        }
    }
}
