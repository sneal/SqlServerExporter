﻿namespace Sneal.SqlExporter.Core
{
    public class ScriptWriterFactory
    {
        public IScriptWriter Create(string exportDirectory, string databaseName, bool useMultipleFiles)
        {
            if (useMultipleFiles)
            {
                return new MultiFileScriptWriter(exportDirectory);
            }
            return new SingleFileScriptWriter(exportDirectory, databaseName);
        }
    }
}
