
using System.Xml.Linq;
using System;
using System.Diagnostics;

namespace Andrew_2_0_Libraries.Models
{
    /// <summary>
    /// Class for storing methods
    /// </summary>
    public class MethodStep : BaseSaveable
    {
        string _methodText { get; set; }
        int _startTime { get; set; }
        int _duration { get; set; }

        public MethodStep() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="methodText">The text in the method</param>
        /// <param name="startTime">The time at which the method starts</param>
        /// <param name="duration">The duration of this step</param>
        public MethodStep(string methodText, int startTime, int duration)
        {
            _methodText = methodText;
            _startTime = startTime;
            _duration = duration;
        }

        /// <summary>
        /// Gets the string to write to file
        /// </summary>
        /// <returns>The string</returns>
        public override string GetTextOutput()
        {
            return _methodText + "~" + _startTime + "~" + _duration;
        }

        /// <summary>
        /// Parses the data read from file
        /// </summary>
        /// <param name="data">The data to parse</param>
        /// <returns>Whether the parse was successful</returns>
        public override bool ParseData(string data)
        {
            var ingSplit = data.Split(new string[] { "~" }, StringSplitOptions.RemoveEmptyEntries);

            // make sure there are enough entries
            if (ingSplit.Length < 1) return false;

            _methodText = ingSplit[0];
            try
            {
                _startTime = int.Parse(ingSplit[1]);
                _duration = int.Parse(ingSplit[2]);
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
                _startTime = 0;
                _duration = 0;
            }

            return true;
        }

        public string GetMethod()
        {
            return _methodText;
        }

        public int GetStart()
        {
            return _startTime;
        }
        public int GetDuration()
        {
            return _duration;
        }

        /// <summary>
        /// Updates the text for the method
        /// </summary>
        /// <param name="msg">The string to set</param>
        public void UpdateText(string msg)
        {
            _methodText = msg;
        }
    }
}
