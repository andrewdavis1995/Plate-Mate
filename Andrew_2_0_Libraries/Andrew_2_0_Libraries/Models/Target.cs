using System;

namespace Andrew_2_0_Libraries.Models
{
    public class Target : BaseSaveable
    {
        float _progress;
        string _name;
        Guid _targetId;

        #region Accessor functions
        public float GetProgress() { return _progress; }
        public string GetTargetName() { return _name; }
        public Guid GetTargetId() { return _targetId; }
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="targetName">The name of the target</param>
        public Target(string targetName)
        {
            _targetId = Guid.NewGuid();
            _progress = 0f;
            UpdateValues(targetName);
        }

        /// <summary>
        /// Update the values of the target
        /// </summary>
        /// <param name="targetName">The name of the target</param>
        public void UpdateValues (string targetName)
        {
            _name = targetName;
        }

        /// <summary>
        /// Update the progress of the target
        /// </summary>
        /// <param name="progress">Progress of the target</param>
        public void UpdateProgress(float progress)
        {
            _progress = progress;
        }

        /// <summary>
        /// Gets the output to be written to the save file
        /// </summary>
        /// <returns>File output</returns>
        public override string GetTextOutput()
        {
            // TODO
            return "";
        }

        /// <summary>
        /// Parses the data into a Target object
        /// </summary>
        /// <param name="data">The data to parse</param>
        public override bool ParseData(string data)
        {
            // TODO
            return false;
        }
    }
}
