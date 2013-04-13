using System;
using System.Collections.Generic;
using Microsoft.SqlServer.Management.Smo;
using Sneal.SqlExporter.Core.Preconditions;
using Sneal.SqlExporter.Core.Writers;

namespace Sneal.SqlExporter.Core.Engine
{
    internal class ExportSession : IExportSession
    {
        private readonly Database _database;

        internal ExportSession(Database database)
        {
            Throw.If(database).IsNull();
            _database = database;
        }

        /// <summary>
        /// The database 
        /// </summary>
        public string DatabaseName
        {
            get { return _database.Name; }
        }

        /// <summary>
        /// This event fires each time a script is written to disk.
        /// </summary>
        public event EventHandler<ProgressEventArgs> ProgressEvent;

        public IList<string> GetUserTables()
        {
            List<string> tables = new List<string>();
            foreach (Table table in _database.Tables)
            {
                if (!table.IsSystemObject)
                {
                    tables.Add(table.Name);
                }
            }

            return tables;
        }

        public IList<string> GetUserSprocs()
        {
            List<string> sprocs = new List<string>();
            foreach (StoredProcedure sproc in _database.StoredProcedures)
            {
                if (!sproc.IsSystemObject && !sproc.Name.StartsWith("dt"))
                {
                    sprocs.Add(sproc.Name);
                }
            }

            return sprocs;
        }

        public IList<string> GetUserViews()
        {
            List<string> views = new List<string>();
            foreach (View view in _database.Views)
            {
                if (!view.Name.StartsWith("sys"))
                {
                    views.Add(view.Name);
                }
            }

            return views;
        }

        public void Export(IExportParams exportParams)
        {
            ScriptEngine engine = null;
            try
            {
                var scriptWriter = new ScriptWriterFactory().Create(
                    exportParams.ExportDirectory, DatabaseName, exportParams.UseMultipleFiles);

                engine = new ScriptEngine(_database);
                engine.ProgressEvent += Engine_ProgressEvent;
                engine.ExportScripts(exportParams, scriptWriter);
            }
            finally
            {
                if (engine != null)
                {
                    engine.ProgressEvent -= Engine_ProgressEvent;
                }
            }
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
    }
}