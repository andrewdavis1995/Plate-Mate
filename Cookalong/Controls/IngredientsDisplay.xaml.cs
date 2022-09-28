using Andrew_2_0_Libraries.Models;
using System;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Cookalong.Controls
{
    /// <summary>
    /// Interaction logic for IngredientsDisplay.xaml
    /// </summary>
    public partial class IngredientsDisplay : UserControl
    {
        Ingredient _ingredient;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ingredient">Ingredient to display</param>
        public IngredientsDisplay(Ingredient ingredient)
        {
            InitializeComponent();
            _ingredient = ingredient;
            SetContent(ingredient);
        }

        /// <summary>
        /// Sets the content to display
        /// </summary>
        /// <param name="ingredient">Ingredient to display</param>
        public void SetContent(Ingredient ingredient)
        {
            _ingredient = ingredient;
            txtContent.Text = ingredient.GetName();
            txtQuantity.Text = ingredient.GetValue() + " " + Enum.GetName(typeof(MeasurementUnit), ingredient.GetUnit()).ToLower() + "(s)";
            ingredientIcon.Source = new BitmapImage(new Uri(Ingredient_Image_Option.GetImagePath(ingredient.GetImageIndex())));
        }

        /// <summary>
        /// Get the ingredient from this control
        /// </summary>
        /// <returns>The ingredient stored</returns>
        internal Ingredient GetIngredient()
        {
            return _ingredient;
        }
    }
}
