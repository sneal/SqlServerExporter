using System;
using System.Collections.Generic;
using Microsoft.SqlServer.Management.Smo;
using Sneal.SqlExporter.Core.Preconditions;

namespace Sneal.SqlExporter.Core
{
    public class ExportSessionFactory
    {
        private readonly IConnectionSettings _settings;
        private SqlServerConnection _connection;

        public ExportSessionFactory(IConnectionSettings settings)
        {
            Throw.If(settings).IsNull();
            Throw.If(settings.ServerName).IsEmpty();
            _settings = settings;
        }

        public IExportSession CreateExportSession(string databaseName)
        {
            Throw.If(databaseName).IsNullOrEmpty();

            Database database;

            try
            {
                database = GetConnection().GetDatabase(databaseName);
            }
            catch (Exception ex)
            {
                throw new SqlExporterConnectionException(
                    string.Format("Could not find or connect to the database {0} on server {1}",
                    databaseName, _settings.ServerName), ex);
            }

            if (database == null)
            {
                throw new SqlExporterConnectionException(
                    string.Format("Could not find or connect to the database {0} on server {1}",
                    databaseName, _settings.ServerName));
            }

            return new ExportSession(database);
        }

        public IList<string> GetDatabaseNames()
        {
            try
            {
                return GetConnection().GetDatabaseNames();
            }
            catch (Exception ex)
            {
                string msg = string.Format(
                    "There was an error while trying to connect to enumerate the databases on server {0}",
                    _settings.ServerName);
                throw new SqlExporterException(msg, ex);                
            }
        }

        private SqlServerConnection GetConnection()
        {
            return _connection ?? (_connection = CreateConnection());
        }

        private SqlServerConnection CreateConnection()
        {
            try
            {
                var connection = _settings.UseIntegratedAuthentication ?
                    new SqlServerConnection(_settings.ServerName) :
                    new SqlServerConnection(_settings.ServerName, _settings.UserName, _settings.Password);

                connection.Connect();

                return connection;
            }
            catch (Exception ex)
            {
                string msg = string.Format(
                    "There was an error while trying to connect to the SQL Server {0}",
                    _settings.ServerName);
                throw new SqlExporterConnectionException(msg, ex);
            }
        }
    }
}