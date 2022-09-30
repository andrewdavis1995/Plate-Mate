using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Cookalong.Controls
{
    /// <summary>
    /// Interaction logic for Ingredient_Image_Option.xaml
    /// </summary>
    public partial class Ingredient_Image_Option : UserControl
    {
        Action _deselectAllCallback;
        bool _selected = false;

        /// <summary>
        /// Get the path to use for food icons
        /// </summary>
        /// <param name="index">Index of images to get</param>
        /// <returns>The full path to the resource image</returns>
        public static string GetImagePath(int index)
        {
            return "pack://application:,,,/" + Assembly.GetExecutingAssembly().FullName + ";component/Images/FoodIcons/" + index + ".png";
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="index">Index of the icon</param>
        /// <param name="callback">The action to call when selected</param>
        public Ingredient_Image_Option(int index, Action callback)
        {
            InitializeComponent();

            // set image
            Uri uri = new Uri(GetImagePath(index), UriKind.RelativeOrAbsolute);
            imgIcon.Source = BitmapFrame.Create(uri);

            _deselectAllCallback = callback;
        }

        /// <summary>
        /// Mark the icon as selected
        /// </summary>
        internal void Select()
        {
            // deselect all other icons
            _deselectAllCallback();

            // mark this as selected
            _selected = true;
            theBorder.BorderBrush = Application.Current.Resources["RecipeHoverEnter"] as SolidColorBrush;
        }

        /// <summary>
        /// No longer selected
        /// </summary>
        internal void Deselect()
        {
            theBorder.BorderBrush = null;
            _selected = false;
        }

        /// <summary>
        /// Event handler for when this control is clicked
        /// </summary>
        private void UserControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // select icon
            Select();
        }

        /// <summary>
        /// Accessor for whether the icon is selected
        /// </summary>
        /// <returns>Whether the icon is selected</returns>
        public bool Selected()
        {
            return _selected;
        }
    }
}
