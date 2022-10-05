using Andrew_2_0_Libraries.Models;
using Cookalong.Controls.PopupWindows;
using Cookalong.Helpers;
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
        StackPanel _stackPanel;
        Popup_Confirmation ? _confirmationPopup;
        Grid _parentGrid;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ingredient">Ingredient to display</param>
        /// <param name="stackPanel">The panel that owns it</param>
        public IngredientsDisplay(Ingredient ingredient, StackPanel stackPanel, Grid parentGrid)
        {
            InitializeComponent();

            _ingredient = ingredient;
            _stackPanel = stackPanel;
            _parentGrid = parentGrid;

            SetContent(ingredient);
        }

        /// <summary>
        /// Sets the content to display
        /// </summary>
        /// <param name="ingredient">Ingredient to display</param>
        public void SetContent(Ingredient ingredient)
        {
            _ingredient = ingredient;

            // display content
            txtContent.Text = ingredient.GetName();
            txtQuantity.Text = ingredient.GetValue() + " " + Enum.GetName(typeof(MeasurementUnit), ingredient.GetUnit())?.ToLower() + "(s)";
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

        /// <summary>
        /// Removes the delete button
        /// </summary>
        public void DisableDelete()
        {
            cmdDelete.Visibility = System.Windows.Visibility.Collapsed;
        }

        /// <summary>
        /// Event handler for the delete button
        /// </summary>
        private void cmdDelete_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _confirmationPopup = new Popup_Confirmation("Confirm delete", "Are you sure you want to delete this ingredient?", () =>
            {
                // cancel callback
                _parentGrid?.Children.Remove(_confirmationPopup);
            },
            () =>
            {
                // confirm callback
                _parentGrid?.Children.Remove(_confirmationPopup);
                _stackPanel.Children.Remove(this);
            });

            // show popup
            PopupController.AboveAll(_confirmationPopup);
            _parentGrid?.Children.Add(_confirmationPopup);
        }
    }
}
