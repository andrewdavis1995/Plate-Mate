using Andrew_2_0_Libraries.Models;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Cookalong.Controls
{
    /// <summary>
    /// Interaction logic for RecipeDisplay.xaml
    /// </summary>
    public partial class RecipeDisplay : UserControl
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public RecipeDisplay()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="recipe">Recipe to show</param>
        public RecipeDisplay(Recipe recipe)
        {
            InitializeComponent();

            // display data
            txtName.Text = recipe.GetRecipeName();
            txtServings.Text = recipe.GetServingSize().ToString();

            // show image if it exists
            if(File.Exists(recipe.GetImagePath()))
            {
                imgRecipePic.Source = new BitmapImage(new Uri(recipe.GetImagePath(), UriKind.Absolute));
            }

            // show dietary info if necessary
            imgVegetarian.Visibility = recipe.IsVegetarian() ? Visibility.Visible : Visibility.Collapsed;
            imgVegan.Visibility = recipe.IsVegan() ? Visibility.Visible : Visibility.Collapsed;
            imgGluten.Visibility = recipe.IsGlutenFree() ? Visibility.Visible : Visibility.Collapsed;
            imgDairy.Visibility = recipe.IsDairyFree() ? Visibility.Visible : Visibility.Collapsed;

            // show time
            if(recipe.GetSetTime() > 0)
            {
                txtTime.Text = recipe.GetSetTime().ToString();
            }
            else
            {
                txtTime.Visibility = Visibility.Collapsed;
                imgTime.Visibility = Visibility.Collapsed;
                txtMinutes.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Event handler when the mouse is over the control
        /// </summary>
        private void UserControl_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            string resourceName = "RecipeHoverEnter";
            Background = Application.Current.Resources[resourceName] as SolidColorBrush;
        }

        /// <summary>
        /// Event handler when the mouse is no longer over the control
        /// </summary>
        private void UserControl_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            string resourceName = "RecipeHoverLeave";
            Background = Application.Current.Resources[resourceName] as SolidColorBrush;
        }
    }
}
