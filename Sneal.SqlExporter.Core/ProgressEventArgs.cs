using System;

namespace Sneal.SqlExporter.Core
{
    public class ProgressEventArgs : EventArgs
    {
        private readonly string objectName;
        private readonly ushort percentDone;

        public ProgressEventArgs(ushort percentDone, string objectName)
        {
            this.percentDone = percentDone;
            this.objectName = objectName;
        }

        public ProgressEventArgs(ushort percentDone)
        {
            this.percentDone = percentDone;
        }

        public string ObjectName
        {
            get { return objectName; }
        }

        public ushort PercentDone
        {
            get { return percentDone; }
        }
    }
}