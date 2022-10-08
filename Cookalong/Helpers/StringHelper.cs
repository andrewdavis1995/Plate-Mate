using System;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Cookalong.Helpers
{
    /// <summary>
    /// Functions to format output strings
    /// </summary>
    internal static class StringHelper
    {
        /// <summary>
        /// Displays the duration in a user-friendly manner
        /// </summary>
        /// <param name="v">The time in minutes</param>
        /// <returns>A formatted output string</returns>
        public static string GetTimeString(uint v)
        {
            string output = "";

            // if no time, report so
            if (v == 0)
            {
                output = "No time specified";
            }
            else
            {
                // break into time components
                var hours = v / 60;
                var minutes = v - hours * 60;

                // construct string
                if (hours > 0) output += hours + " hours";
                if (minutes > 0)
                {
                    if (hours > 0) output += ", ";
                    output += minutes + " minutes";
                }
            }

            return output;
        }

        /// <summary>
        /// Checks if the provided string contains a forbidden character
        /// </summary>
        /// <param name="text">The string to check</param>
        /// <returns>Whether the string contains a forbidden character</returns>
        internal static bool ForbiddenCharacter(string text)
        {
            // don't allow these characters
            char[] forbidden = { '@', '~', '\"', '#' };
            return forbidden.Any(c => text.Contains(c));
        }

        /// <summary>
        /// Checks if the space key is allowed
        /// </summary>
        /// <param name="text">The current string before space is potentially added</param>
        /// <returns>Whether a space is valid</returns>
        internal static bool IsSpaceAllowed(string text)
        {
            var valid = true;

            // don't allow space if nothing else
            if (text.Length == 0)
                valid = false;
            // don't allow double space
            else if (text.Last() == ' ')
                valid = false;

            return valid;
        }

        /// <summary>
        /// Gets the output for the time in a nicer format - different to the standard one
        /// </summary>
        /// <param name="totalSeconds">The total number of seconds</param>
        /// <returns>Formatted string</returns>
        public static string TimeConfigOutput(int totalSeconds, bool basic = false)
        {
            // break into individual components
            var hours = (totalSeconds / 60) / 60;
            var minutes = (totalSeconds - (hours * 60 * 60)) / 60;
            var seconds = totalSeconds - (hours * 60 * 60) - (minutes * 60);

            // construct string
            var str = new StringBuilder();

            if (basic)
            {
                var totalMinutes = (hours * 60) + minutes;
                str.Append(totalMinutes.ToString("00:"));
                str.Append(seconds.ToString("00"));
            }
            else
            {
                if (hours > 0) str.Append(hours + "hrs ");
                str.Append(minutes + "mins ");
                str.Append(seconds + "s");
            }

            return str.ToString().Trim();
        }
    }
}
