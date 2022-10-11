using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Andrew_2_0_Libraries.Models
{
    public class Recipe : BaseSaveable
    {
        public static readonly byte FLAG_VEGGIE = 1;
        public static readonly byte FLAG_VEGAN  = 2;
        public static readonly byte FLAG_DAIRY  = 4;
        public static readonly byte FLAG_GLUTEN = 8;

        string _recipeName;
        Guid _recipeId;
        List<Ingredient> _ingredients;
        List<MethodStep> _methodSteps;
        List<ItemComment> _comments;
        byte _dietary;
        int _servingSize;
        string _imagePath;
        uint _setTime;

        #region Accessor functions
        public string GetRecipeName() { return _recipeName; }
        public Guid GetRecipeId() { return _recipeId; }
        public List<Ingredient> GetIngredients() { return _ingredients; }
        public List<MethodStep> GetMethodSteps() { return _methodSteps; }
        public List<ItemComment> GetComments() { return _comments; }
        public bool IsVegetarian() { return (_dietary & FLAG_VEGGIE) == FLAG_VEGGIE; }
        public bool IsVegan() { return (_dietary & FLAG_VEGAN) == FLAG_VEGAN; }
        public bool IsGlutenFree() { return (_dietary & FLAG_GLUTEN) == FLAG_GLUTEN; }
        public bool IsDairyFree() { return (_dietary & FLAG_DAIRY) == FLAG_DAIRY; }
        public int GetServingSize() { return _servingSize; }
        public string GetImagePath() { return _imagePath; }
        public uint GetSetTime() { return _setTime; }
        #endregion

        public Recipe() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="recipeName">The name of the recipe</param>
        /// <param name="ingredients">List of ingredients to add</param>
        /// <param name="steps">List of steps involved in the recipe</param>
        /// <param name="dietary">Whether the recipe is vegetarian, gluten free etc</param>
        /// <param name="servingSize">Number of portions</param>
        /// <param name="imagePath">Path to the image of the recipe</param>
        public Recipe(string recipeName, List<Ingredient> ingredients, List<MethodStep> steps, byte dietary, int servingSize, string imagePath, uint setTime)
        {
            _comments = new List<ItemComment>();
            _recipeId = Guid.NewGuid();
            _ingredients = new List<Ingredient>();
            _methodSteps = new List<MethodStep>();
            UpdateRecipe(recipeName, ingredients, steps, dietary, servingSize, imagePath, setTime);
        }

        /// <summary>
        /// Updates this recipe with new information
        /// </summary>
        /// <param name="recipeName">The name of the recipe</param>
        /// <param name="ingredients">List of ingredients to add</param>
        /// <param name="steps">List of steps involved in the recipe</param>
        /// <param name="vegetarian">Whether the recipe is vegetarian</param>
        /// <param name="servingSize">Number of portions</param>
        /// <param name="imagePath">Path to the image of the recipe</param>
        public void UpdateRecipe(string recipeName, List<Ingredient> ingredients, List<MethodStep> steps, byte dietary, int servingSize, string imagePath, uint setTime)
        {
            _recipeName = recipeName;
            _ingredients = ingredients;
            _methodSteps = steps;
            _dietary = dietary;
            _servingSize = servingSize;
            _imagePath = imagePath;
            _setTime = setTime;
        }

        /// <summary>
        /// Adds a comment to the recipe
        /// </summary>
        /// <param name="comment">The comment to add to the recipe</param>
        public void AddComment(ItemComment comment)
        {
            _comments.Add(comment);
        }

        /// <summary>
        /// Whether this recipe matches the specified filter
        /// </summary>
        /// <param name="filter">The filter to use</param>
        /// <returns>Whether the recipe matches the filter</returns>
        public bool MatchesFilter(string filter)
        {
            // recipe matches the filter if the name of it, or any of its ingredients, matches the search string
            return _recipeName.ToLower().Contains(filter.ToLower())
            || _ingredients.Any(i => i.MatchesFilter(filter));
        }

        /// <summary>
        /// Removes the specified comment from the recipe
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
            var str = _recipeName + "@" + _recipeId.ToString() + "@" + _dietary.ToString() + "@" + _servingSize + "@" + _imagePath + "@" + _setTime + "@" + GetIngredientString_() + "@" + GetMethodString_();
            return str;
        }

        /// <summary>
        /// Get the string for the ingredient list
        /// </summary>
        /// <returns>The formatted string</returns>
        private string GetIngredientString_()
        {
            StringBuilder str = new StringBuilder();

            bool first = true;
            foreach (var ing in _ingredients)
            {
                // build the string
                if (first)
                {
                    first = false;
                }
                else
                {
                    str.Append("#");
                }
                // add the ingredient
                str.Append(ing.GetTextOutput());
            }
            return str.ToString();
        }

        /// <summary>
        /// Get the string for the method list
        /// </summary>
        /// <returns>The formatted string</returns>
        private string GetMethodString_()
        {
            StringBuilder str = new StringBuilder();
            bool first = true;
            foreach (var step in _methodSteps)
            {
                // build the string
                if (first)
                {
                    first = false;
                }
                else
                {
                    str.Append("#");
                }

                // add the method
                str.Append(step.GetTextOutput());
            }
            return str.ToString();
        }

        /// <summary>
        /// Parses the data into a Recipe object
        /// </summary>
        /// <param name="data">The data to parse</param>
        public override bool ParseData(string data)
        {
            _ingredients = new List<Ingredient>();
            _methodSteps = new List<MethodStep>();

            var split = data.Split('@');

            // not enough data
            if (split.Length < 8)
            {
                Debug.WriteLine("Invalid data");
                return false;
            }

            try
            {
                // read data
                _recipeName = split[0];
                _recipeId = Guid.Parse(split[1]);
                _dietary = byte.Parse(split[2]);
                _servingSize = int.Parse(split[3]);
                _imagePath = split[4];
                _setTime = uint.Parse(split[5]);

                var success = ParseIngredients_(split[6]);
                if (!success) return false;

                ParseMethod_(split[7]);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Parses the list of ingredients from the string
        /// </summary>
        /// <param name="ingredientString">The string to parse</param>
        /// <returns>Whether the parsing was successful</returns>
        private bool ParseIngredients_(string ingredientString)
        {
            var split = ingredientString.Split(new string[] { "#" }, StringSplitOptions.RemoveEmptyEntries);

            // loop through each element
            foreach (var ing in split)
            {
                var newIng = new Ingredient();
                newIng.ParseData(ing);
                _ingredients.Add(newIng);
            }

            return true;
        }

        /// <summary>
        /// Parses the list of methods
        /// </summary>
        /// <param name="methodString">The string to parse</param>
        private void ParseMethod_(string methodString)
        {
            var split = methodString.Split(new string[] { "#" }, StringSplitOptions.RemoveEmptyEntries);
            _methodSteps = new List<MethodStep>();

            // add each element
            foreach (var step in split)
            {
                var method = new MethodStep();
                method.ParseData(step);

                _methodSteps.Add(method);
            }
        }

        /// <summary>
        /// Gets the total calories from all of the ingredients
        /// </summary>
        /// <returns>The total number of calories</returns>
        public int GetTotalCalories()
        {
            var total = 0;

            foreach (var i in _ingredients)
            {
                total += i.GetCalories();
            }

            return total;
        }

        /// <summary>
        /// Gets the dietary value of the flag
        /// </summary>
        /// <returns>The value</returns>
        internal byte GetDietaryValue()
        {
            return _dietary;
        }

        /// <summary>
        /// Sets the steps to the specified list
        /// </summary>
        /// <param name="steps">Steps to sit</param>
        public void SetSteps(List<MethodStep> steps)
        {
            _methodSteps=steps;
        }
    }
}
