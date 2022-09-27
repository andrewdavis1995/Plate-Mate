using System;
using System.Globalization;

namespace Andrew_2_0_Libraries.Models
{
    public class ItemComment : BaseSaveable
    {
        const string DATE_FORMAT = "dd/MM/yyyy";

        Guid _ownerId;
        DateTime _date;
        string _comment;
        // TODO: support photo comments
        
        #region Accessor functions
        public Guid GetOwnerId() { return _ownerId; }
        public DateTime GetDate() { return _date; }
        public string GetComment() { return _comment; }
        #endregion

        public ItemComment() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ownerId">The ID of the item this belongs to</param>
        /// <param name="date">The date of the comment</param>
        /// <param name="comment">The content of the comment</param>
        public ItemComment(Guid ownerId, DateTime date, string comment)
        {
            _ownerId = ownerId;
            _date = date;
            _comment = comment;
        }

        /// <summary>
        /// Gets the output to be written to the save file
        /// </summary>
        /// <returns>File output</returns>
        public override string GetTextOutput()
        {
            return _ownerId.ToString() + "@" + _date.ToString(DATE_FORMAT) + "@" + _comment;
        }

        /// <summary>
        /// Parses the data into a Target object
        /// </summary>
        /// <param name="data">The data to parse</param>
        public override bool ParseData(string data)
        {
            bool success = false;
            var split = data?.Split('@');

            // only valid if we have enough data
            if (split.Length > 2)
            {
                try
                {
                    // parse data
                    _ownerId = Guid.Parse(split[0]);
                    _date = DateTime.ParseExact(split[1], DATE_FORMAT, CultureInfo.InvariantCulture);
                    _comment = split[2];
                    success = true;
                }
                catch(Exception ex)
                {
                    // exception
                    Console.WriteLine(ex.Message);
                    success = false;
                }
            }

            return success;
        }
    }
}
