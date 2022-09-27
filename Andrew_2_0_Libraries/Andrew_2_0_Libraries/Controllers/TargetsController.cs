using System.Collections.Generic;
using Andrew_2_0_Libraries.Models;
using System;
using System.Linq;

namespace Andrew_2_0_Libraries.Controllers
{
    public enum HappyZoneTypes { Fitness, Job, SocialLife, Cooking, Finance };

    public class TargetsController
    {
        static string[] _zoneTypes = new string[] { "Fitness", "Job", "SocialLife", "Cooking", "Finance" };
        // TODO: support more zones

        List<Target> _targets;
        HappyZone[] _happyZones;

        /// <summary>
        /// Constructor
        /// </summary>
        public TargetsController()
        {
            InitialiseHappyZones_();
            _targets = new List<Target>();
        }

        /// <summary>
        /// Initialises the happy zones to blank zones
        /// </summary>
        private void InitialiseHappyZones_()
        {
            // initialise happy zones
            var numHappyZones = Enum.GetValues(typeof(HappyZoneTypes)).Length;
            _happyZones = new HappyZone[numHappyZones];

            // initialise each one
            for (int i = 0; i < numHappyZones; i++)
                _happyZones[i] = new HappyZone(_zoneTypes[i]);
        }

        /// <summary>
        /// Adds a new target
        /// </summary>
        /// <param name="targetName">The name of the new target</param>
        public void AddTarget(string targetName)
        {
            var target = new Target(targetName);
            _targets.Add(target);
        }

        /// <summary>
        /// Updates an existing target
        /// </summary>
        /// <param name="targetId">The ID of the target to update</param>
        /// <param name="targetName">The name of the target</param>
        public void UpdateTarget(Guid targetId, string targetName)
        {
            // find target with correct ID
            var matching = _targets.Where(t => t.GetTargetId() == targetId).FirstOrDefault();
            if (matching != null)
            {
                matching.UpdateValues(targetName);
            }
            else
            {
                // if unable to find the matching target, add a new one
                AddTarget(targetName);
            }
        }

        /// <summary>
        /// Deletes the specified target
        /// </summary>
        /// <param name="targetId">The ID of the target to delete</param>
        public void DeleteTransaction(Guid targetId)
        {
            for (int i = 0; i < _targets.Count; i++)
            {
                // find and remove the specified recipe
                if (_targets[i].GetTargetId() == targetId)
                {
                    _targets.RemoveAt(i);
                    break;
                }
            }
        }

        /// <summary>
        /// Sets the progress of the specified target
        /// </summary>
        /// <param name="targetId">The ID of the target to update</param>
        /// <param name="progress">The value to set as the progress</param>
        public void SetTargetProgress(Guid targetId, float progress)
        {
            // find target with correct ID
            var matching = _targets.Where(t => t.GetTargetId() == targetId).FirstOrDefault();
            if (matching != null)
            {
                matching.UpdateProgress(progress);
            }
        }

        /// <summary>
        /// Returns a list of all targets
        /// </summary>
        /// <returns>The targets</returns>
        public List<Target> GetTargets()
        {
            return _targets;
        }

        /// <summary>
        /// Returns a list of all happy zones
        /// </summary>
        /// <returns>The happy zones</returns>
        public HappyZone[] GetHappyZones()
        {
            return _happyZones;
        }

        /// <summary>
        /// Updates the value of a happy zone
        /// </summary>
        /// <param name="type">The zone to update</param>
        /// <param name="value">The new value of the zone</param>
        /// <param name="updateTime">The time that this zone was updated</param>
        public void UpdateHappyZone(HappyZoneTypes type, float value, DateTime updateTime)
        {
            _happyZones[(int)type].UpdateValue(value, updateTime);
        }

        /// <summary>
        /// Gets the value of the specified happy zone
        /// </summary>
        /// <param name="type">The zone to get the value for</param>
        /// <param name="lastUpdate">[Output] When the zone was last updated</param>
        /// <returns>The current value of the happy zone</returns>
        public float GetZoneValue(HappyZoneTypes type, out DateTime lastUpdate)
        {
            lastUpdate = _happyZones[(int)type].GetLastUpdate();
            return _happyZones[(int)type].GetCompletion();
        }

        /// <summary>
        /// Adds a comment to the specified zone
        /// </summary>
        /// <param name="type">The zone to add the comment to</param>
        /// <param name="comment">The comment to add</param>
        public void AddComment(HappyZoneTypes type, string comment)
        {
            AddComment(type, comment, DateTime.Now);
        }

        /// <summary>
        /// Adds a comment to the specified zone
        /// </summary>
        /// <param name="type">The zone to add the comment to</param>
        /// <param name="comment">The comment to add</param>
        /// <param name="date">The date of the comment</param>
        public void AddComment(HappyZoneTypes type, string comment, DateTime date)
        {
            _happyZones[(int)type].AddComment(date, comment);
        }

        /// <summary>
        /// Removes the specified comment from the specified happy zone
        /// </summary>
        /// <param name="type">The zone to remove the comment from</param>
        /// <param name="commentId">The ID of the comment to remove</param>
        public void RemoveComment(HappyZoneTypes type, Guid commentId)
        {
            _happyZones[(int)type].RemoveComment(commentId);
        }
    }
}
