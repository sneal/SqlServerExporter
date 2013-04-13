using System;

namespace Sneal.SqlExporter.Core
{
    public interface IScriptWriter
    {
        string ExportDirectory { get; set; }

        event EventHandler<ScriptWrittenEventArgs> ScriptWrittenEvent;

        void WriteIndexScript(string objectName, string sql);
        void WriteConstraintScript(string objectName, string sql);
        void WriteTableScript(string objectName, string sql);
        void WriteViewScript(string objectName, string sql);
        void WriteSprocScript(string objectName, string sql);
        void WriteTableDataScript(string objectName, string sql);
    }
}