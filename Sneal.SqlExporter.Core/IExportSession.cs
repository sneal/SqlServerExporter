using System;
using System.Collections.Generic;

namespace Sneal.SqlExporter.Core
{
    public interface IExportSession
    {
        string DatabaseName { get; }

        /// <summary>
        /// This event fires each time a script is written to disk.
        /// </summary>
        event EventHandler<ProgressEventArgs> ProgressEvent;

        IList<string> GetUserTables();
        IList<string> GetUserSprocs();
        IList<string> GetUserViews();

        void Export(IExportParams exportParams);
    }
}