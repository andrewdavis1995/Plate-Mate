namespace Andrew_2_0_Libraries.FileHandling
{
    /// <summary>
    /// Handles file access for recipes
    /// </summary>
    class RecipeFileHandler : BaseFileHandler
    {
        /// <summary>
        /// Gets the path to the output file
        /// </summary>
        /// <returns></returns>
        internal override string GetFilePath()
        {
            return "Recipes";
        }
    }
}
