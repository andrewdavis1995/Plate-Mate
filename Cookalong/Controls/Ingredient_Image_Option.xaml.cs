using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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
        Action _deselectCallback;
        bool _selected = false;

        public static string GetImagePath(int index)
        {
            return "pack://application:,,,/" + Assembly.GetExecutingAssembly().FullName + ";component/Images/FoodIcons/" + index + ".png";
        }

        public Ingredient_Image_Option(int index, Action callback)
        {
            InitializeComponent();
            Uri uri = new Uri(GetImagePath(index), UriKind.RelativeOrAbsolute);
            imgIcon.Source = BitmapFrame.Create(uri);
            _deselectCallback = callback;
        }

        internal void Select()
        {
            _deselectCallback();
            _selected = true;
            theBorder.BorderBrush = Application.Current.Resources["RecipeHoverEnter"] as SolidColorBrush;
        }

        internal void Deselect()
        {
            theBorder.BorderBrush = null;
            _selected = false;
        }

        private void UserControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Select();
        }

        public bool Selected()
        {
            return _selected;
        }
    }
}
