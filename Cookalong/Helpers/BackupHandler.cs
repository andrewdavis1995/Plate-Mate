using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;

namespace Cookalong.Helpers
{
    internal static class BackupHandler
    {
        const string BACKUP_PATH = "Backup";
        const int RETAIN_DAYS = 30;
        const int MAX_FILES = 20;
        const long MAX_ZIP_FILE_SIZE = 200000;

        /// <summary>
        /// Saves the current saved data to a zip folder
        /// </summary>
        public static void SaveBackup()
        {
            var location = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);

            // ensure backup directory exists
            var fullPath = Path.Combine(string.IsNullOrEmpty(location) ? String.Empty : location, BACKUP_PATH);
            Directory.CreateDirectory(fullPath);

            if (Directory.Exists(DataPath_()))
            {
                // copy to zip
                ZipFile.CreateFromDirectory(DataPath_(), Path.Combine(fullPath, DateTime.Now.ToString("ddMMyy-HHmmss")) + ".zip");
            }
        }

        /// <summary>
        /// Returns the path to the saved data
        /// </summary>
        /// <returns>Path to data</returns>
        static string DataPath_()
        {
            var location = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);
            return Path.Combine(string.IsNullOrEmpty(location) ? string.Empty : location, "SavedData");
        }

        /// <summary>
        /// Returns the path to the temp folder
        /// </summary>
        /// <returns>Path to temp folder</returns>
        static string TempPath_()
        {
            var location = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);
            return Path.Combine(string.IsNullOrEmpty(location) ? String.Empty : location, "TEMP");
        }

        /// <summary>
        /// Restores the specified backup file
        /// </summary>
        /// <param name="path">The path to restore from</param>
        /// <returns>Whether it was successful</returns>
        public static bool RestoreBackup_(string path)
        {
            bool success = false;

            try
            {
                // tidy up by deleting temp
                if (Directory.Exists(TempPath_()))
                    Directory.Delete(TempPath_(), true);

                // ensure the target path exist
                Directory.CreateDirectory(DataPath_());

                // copy existing data into temp folder
                Directory.Move(DataPath_(), TempPath_());

                // check file exists
                if (File.Exists(path) && new FileInfo(path).Length < MAX_ZIP_FILE_SIZE)
                {
                    // unzip specified path into data folder
                    ZipFile.ExtractToDirectory(path, DataPath_());
                }

                // tidy up by deleting temp
                Directory.Delete(TempPath_(), true);

                success = true;
            }
            catch (Exception)
            {
                // copy data back from temp folder
                if (Directory.Exists(TempPath_()))
                    Directory.Move(TempPath_(), DataPath_());
            }

            return success;
        }

        /// <summary>
        /// Load all files from the backup folder
        /// </summary>
        /// <returns>A list of valid backup files</returns>
        public static List<string> GetBackups()
        {
            var list = new List<string>();

            // ensure backup directory exists
            var location = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);
            var fullPath = Path.Combine(string.IsNullOrEmpty(location) ? String.Empty : location, BACKUP_PATH);
            if (Directory.Exists(fullPath))
            {
                // get all files, and delete ones outwith the valid range
                var files = Directory.GetFiles(fullPath);
                list = DeleteOldData_(files);
            }

            return list;
        }

        /// <summary>
        /// Deletes any files that are older than permitted range
        /// </summary>
        /// <param name="files">All files that were found</param>
        /// <returns>List of files that remain after check</returns>
        private static List<string> DeleteOldData_(string[] files)
        {
            var valid = new List<string>();

            // loop through all files (sorted by date)
            foreach (var file in files.OrderByDescending(f => File.GetLastWriteTime(f)))
            {
                // get the write time of the files
                var date = File.GetLastWriteTime(file);

                // if older than valid time (or too many already), delete
                if ((DateTime.Now - date).TotalDays >= RETAIN_DAYS || valid.Count >= MAX_FILES)
                {
                    File.Delete(file);
                }
                else
                {
                    // otherwise, add to list to return
                    valid.Add(file);
                }
            }

            return valid;
        }
    }
}
