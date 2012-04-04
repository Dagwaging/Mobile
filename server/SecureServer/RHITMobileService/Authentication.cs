﻿using System;
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
            var toRemove = _activeTokens.Where(kvp => kvp.Value > now).Select(kvp => kvp.Key);
            foreach (string authToken in toRemove)
            {
                _activeTokens.Remove(authToken);
            }
        }

        public bool IsAuthenticated(string authToken)
        {
            RemoveExpiredTokens();

            if (!_activeTokens.ContainsKey(authToken))
                return false;

            return true;
        }

        public void Invalidate(string authToken)
        {
            _activeTokens.Remove(authToken);
        }

        public string AuthenticateUser(string username, string password, out DateTime expiration)
        {
            if (!_ldap.Authenticate(username, password))
            {
                expiration = DateTime.MinValue;
                return null;
            }

            string authToken = Guid.NewGuid().ToString("N");
            expiration = DateTime.UtcNow.AddHours(1);
            _activeTokens.Add(authToken, expiration);
            return authToken;
        }
    }

    class LdapAuthentication
    {
        private string _path;
        private string _filterAttribute;

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

                DirectorySearcher search = new DirectorySearcher(entry);

                search.Filter = "(SAMAccountName=" + username + ")";
                search.PropertiesToLoad.Add("cn");
                SearchResult result = search.FindOne();

                if (null == result)
                {
                    return false;
                }

                //Update the new path to the user in the directory.
                _path = result.Path;
                _filterAttribute = (string)result.Properties["cn"][0];
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}
