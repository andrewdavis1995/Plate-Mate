using Andrew_2_0_Libraries.FileHandling;
using Andrew_2_0_Libraries.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Andrew_2_0_Libraries.Controllers
{
    public class RecipeController
    {
        List<Recipe> _recipes;
        RecipeFileHandler _fileHandler = new RecipeFileHandler();

        /// <summary>
        /// Constructor
        /// </summary>
        public RecipeController()
        {
            _recipes = new List<Recipe>();
            _recipes = _fileHandler.ReadFile<Recipe>();
        }

        /// <summary>
        /// Reloads the data
        /// </summary>
        public void Refresh()
        {
            _recipes = _fileHandler.ReadFile<Recipe>();
        }

        /// <summary>
        /// Adds a new recipe
        /// </summary>
        /// <param name="recipeName">The name of the recipe</param>
        /// <param name="ingredients">The ingredients in the recipe</param>
        /// <param name="steps">The steps involved in the recipe</param>
        /// <param name="vegetarian">Whether the recipe is vegetarian</param>
        /// <param name="servingSize">Number of portions</param>
        /// <param name="imagePath">Path to the image of the recipe</param>
        public Recipe AddRecipe(string recipeName, List<Ingredient> ingredients, List<string> steps, byte dietary, int servingSize, string imagePath, uint setTime)
        {
            var recipe = new Recipe(recipeName, ingredients, steps, dietary, servingSize, imagePath, setTime);
            _recipes.Add(recipe);
            Save_();
            return recipe;
        }

        /// <summary>
        /// Updates an existing recipe
        /// </summary>
        /// <param name="recipeId">The ID of the recipe to update</param>
        /// <param name="recipeName">The name of the recipe</param>
        /// <param name="ingredients">The ingredients in the recipe</param>
        /// <param name="steps">The steps involved in the recipe</param>
        /// <param name="vegetarian">Whether the recipe is vegetarian</param>
        /// <param name="servingSize">Number of portions</param>
        /// <param name="imagePath">Path to the image of the recipe</param>
        public Recipe UpdateRecipe(Guid recipeId, string recipeName, List<Ingredient> ingredients, List<string> steps, byte dietary, int servingSize, string imagePath, uint setTime)
        {
            // find recipe with correct ID
            var matching = _recipes.Where(r => r.GetRecipeId() == recipeId).FirstOrDefault();
            if (matching != null)
            {
                matching.UpdateRecipe(recipeName, ingredients, steps, dietary, servingSize, imagePath, setTime);
            }
            else
            {
                // if unable to find the matching recipe, add a new one
                matching = AddRecipe(recipeName, ingredients, steps, dietary, servingSize, imagePath, setTime);
            }

            Save_();

            return matching;
        }

        /// <summary>
        /// Deletes the specified recipe
        /// </summary>
        /// <param name="recipeId">The ID of the recipe to delete</param>
        public void DeleteRecipe(Guid recipeId)
        {
            for (int i = 0; i < _recipes.Count; i++)
            {
                // find and remove the specified recipe
                if (_recipes[i].GetRecipeId() == recipeId)
                {
                    _recipes.RemoveAt(i);
                    break;
                }
            }
            Save_();
        }

        /// <summary>
        /// Gets a list of all recipes
        /// </summary>
        /// <param name="filter">Filter to search on</param>
        /// <param name="veggieOnly">Whether to only include vegetarian options</param>
        /// <returns>List of all recipes</returns>
        public List<Recipe> GetRecipes(string filter, byte dietary) 
        {
            var matching = _recipes.Where(r => 
                (r.GetRecipeName().ToLower().Contains(filter.ToLower()))
                && ((r.GetDietaryValue() & dietary) == dietary)
            ).ToList();

            return matching;
        }

        /// <summary>
        /// Gets a list of all recipes which match the specified filter
        /// </summary>
        /// <param name="filter">The filter to use in the search</param>
        /// <returns>List of all recipes that match the filter</returns>
        public List<Recipe> GetRecipes(string filter)
        {
            return _recipes.Where(r => r.MatchesFilter(filter)).ToList();
        }

        /// <summary>
        /// Adds a comment to the specified recipe
        /// </summary>
        /// <param name="recipeId">The ID of the recipe to add a comment to</param>
        /// <param name="comment">The comment to add</param>
        public void AddComment(Guid recipeId, string comment)
        {
            AddComment(recipeId, comment, DateTime.Now);
            Save_();
        }

        /// <summary>
        /// Adds a comment to the specified recipe
        /// </summary>
        /// <param name="recipeId">The ID of the recipe to add a comment to</param>
        /// <param name="comment">The comment to add</param>
        /// <param name="date">The date of the comment</param>
        public void AddComment(Guid recipeId, string comment, DateTime date)
        {
            var recipe = _recipes.Where(r => r.GetRecipeId() == recipeId).FirstOrDefault();
            if (recipe != null)
                recipe.AddComment(new ItemComment(recipeId, date, comment));
            Save_();
        }

        /// <summary>
        /// Removes the specified comment from the specified recipe
        /// </summary>
        /// <param name="recipeId">The ID of the recipe to remove from</param>
        /// <param name="commentId">The ID of the comment to remove</param>
        public void RemoveComment(Guid recipeId, Guid commentId)
        {
            var recipe = _recipes.Where(r => r.GetRecipeId() == recipeId).FirstOrDefault();
            if (recipe != null)
                recipe.RemoveComment(commentId);
            Save_();
        }

        /// <summary>
        /// Saves the list of recipes
        /// </summary>
        private void Save_()
        {
            _fileHandler.WriteFile(_recipes);
        }
    }
}
