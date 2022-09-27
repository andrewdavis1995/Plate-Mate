using System;
using System.Collections.Generic;

namespace Andrew_2_0_Libraries.Models
{
    public class HappyZone
    {
        float _completion;
        string _name;
        Guid _zoneId;
        DateTime _lastUpdate;
        List<ItemComment> _comments;

        #region Accessor functions
        public float GetCompletion() { return _completion; }
        public string GetZoneName() { return _name; }
        public Guid GetZoneId() { return _zoneId; }
        public DateTime GetLastUpdate() { return _lastUpdate; }
        public List<ItemComment> GetComments() { return _comments; }
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="zoneName">The name of the target</param>
        public HappyZone(string zoneName)
        {
            _zoneId = Guid.NewGuid();
            _completion = 0f;
            UpdateValues(zoneName);
            _comments = new List<ItemComment>();
        }

        /// <summary>
        /// Update the values of the target
        /// </summary>
        /// <param name="zoneName">The name of the zone</param>
        public void UpdateValues(string zoneName)
        {
            _name = zoneName;
        }

        /// <summary>
        /// Update the completeness of the zone
        /// </summary>
        /// <param name="value">Satisfaction rating of the happy zone</param>
        /// <param name="updateTime">When the last update was</param>
        public void UpdateValue(float value, DateTime updateTime)
        {
            _completion = value;
            _lastUpdate = updateTime;
        }

        /// <summary>
        /// Adds a comment to the happy zone
        /// </summary>
        /// <param name="date">When the comment was added</param>
        /// <param name="comment">The content of the comment</param>
        public void AddComment(DateTime date, string comment)
        {
            _comments.Add(new ItemComment(_zoneId, date, comment));
        }

        /// <summary>
        /// Removes the specified comment from the zone
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
    }
}
