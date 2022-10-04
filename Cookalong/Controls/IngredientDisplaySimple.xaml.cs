using Andrew_2_0_Libraries.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Cookalong.Controls
{
    /// <summary>
    /// Interaction logic for IngredientDisplaySimple.xaml
    /// </summary>
    public partial class IngredientDisplaySimple : UserControl
    {
        public IngredientDisplaySimple(Ingredient ingredient)
        {
            InitializeComponent();
            SetContent(ingredient);
        }

        /// <summary>
        /// Sets the content to display
        /// </summary>
        /// <param name="ingredient">Ingredient to display</param>
        public void SetContent(Ingredient ingredient)
        {
            // display content
            txtContent.Text = ingredient.GetName();
            txtQuantity.Text = ingredient.GetValue() + " " + Enum.GetName(typeof(MeasurementUnit), ingredient.GetUnit())?.ToLower() + "(s)";
            ingredientIcon.Source = new BitmapImage(new Uri(Ingredient_Image_Option.GetImagePath(ingredient.GetImageIndex())));
        }

        /// <summary>
        /// Sets the control to selected colour - in progress
        /// </summary>
        internal void MarkAsActive()
        {
            brdBackground.Background = Application.Current.Resources["SelectedColour"] as SolidColorBrush;
        }

        /// <summary>
        /// Sets the control to green - complete
        /// </summary>
        internal void MarkAsDone()
        {
            brdBackground.Background = Application.Current.Resources["ConfirmedColour"] as SolidColorBrush;
        }
    }
}
