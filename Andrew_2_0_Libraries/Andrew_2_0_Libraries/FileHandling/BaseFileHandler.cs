using Andrew_2_0_Libraries.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Reflection;

namespace Andrew_2_0_Libraries.FileHandling
{
    internal abstract class BaseFileHandler
    {
        const string BASE_FILE_PATH = "SavedData";
        internal abstract string GetFilePath();

        FileEncryption _encryption = new FileEncryption();

        /// <summary>
        /// Constructor
        /// </summary>
        protected BaseFileHandler()
        {
            var fullPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), BASE_FILE_PATH);

            // ensure folders exist
            Directory.CreateDirectory(BASE_FILE_PATH);
            Directory.CreateDirectory(fullPath);
        }

        /// <summary>
        /// Reads items from file
        /// </summary>
        /// <returns>A list of all objects loaded from file</returns>
        public List<T> ReadFile<T>() where T : BaseSaveable, new()
        {
            var list = new List<T>();

            var fullPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), BASE_FILE_PATH, GetFilePath());

            // ensure file exists
            if (File.Exists(fullPath))
            {
                var data = string.Empty;
                var success = _encryption.Read(fullPath, ref data);

                // read file
                var lines = data.Split(Environment.NewLine);
                foreach (var line in lines)
                {
                    string fixedLine = line.Replace("\\n", "\n").Replace("\\r", "\r");

                    // parse data
                    var t = new T();
                    if (t.ParseData(fixedLine))
                        list.Add(t);
                }
            }

            return list;
        }

    /// <summary>
    /// Writes the supplied list of objects to file
    /// </summary>
    /// <param id="list">The list of objects to save</param>
    public void WriteFile<T>(List<T> list) where T : BaseSaveable
    {
        var fullPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), BASE_FILE_PATH, GetFilePath());
        var strings = list.Select(i => i.GetTextOutput()).ToList();

        _encryption.WriteList(fullPath, strings);
    }
}
}
