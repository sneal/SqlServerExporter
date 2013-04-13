using Sneal.SqlExporter.Core.Preconditions;

namespace Sneal.SqlExporter.Core
{
    public class ConnectionSettings : IConnectionSettings
    {
        private string password;
        private string serverName;
        private bool useIntegratedAuthentication;
        private string userName;

        #region IConnectionSettings Members

        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        public string ServerName
        {
            get { return serverName; }
            set
            {
                Throw.If(value).IsEmpty();
                serverName = value;
            }
        }

        public bool UseIntegratedAuthentication
        {
            get { return useIntegratedAuthentication; }
            set { useIntegratedAuthentication = value; }
        }

        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }

        #endregion
    }
}