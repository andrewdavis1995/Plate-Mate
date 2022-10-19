using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Andrew_2_0_Libraries.FileHandling
{
    public class FileEncryption
    {
        readonly byte[] ENCRYPTION_KEY = { 0x07, 0x04, 0x06, 0x05, 0x01, 0x07, 0x14, 0x08, 0x08, 0x04, 0x06, 0x12, 0x03, 0x05, 0x19, 0x04 };

        /// <summary>
        /// Writes a list of string to the specified file
        /// </summary>
        /// <param name="filePath">The file to write to</param>
        /// <param name="data">The data to write</param>
        public void WriteList(string filePath, List<string> data)
        {
            // build a full string
            StringBuilder sb = new StringBuilder();
            foreach (var str in data)
            {
                sb.Append(str);
                sb.AppendLine();
            }

            // write the data
            Write(filePath, sb.ToString());
        }

        /// <summary>
        /// Encrypts and writes the specified data
        /// </summary>
        /// <param name="filePath">The file to write to</param>
        /// <param name="data">The data to write</param>
        /// <returns>Whether the write was successful</returns>
        public bool Write(string filePath, string data)
        {
            bool success = false;

            try
            {
                // create file stream with data to write
                using FileStream myStream = new FileStream(filePath, FileMode.OpenOrCreate);

                // configure encryption key
                using Aes aes = Aes.Create();
                aes.Key = ENCRYPTION_KEY;
                byte[] iv = aes.IV;
                myStream.Write(iv, 0, iv.Length);

                // encrypt filestream
                using CryptoStream cryptStream = new CryptoStream(
                    myStream,
                    aes.CreateEncryptor(),
                    CryptoStreamMode.Write);

                // write to file stream
                using StreamWriter sWriter = new StreamWriter(cryptStream);
                sWriter.WriteLine(data);

                success = true;
            }
            catch (Exception ex)
            {
                // report error
                Debug.WriteLine("An error occurred : " + ex.Message);
            }

            return success;
        }

        /// <summary>
        /// Reads the data from the specified encrypted file
        /// </summary>
        /// <param name="filePath">The file to read</param>
        /// <param name="data">A variable to populate with the output data</param>
        /// <returns>Whether the read was successful</returns>
        public bool Read(string filePath, ref string data)
        {
            data = string.Empty;
            bool success = false;

            // check that the file exists
            if (File.Exists(filePath))
            {
                try
                {
                    // create file stream
                    using FileStream myStream = new FileStream(filePath, FileMode.Open);

                    // setup
                    using Aes aes = Aes.Create();
                    byte[] iv = new byte[aes.IV.Length];
                    myStream.Read(iv, 0, iv.Length);

                    // decrypt data
                    using CryptoStream cryptStream = new CryptoStream(
                       myStream,
                       aes.CreateDecryptor(ENCRYPTION_KEY, iv),
                       CryptoStreamMode.Read);

                    // read stream
                    using StreamReader sReader = new StreamReader(cryptStream);

                    // display stream
                    data = sReader.ReadToEnd();

                    success = true;
                }
                catch (Exception ex)
                {
                    // report error
                    Debug.WriteLine("An error occurred : " + ex.Message);

                    success = false;
                }
            }
            else
            {
                // no file found
                Debug.WriteLine("File does not exist");
                data = string.Empty;

                success = false;
            }

            return success;
        }
    }
}