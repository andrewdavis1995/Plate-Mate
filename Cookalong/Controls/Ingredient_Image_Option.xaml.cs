using System;
using System.Collections.Generic;
using System.Linq;
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

        public Ingredient_Image_Option(string path, Action callback)
        {
            InitializeComponent();
            imgIcon.Source = new BitmapImage(new Uri((path)));
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
