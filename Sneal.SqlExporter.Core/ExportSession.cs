using System;
using System.Collections.Generic;
using Microsoft.SqlServer.Management.Smo;
using Sneal.SqlExporter.Core.Preconditions;

namespace Sneal.SqlExporter.Core
{
    public class ExportSession : IExportSession
    {
        private readonly SqlServerConnection connection;
        private readonly Database database;
        private bool disposed;

        public ExportSession(SqlServerConnection connection, string databaseName)
        {
            Throw.If(connection).IsNull();
            Throw.If(databaseName).IsEmpty();

            this.connection = connection;

            try
            {
                database = connection.GetDatabase(databaseName);
            }
            catch (Exception ex)
            {
                throw new SqlExporterConnectionException(
                    string.Format("Could not find or connect to the database {0} on server {1}",
                    databaseName, connection.Name), ex);                
            }

            if (database == null)
            {
                throw new SqlExporterConnectionException(
                    string.Format("Could not find or connect to the database {0} on server {1}",
                    databaseName, connection.Name));
            }
        }

        /// <summary>
        /// The database 
        /// </summary>
        public string DatabaseName
        {
            get { return database.Name; }
        }

        /// <summary>
        /// This event fires each time a script is written to disk.
        /// </summary>
        public event EventHandler<ProgressEventArgs> ProgressEvent;

        public IList<string> GetUserTables()
        {
            List<string> tables = new List<string>();
            foreach (Table table in database.Tables)
            {
                if (!table.IsSystemObject)
                    tables.Add(table.Name);
            }

            return tables;
        }

        public IList<string> GetUserSprocs()
        {
            List<string> sprocs = new List<string>();
            foreach (StoredProcedure sproc in database.StoredProcedures)
            {
                if (!sproc.IsSystemObject && !sproc.Name.StartsWith("dt"))
                    sprocs.Add(sproc.Name);
            }

            return sprocs;
        }

        public IList<string> GetUserViews()
        {
            List<string> views = new List<string>();
            foreach (View view in database.Views)
            {
                if (!view.Name.StartsWith("sys"))
                {
                    views.Add(view.Name);
                }
            }

            return views;
        }

        public void Export(string exportDirectory, IExportParams exportParams)
        {
            ScriptEngine engine = null;
            try
            {
                var scriptWriter = new ScriptWriterFactory().Create(exportParams);

                engine = new ScriptEngine(database);
                engine.ProgressEvent += Engine_ProgressEvent;
                engine.ExportScripts(exportParams, scriptWriter);
            }
            finally
            {
                if (engine != null)
                    engine.ProgressEvent -= Engine_ProgressEvent;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void OnProgressEvent(ProgressEventArgs e)
        {
            EventHandler<ProgressEventArgs> evt = ProgressEvent;
            if (evt != null)
            {
                evt(this, e);
            }
        }

        private void Engine_ProgressEvent(object sender, ProgressEventArgs e)
        {
            // forward the event from the engine to session listeners
            OnProgressEvent(e);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed && disposing)
            {
                if (connection != null)
                    connection.Dispose();
            }

            disposed = true;
        }
    }
}