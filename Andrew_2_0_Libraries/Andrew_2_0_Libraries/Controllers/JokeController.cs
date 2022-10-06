using Andrew_2_0_Libraries.FileHandling;
using Andrew_2_0_Libraries.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Andrew_2_0_Libraries.Controllers
{
    public class JokeController
    {
        List<Joke> _jokes;
        //List<ItemComment> _comments;

        JokeFileHandler _fileHandler = new JokeFileHandler();
        //JokeCommentFileHandler _commentFileHandler = new JokeCommentFileHandler();

        /// <summary>
        /// Constructor
        /// </summary>
        public JokeController()
        {
            // load jokes
            _jokes = _fileHandler.ReadFile<Joke>();

            // load comments
            //_comments = _commentFileHandler.ReadFile<ItemComment>();

            // add comments to matching jokes
            //foreach (var c in _comments)
            //{
            //    // look for match
            //    var match = _jokes.Where(j => j.GetJokeId() == c.GetOwnerId()).FirstOrDefault();
            //    if (match != null)
            //        match.AddComment(c);
            //}
        }

        /// <summary>
        /// Adds a new joke
        /// </summary>
        /// <param name="jokeText">The content of the joke</param>
        public void AddJoke(string jokeText)
        {
            // add a new joke
            var joke = new Joke(jokeText);
            _jokes.Add(joke);

            Save_();
        }

        /// <summary>
        /// Updates an existing recipe
        /// </summary>
        /// <param name="jokeId">The ID of the joke to update</param>
        /// <param name="jokeText">The text of the joke</param>
        public void UpdateJoke(Guid jokeId, string jokeText)
        {
            // find recipe with correct ID
            var matching = _jokes.FirstOrDefault(r => r.GetJokeId() == jokeId);
            if (matching != null)
            {
                matching.UpdateJoke(jokeText);
            }
            else
            {
                // if unable to find the matching joke, add a new one
                AddJoke(jokeText);
            }
            Save_();
        }

        /// <summary>
        /// Deletes the specified joke
        /// </summary>
        /// <param name="recipeId">The ID of the joke to delete</param>
        public void DeleteJoke(Guid jokeId)
        {
            for (int i = 0; i < _jokes.Count; i++)
            {
                // find and remove the specified joke
                if (_jokes[i].GetJokeId() == jokeId)
                {
                    _jokes.RemoveAt(i);
                    break;
                }
            }
            Save_();
        }


        /// <summary>
        /// Gets a list of all jokes
        /// </summary>
        /// <returns>List of all jokes</returns>
        public List<Joke> GetJokes() { return _jokes; }

        /// <summary>
        /// Gets a list of all jokes which match the specified filter
        /// </summary>
        /// <param name="filter">The filter to use in the search</param>
        /// <returns>List of all jokes that match the filter</returns>
        public List<Joke> GetJokes(string filter)
        {
            return _jokes.Where(r => r.MatchesFilter(filter)).ToList();
        }

        /// <summary>
        /// Adds a comment to the specified joke
        /// </summary>
        /// <param name="jokeId">The ID of the joke to add a comment to</param>
        /// <param name="comment">The comment to add</param>
        public void AddComment(Guid jokeId, string comment)
        {
            AddComment(jokeId, comment, DateTime.Now);
        }

        /// <summary>
        /// Adds a comment to the specified joke
        /// </summary>
        /// <param name="jokeId">The ID of the joke to add a comment to</param>
        /// <param name="comment">The comment to add</param>
        /// <param name="date">The date of the comment</param>
        public void AddComment(Guid jokeId, string comment, DateTime date)
        {
            var joke = _jokes.FirstOrDefault(r => r.GetJokeId() == jokeId);
            if (joke != null)
            {
                // add a comment
                var newComment = new ItemComment(jokeId, date, comment);
                joke.AddComment(newComment);
                //_comments.Add(newComment);
                Save_();
            }
        }

        /// <summary>
        /// Removes the specified comment from the specified joke
        /// </summary>
        /// <param name="jokeId">The ID of the joke to remove from</param>
        /// <param name="commentId">The ID of the comment to remove</param>
        public void RemoveComment(Guid jokeId, Guid commentId)
        {
            // get matching joke and remove comment
            var joke = _jokes.FirstOrDefault(r => r.GetJokeId() == jokeId);
            if (joke != null)
                joke.RemoveComment(commentId);

            // get updated comment list
            var comments = new List<ItemComment>();
            foreach (var j in _jokes)
                comments.AddRange(j.GetComments());

            Save_();
        }

        /// <summary>
        /// Saves all jokes and comments to file
        /// </summary>
        void Save_()
        {
            _fileHandler.WriteFile(_jokes);
            //_commentFileHandler.WriteFile(_comments);
        }
    }
}
