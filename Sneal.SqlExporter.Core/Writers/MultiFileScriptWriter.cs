using System;
using System.Diagnostics;
using System.IO;
using Sneal.SqlExporter.Core.Engine;
using Sneal.SqlExporter.Core.Preconditions;

namespace Sneal.SqlExporter.Core.Writers
{
    internal class MultiFileScriptWriter : IScriptWriter
    {
        private string exportDirectory;

        public MultiFileScriptWriter(string exportDirectory)
        {
            this.exportDirectory = exportDirectory;
        }

        #region IScriptWriter Members

        public event EventHandler<ScriptWrittenEventArgs> ScriptWrittenEvent;

        public virtual string ExportDirectory
        {
            get { return exportDirectory; }
            set { exportDirectory = value; }
        }

        public virtual void WriteIndexScript(string objectName, string sql)
        {
            WriteScript(objectName, ExportObjectType.Index, sql);
        }

        public virtual void WriteConstraintScript(string objectName, string sql)
        {
            WriteScript(objectName, ExportObjectType.Constraint, sql);
        }

        public virtual void WriteTableScript(string objectName, string sql)
        {
            WriteScript(objectName, ExportObjectType.Table, sql);
        }

        public virtual void WriteViewScript(string objectName, string sql)
        {
            WriteScript(objectName, ExportObjectType.View, sql);
        }

        public virtual void WriteSprocScript(string objectName, string sql)
        {
            WriteScript(objectName, ExportObjectType.Sproc, sql);
        }

        public virtual void WriteTableDataScript(string objectName, string sql)
        {
            WriteScript(objectName, ExportObjectType.Data, sql);
        }

        #endregion

        protected virtual void OnScriptWrittenEvent(ScriptWrittenEventArgs e)
        {
            Throw.If(e).IsNull();

            Trace.WriteLine("Wrote " + e.ScriptPath);

            EventHandler<ScriptWrittenEventArgs> evt = ScriptWrittenEvent;
            if (evt != null)
            {
                evt(this, e);
            }
        }

        protected virtual void WriteScript(string objectName, ExportObjectType objectType, string sql)
        {
            Throw.If(objectName).IsEmpty();

            if (sql != null)
                sql = sql.Trim();

            // write sproc to file
            string dir = Path.Combine(exportDirectory, objectType.ToString());
            string scriptPath = Path.Combine(dir, objectName + ".sql");

            if (!string.IsNullOrEmpty(sql))
            {
                try
                {
                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);

                    if (File.Exists(scriptPath))
                        File.SetAttributes(scriptPath, FileAttributes.Normal);

                    using (StreamWriter scriptFile = new StreamWriter(scriptPath, false))
                    {
                        scriptFile.Write(sql);
                    }
                }
                catch (Exception ex)
                {
                    string msg = string.Format("Could not write the script file {0} to disk.",
                                               scriptPath);
                    throw new SqlExporterException(msg, ex);
                }

                OnScriptWrittenEvent(new ScriptWrittenEventArgs(scriptPath));
            }
        }
    }
}