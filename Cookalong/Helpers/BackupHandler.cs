using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Cookalong.Helpers
{
    internal static class BackupHandler
    {
        const string BACKUP_PATH = "Backup";
        const int RETAIN_DAYS = 30;
        const int MAX_FILES = 20;

        /// <summary>
        /// Saves the current saved data to a zip folder
        /// </summary>
        public static void SaveBackup()
        {
            // ensure backup directory exists
            var fullPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), BACKUP_PATH);
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
            return Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "SavedData");
        }

        /// <summary>
        /// Returns the path to the temp folder
        /// </summary>
        /// <returns>Path to temp folder</returns>
        static string TempPath_()
        {
            return Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "TEMP");
        }


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

                // unzip specified path into data folder
                ZipFile.ExtractToDirectory(path, DataPath_());

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
            var fullPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), BACKUP_PATH);
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
            List<string> valid = new List<string>();

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
