namespace Cookalong.Helpers
{
    /// <summary>
    /// Functions to format output strings
    /// </summary>
    internal class StringHelper
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
    }
}
