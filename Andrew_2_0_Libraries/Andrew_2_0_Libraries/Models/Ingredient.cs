using System;
using System.Diagnostics;

namespace Andrew_2_0_Libraries.Models
{
    public enum MeasurementUnit { Breast, Chunk, Florets, Gram, Leaves, Millilitre, Ounce, Serving, Slice, Sprinkle, Squirt, Tablespoon, Teaspoon, Unit };

    public class Ingredient : BaseSaveable
    {
        MeasurementUnit _unit;
        float _value;
        string _name;
        int _calories;
        int _imageIndex;

        #region Accessor functions
        public MeasurementUnit GetUnit() { return _unit; }
        public float GetValue() { return _value; }
        public string GetName() { return _name; }
        public int GetCalories() { return _calories; }
        public int GetImageIndex() { return _imageIndex; }
        #endregion

        public Ingredient() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ingredientName">The name of the ingredient</param>
        /// <param name="unit">The unit of measurement to be used</param>
        /// <param name="measurementValue">How much of the ingredient to use</param>
        /// <param name="calories">How many calories are in this ingredient</param>
        /// <param name="imageIndex">Index of which icon to use</param>
        public Ingredient(string ingredientName, MeasurementUnit unit, float measurementValue, int calories, int imageIndex)
        {
            _name = ingredientName;
            _unit = unit;
            _value = measurementValue;
            _calories = calories;
            _imageIndex = imageIndex;
        }

        public string GetDisplayOutput()
        {
            return GetValue() + " " + GetUnit() + "(s) of " + GetName();
        }

        /// <summary>
        /// Checks if this ingredients name is a match for the supplied filter
        /// </summary>
        /// <param name="filter">The filter to use</param>
        /// <returns>Whether the ingredient matches the filter</returns>
        internal bool MatchesFilter(string filter)
        {
            return _name.ToLower().Contains(filter.ToLower());
        }

        public override string GetTextOutput()
        {
            return GetName() + "~" + GetUnit() + "~" + GetValue() + "~" + GetCalories() + "~" + _imageIndex;
        }

        public override bool ParseData(string data)
        {
            var success = false;

            var ingSplit = data.Split(new string[] { "~" }, StringSplitOptions.RemoveEmptyEntries);

            // make sure there are enough entries
            if (ingSplit.Length < 5) return false;

            try
            {
                _name = ingSplit[0];
                _unit = (MeasurementUnit)Enum.Parse(typeof(MeasurementUnit), ingSplit[1]);
                _value = float.Parse(ingSplit[2]);
                _calories = int.Parse(ingSplit[3]);
                _imageIndex = int.Parse(ingSplit[4]);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
            return success;
        }
    }
}
