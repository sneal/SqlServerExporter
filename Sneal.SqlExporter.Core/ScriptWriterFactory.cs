namespace Sneal.SqlExporter.Core
{
    public class ScriptWriterFactory
    {
        public IScriptWriter Create(IExportParams exportParams)
        {
            if (exportParams.UseMultipleFiles)
            {
                return new MultiFileScriptWriter(exportParams.ExportDirectory);
            }
            return new SingleFileScriptWriter(exportParams.ExportDirectory, exportParams.DatabaseName);
        }
    }
}
