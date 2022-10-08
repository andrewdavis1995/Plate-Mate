using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Andrew_2_0_Libraries.Models
{
    public class Joke : BaseSaveable
    {
        string _jokeContent;
        Guid _jokeId;
        List<ItemComment> _comments;

        #region Accessor functions
        public string GetJokeContent() { return _jokeContent; }
        public Guid GetJokeId() { return _jokeId; }
        public List<ItemComment> GetComments() { return _comments; }
        #endregion

        public Joke()
        {
            _comments = new List<ItemComment>();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="jokeContent">The content of the joke</param>
        public Joke(string jokeContent)
        {
            _jokeContent = jokeContent;
            _comments = new List<ItemComment>();
            _jokeId = Guid.NewGuid();
        }

        /// <summary>
        /// Updates this joke with new information
        /// </summary>
        /// <param name="jokeContent">The content of the joke</param>
        public void UpdateJoke(string content)
        {
            _jokeContent = content;
        }

        /// <summary>
        /// Adds a comment to the joke
        /// </summary>
        /// <param name="comment">The comment to add to the joke</param>
        public void AddComment(ItemComment comment)
        {
            _comments.Add(comment);
        }

        /// <summary>
        /// Whether this joke matches the specified filter
        /// </summary>
        /// <param name="filter">The filter to use</param>
        /// <returns>Whether the joke matches the filter</returns>
        public bool MatchesFilter(string filter)
        {
            // joke matches the filter if the name of it, or any of its ingredients, matches the search string
            return _jokeContent.ToLower().Contains(filter.ToLower());
        }

        /// <summary>
        /// Removes the specified comment from the joke
        /// </summary>
        /// <param name="commentId">The ID of the comment to remove</param>
        public void RemoveComment(Guid commentId)
        {
            for (int i = 0; i < _comments.Count; i++)
            {
                // find and remove comment which matches the comment ID
                if (_comments[i].GetOwnerId() == commentId)
                {
                    _comments.RemoveAt(i);
                    break;
                }
            }
        }

        /// <summary>
        /// Gets the output to be written to the save file
        /// </summary>
        /// <returns>File output</returns>
        public override string GetTextOutput()
        {
            return _jokeId.ToString() + "@" + _jokeContent;
        }

        /// <summary>
        /// Parses the data into a Target object
        /// </summary>
        /// <param name="data">The data to parse</param>
        public override bool ParseData(string data)
        {
            bool success = false;
            var split = data?.Split('@');

            if (split.Length > 1)
            {
                try
                {
                    _jokeId = Guid.Parse(split[0]);
                    _jokeContent = split[1];
                    success = true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    success = false;
                }
            }

            return success;
        }
    }
}
