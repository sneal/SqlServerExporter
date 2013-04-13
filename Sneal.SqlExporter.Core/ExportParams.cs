using System.Collections.Generic;

namespace Sneal.SqlExporter.Core
{
    public class ExportParams : IExportParams
    {
        private readonly List<string> _sprocsToScript = new List<string>();
        private readonly List<string> _tablesToScript = new List<string>();
        private readonly List<string> _tablesToScriptData = new List<string>();
        private readonly List<string> viewsToScript = new List<string>();

        public ExportParams()
        {
            UseMultipleFiles = true;
        }

        public bool ScriptTableSchema { get; set; }

        public bool ScriptTableConstraints { get; set; }

        public bool ScriptTableIndexes { get; set; }

        public bool ScriptDataAsSql { get; set; }

        public bool ScriptDataAsXml { get; set; }

        public bool UseMultipleFiles { get; set; }

        public string ExportDirectory { get; set; }

        public IList<string> TablesToScriptData
        {
            get { return _tablesToScriptData; }
        }

        public IList<string> SprocsToScript
        {
            get { return _sprocsToScript; }
        }

        public IList<string> TablesToScript
        {
            get { return _tablesToScript; }
        }

        public IList<string> ViewsToScript
        {
            get { return viewsToScript; }
        }
    }
}