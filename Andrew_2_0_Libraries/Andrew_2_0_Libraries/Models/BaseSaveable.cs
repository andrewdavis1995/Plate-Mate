namespace Andrew_2_0_Libraries.Models
{
    /// <summary>
    /// Base class to allow items to be saved
    /// </summary>
    public abstract class BaseSaveable
    {
        /// <summary>
        /// Get a text output for the object
        /// </summary>
        /// <returns></returns>
        public abstract string GetTextOutput();

        /// <summary>
        /// Parses the supplied data into the object
        /// </summary>
        /// <param name="data">The data to parse</param>
        /// <returns>Whether the parse was successful</returns>
        public abstract bool ParseData(string data);
    }
}
