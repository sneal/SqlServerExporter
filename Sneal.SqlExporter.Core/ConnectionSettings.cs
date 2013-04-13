using Sneal.SqlExporter.Core.Preconditions;

namespace Sneal.SqlExporter.Core
{
    public class ConnectionSettings : IConnectionSettings
    {
        private string serverName;

        public string ServerName
        {
            get { return serverName; }
            set
            {
                Throw.If(value).IsNullOrEmpty();
                serverName = value;
            }
        }

        public string Password { get; set; }

        public bool UseIntegratedAuthentication { get; set; }

        public string UserName { get; set; }
    }
}