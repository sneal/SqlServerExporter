using System;

namespace Sneal.SqlExporter
{
    [Serializable]
    public class UserPrefs
    {
        private string database;
        private string exportDirectory;
        private string server;

        public string Server
        {
            get { return server; }
            set { server = value; }
        }

        public string Database
        {
            get { return database; }
            set { database = value; }
        }

        public string ExportDirectory
        {
            get { return exportDirectory; }
            set { exportDirectory = value; }
        }
    }
}