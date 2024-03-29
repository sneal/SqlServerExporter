using System;
using System.Collections.Generic;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using Sneal.SqlExporter.Core.Preconditions;

namespace Sneal.SqlExporter.Core.Engine
{
    /// <summary>
    /// Connects to stuff
    /// </summary>
    public class SqlServerConnection
    {
        private readonly string _serverName;
        private readonly string _userName;
        private readonly string _password;
        private Server _server;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerConnection"/> class.
        /// Use this constructor when needing to use integrated authentication under
        /// a different user, i.e. a different NT user then this app is running
        /// under.
        /// </summary>
        /// <param name="server">The server.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        public SqlServerConnection(string server, string userName, string password)
        {
            Throw.If(server).IsNullOrEmpty();
               
            _serverName = server;
            _userName = userName;
            _password = password;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerConnection"/> class.
        /// This constructor assumes NT Integrated Authentication.
        /// </summary>
        /// <param name="server">The server.</param>
        public SqlServerConnection(string server)
            : this(server, null, null)
        { }

        /// <summary>
        /// Gets a database reference.
        /// </summary>
        /// <param name="name">The name of the database to get.</param>
        /// <returns></returns>
        public Database GetDatabase(string name)
        {
            AssertConnectWasCalled();
            return _server.Databases[name];
        }

        public IList<string> GetDatabaseNames()
        {
            AssertConnectWasCalled();

            var names = new List<string>();
            foreach (Database db in _server.Databases)
            {
                names.Add(db.Name);
            }
            return names;
        }

        private void AssertConnectWasCalled()
        {
            if (_server == null)
            {
                throw new InvalidOperationException(
                    "You must first call Connect before attempting any operations");
            }
        }

        public string Name
        {
            get
            {
                AssertConnectWasCalled();
                return _server.Name;
            }
        }

        public void Connect()
        {
            var serverConnection = string.IsNullOrEmpty(_userName)
                ? new ServerConnection(_serverName)
                : new ServerConnection(_serverName, _userName, _password);

            _server = new Server(serverConnection);
        }
    }
}
