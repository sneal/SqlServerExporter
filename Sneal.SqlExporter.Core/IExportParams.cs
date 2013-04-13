using System.Collections.Generic;

namespace Sneal.SqlExporter.Core
{
    public interface IExportParams
    {
        bool ScriptTableConstraints { get; }
        bool ScriptTableIndexes { get; }
        bool ScriptTableSchema { get; }
        bool ScriptDataAsSql { get; }
        bool ScriptDataAsXml { get; }

        bool UseMultipleFiles { get; }
        string ExportDirectory { get; }

        IList<string> TablesToScriptData { get; }
        IList<string> SprocsToScript { get; }
        IList<string> TablesToScript { get; }
        IList<string> ViewsToScript { get; }
    }
}