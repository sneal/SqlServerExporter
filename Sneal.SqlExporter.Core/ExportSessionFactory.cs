using System;
using System.Collections.Generic;
using Sneal.Preconditions;

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

            CreateConnection();
        }

        protected void CreateConnection()
        {
            try
            {
                _connection = _settings.UseIntegratedAuthentication ?
                    new SqlServerConnection(_settings.ServerName) :
                    new SqlServerConnection(_settings.ServerName, _settings.UserName, _settings.Password);

                _connection.Connect();
            }
            catch (Exception ex)
            {
                string msg = string.Format(
                    "There was an error while trying to connect to the SQL Server {0}",
                    _settings.ServerName);
                throw new SqlExporterConnectionException(msg, ex);
            }            
        }

        public virtual IExportSession CreateExportSession(string database)
        {
            Throw.If(database).IsEmpty();

            try
            {
                IExportSession exportSession = new ExportSession(_connection, database);
                return exportSession;
            }
            catch (Exception ex)
            {
                string msg = string.Format(
                    "There was an error while trying to connect to the database {1} on server {0}",
                    database, _settings.ServerName);
                throw new SqlExporterConnectionException(msg, ex);
            }
        }

        public IList<string> GetDatabaseNames()
        {
            try
            {
                return _connection.GetDatabaseNames();
            }
            catch (Exception ex)
            {
                string msg = string.Format(
                    "There was an error while trying to connect to enumerate the databases on server {0}",
                    _settings.ServerName);
                throw new SqlExporterException(msg, ex);                
            }
        }
    }
}