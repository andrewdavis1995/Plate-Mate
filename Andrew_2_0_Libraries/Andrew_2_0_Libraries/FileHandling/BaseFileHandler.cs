using Andrew_2_0_Libraries.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Andrew_2_0_Libraries.FileHandling
{
    internal abstract class BaseFileHandler
    {
        const string BASE_FILE_PATH = "SavedData";
        internal abstract string GetFilePath();

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
            
            var fullPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), BASE_FILE_PATH, GetFilePath() + ".txt");

            // ensure file exists
            if (File.Exists(fullPath))
            {
                // read file
                var sr = new StreamReader(fullPath);
                var line = sr.ReadLine();
                while (line != null)
                {
                    line = line.Replace("\\n", "\n").Replace("\\r", "\r");

                    // parse data
                    var t = new T();
                    if (t.ParseData(line))
                        list.Add(t);

                    // next line
                    line = sr.ReadLine();
                }
                sr.Close();
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

            var sw = new StreamWriter(fullPath + ".txt");

            // write each item
            foreach (var item in list)
                sw.WriteLine(item.GetTextOutput());

            sw.Close();
        }
    }
}
