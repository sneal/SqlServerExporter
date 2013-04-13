using System;
using System.IO;
using System.Security;
using System.Xml.Serialization;

namespace Sneal.SqlExporter
{
    public class UserPrefsRepository
    {
        private readonly XmlSerializer serializer = new XmlSerializer(typeof (UserPrefs));

        /// <summary>
        /// Full path to the user preferences file.
        /// </summary>
        public virtual string ConfigFilePath
        {
            get
            {
                string configDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                configDir = Path.Combine(configDir, "SqlExporter");
                return Path.Combine(configDir, "userSettings.cfg");
            }
        }

        /// <summary>
        /// Loads the preferences from disk or returns a default user pref
        /// instance if the file does not exist.
        /// </summary>
        /// <returns>The loaded preferences, or default instance.</returns>
        public virtual UserPrefs LoadUserPrefs()
        {
            if (File.Exists(ConfigFilePath))
            {
                using (StreamReader sr = new StreamReader(ConfigFilePath))
                {
                    return serializer.Deserialize(sr) as UserPrefs;
                }
            }

            return new UserPrefs();
        }

        public virtual void SaveUserPrefs(UserPrefs prefs)
        {
            EnsureDirectoryExists();

            try
            {
                using (StreamWriter sr = new StreamWriter(ConfigFilePath))
                {
                    serializer.Serialize(sr, prefs);
                }
            }
            catch (UnauthorizedAccessException) { }
            catch (SecurityException) { }
        }

        private void EnsureDirectoryExists()
        {
            try
            {
                string dir = Path.GetDirectoryName(ConfigFilePath);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
            }
            catch (UnauthorizedAccessException) { }
            catch (IOException) {}
        }
    }
}