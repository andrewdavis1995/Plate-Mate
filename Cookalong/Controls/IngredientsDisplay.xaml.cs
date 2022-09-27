using Andrew_2_0_Libraries.Models;
using Cookalong.Controls.PopupWindows;
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

        public IngredientsDisplay(Ingredient ingredient)
        {
            InitializeComponent();
            _ingredient = ingredient;
            SetContent(ingredient);
 }

        public void SetContent(Ingredient ingredient)
        {
            _ingredient = ingredient;
            txtContent.Text = ingredient.GetName();
            txtQuantity.Text = ingredient.GetValue() + " " + Enum.GetName(typeof(MeasurementUnit), ingredient.GetUnit()).ToLower() + "(s)";
            ingredientIcon.Source = new BitmapImage(new Uri(Popup_Ingredient.GetImagePath(ingredient.GetImageIndex())));
        }

        internal Ingredient GetIngredient()
        {
            return _ingredient;
        }
    }
}
