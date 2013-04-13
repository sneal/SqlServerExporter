using Microsoft.SqlServer.Management.Smo;
using Sneal.SqlExporter.Core.Preconditions;

namespace Sneal.SqlExporter.Core
{
    public class DifferencingScriptEngine : ScriptEngine
    {
        private readonly Database targetDatabase;

        public DifferencingScriptEngine(Database database, Database targetDatabase)
            : base(database)
        {
            Throw.If(targetDatabase).IsNull();
            this.targetDatabase = targetDatabase;
        }

        /// <summary>
        /// Exports the T-SQL scripts to disk.
        /// </summary>
        public virtual void ExportUpgradeScripts(IExportParams exportParameters, IScriptWriter scriptWriter)
        {
            Throw.If(exportParameters).IsNull();
            Throw.If(scriptWriter).IsNull();

            exportParams = exportParameters;
            writer = scriptWriter;

            CalculateScriptObjectCount();

            ScriptTableDataDifferences();
        }

        protected void ScriptTableDataDifferences()
        {
            if (!exportParams.ScriptDataAsSql)
                return;

            foreach (string tableName in exportParams.TablesToScriptData)
            {
                Table table = database.Tables[tableName];
                if (table == null)
                {
                    string msg = string.Format(
                        "Cannot find the table {0} in the source database {1}",
                        tableName, database.Name);
                    throw new SqlExporterException(msg);
                }

                Table targetTable = targetDatabase.Tables[tableName];
                if (targetTable == null)
                {
                    string msg = string.Format(
                        "Cannot find the table {0} in the target database {1}",
                        tableName, targetDatabase.Name);
                    throw new SqlExporterException(msg);
                }

                // get table PK
            }
        }
    }
}
