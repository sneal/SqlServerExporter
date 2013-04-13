using System;

namespace Sneal.SqlExporter.Core
{
    public class ScriptWrittenEventArgs : EventArgs
    {
        private readonly string scriptPath;

        public ScriptWrittenEventArgs(string scriptPath)
        {
            this.scriptPath = scriptPath;
        }

        public string ScriptPath
        {
            get { return scriptPath; }
        }
    }
}