using Andrew_2_0_Libraries.Models;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Cookalong.Controls.PopupWindows
{
    /// <summary>
    /// Interaction logic for Popup_Recipe.xaml
    /// </summary>
    public partial class Popup_Recipe : UserControl
    {
        Grid? _parent;
        Recipe? _recipe;

        public Popup_Recipe()
        {
            InitializeComponent();
        }

        public Popup_Recipe(Grid parent, Recipe recipe)
        {
            InitializeComponent();
            _parent = parent;
            _recipe = recipe;
            cmdClose.Configure("Back", true, "Cancel");
            cmdEdit.Configure("Edit");

            DisplayRecipe_();
        }

        private void DisplayRecipe_()
        {
            if (_recipe == null) return;

            txtRecipeName.Text = _recipe.GetRecipeName();

            stckIngredients.Children.Clear();
            foreach(var i in _recipe.GetIngredients())
            {
                var output = new IngredientsDisplay(i);
                stckIngredients.Children.Add(output);
            }

            stckMethod.Children.Clear();
            int index = 1;
            foreach(var s in _recipe.GetMethodSteps())
            {
                stckMethod.Children.Add(new MethodStepItem(index++, s));
            }

            if (File.Exists(_recipe.GetImagePath()))
            {
                imgRecipe.Source = new BitmapImage(new Uri(_recipe.GetImagePath(), UriKind.Absolute));
                imgBackground.Source = new BitmapImage(new Uri(_recipe.GetImagePath(), UriKind.Absolute));
                imgBackground.Visibility = Visibility.Visible;
            }
            else
            {
                imgBackground.Visibility = Visibility.Collapsed;
            }

            txtServings.Text = _recipe.GetServingSize().ToString();
            txtCalories.Text = _recipe.GetTotalCalories() > 0 ? _recipe.GetTotalCalories().ToString() : "";

            SetTime_(_recipe.GetSetTime());

            imgVegetarian.Visibility = _recipe.IsVegetarian() ? Visibility.Visible : Visibility.Collapsed;
            imgVegan.Visibility = _recipe.IsVegan() ? Visibility.Visible : Visibility.Collapsed;
            imgGluten.Visibility = _recipe.IsGlutenFree() ? Visibility.Visible : Visibility.Collapsed;
            imgDairy.Visibility = _recipe.IsDairyFree() ? Visibility.Visible : Visibility.Collapsed;

            iconCalories.Visibility = _recipe.GetTotalCalories() > -1 ? Visibility.Visible : Visibility.Collapsed;
            iconCalories.SetImage("Calories");
            iconServings.SetImage("Servings");
            iconTiming.SetImage("Timing");

            SetTime_(_recipe.GetSetTime());
        }

        private void SetTime_(uint v)
        {
            if(v == 0)
            {
                txtTiming.Text = "No time specified";
            }
            else
            {
                var hours = v / 60;
                var minutes = v - (hours * 60);

                string str = "";
                if (hours > 0) str += hours + " hours";
                if (minutes > 0)
                {
                    if (hours > 0) str += hours + ", ";
                    str += minutes + " minutes";
                }

                txtTiming.Text = str;
            }
        }

        private void cmdClose_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _parent?.Children.Remove(this);
        }

        private void cmdEdit_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var editWindow = new Popup_NewRecipe(_parent, _recipe, RecipeMenu.Instance?.Controller,
                (r) =>
                {
                    RecipeMenu.Instance?.UpdateDisplay();
                    _recipe = r;

                    if (r != null)
                        DisplayRecipe_();
                    else
                        _parent.Children.Remove(this);
                }
            );

            PopupController.AboveAll(editWindow);
            _parent.Children.Add(editWindow);
        }
    }
}
