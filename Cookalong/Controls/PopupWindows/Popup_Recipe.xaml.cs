using Andrew_2_0_Libraries.Controllers;
using Andrew_2_0_Libraries.Models;
using Cookalong.Helpers;
using Cookalong.Windows;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        readonly Action<string>? _errorCallback = null;
        RecipeController ? _controller = null;
        readonly Action? _updateDisplay;

        /// <summary>
        /// Constructor
        /// </summary>
        public Popup_Recipe()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parent">Grid which displays this control</param>
        /// <param name="recipe">Recipe to display</param>
        public Popup_Recipe(Grid parent, Recipe recipe, Action<string>? errorCallback, RecipeController controller, Action? action)
        {
            InitializeComponent();
            _parent = parent;
            _recipe = recipe;
            _errorCallback = errorCallback;
            _controller = controller;
            _updateDisplay = action;

            // configure buttons
            cmdClose.Configure("Back", true, "Cancel");
            cmdEdit.Configure("Edit");
            cmdWalkthrough.Configure("Begin");
            cmdConfigureTime.Configure("Configure Timing");

            DisplayRecipe_();
        }

        /// <summary>
        /// Displays the recipe
        /// </summary>
        private void DisplayRecipe_()
        {
            if (_recipe == null) return;

            // reset UI
            stckIngredients.Children.Clear();
            stckMethod.Children.Clear();

            // set icon images
            iconCalories.SetImage("Calories");
            iconServings.SetImage("Servings");
            iconTiming.SetImage("Timing");

            // display name
            txtRecipeName.Text = _recipe.GetRecipeName();

            // display ingredients
            foreach(var i in _recipe.GetIngredients())
            {
                var output = new IngredientsDisplay(i, stckIngredients, grdOverall);
                
                // can't delete on this page
                output.DisableDelete();
                stckIngredients.Children.Add(output);
            }

            int index = 1;
            // display method
            foreach(var s in _recipe.GetMethodSteps())
            {
                stckMethod.Children.Add(new MethodStepItem(index++, s.GetMethod()));
            }

            // check the image exists
            if (File.Exists(_recipe.GetImagePath()))
            {
                // display the recipe image
                imgRecipe.Source = new BitmapImage(new Uri(_recipe.GetImagePath(), UriKind.Absolute));
                imgBackground.Source = new BitmapImage(new Uri(_recipe.GetImagePath(), UriKind.Absolute));
                imgBackground.Visibility = Visibility.Visible;
            }
            else
            {
                // no image to display
                imgBackground.Visibility = Visibility.Collapsed;
            }

            // display data
            txtServings.Text = _recipe.GetServingSize().ToString();
            txtCalories.Text = _recipe.GetTotalCalories() > 0 ? _recipe.GetTotalCalories().ToString() : "";

            // show dietary icons
            imgVegetarian.Visibility = _recipe.IsVegetarian() ? Visibility.Visible : Visibility.Collapsed;
            imgVegan.Visibility = _recipe.IsVegan() ? Visibility.Visible : Visibility.Collapsed;
            imgGluten.Visibility = _recipe.IsGlutenFree() ? Visibility.Visible : Visibility.Collapsed;
            imgDairy.Visibility = _recipe.IsDairyFree() ? Visibility.Visible : Visibility.Collapsed;

            // show calories
            iconCalories.Visibility = _recipe.GetTotalCalories() > -1 ? Visibility.Visible : Visibility.Collapsed;

            // show time/duration
            txtTiming.Text = StringHelper.GetTimeString(_recipe.GetSetTime());

            // show configuration message
            grdConfigured.Visibility = _recipe.GetMethodSteps().Any(s => s.GetDuration() > 0)
                ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Event handler for the close button
        /// </summary>
        private void cmdClose_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _parent?.Children.Remove(this);
        }

        /// <summary>
        /// Event handler for the Edit button
        /// </summary>
        private void cmdEdit_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // show popup
            var editWindow = new Popup_NewRecipe(_parent, _recipe, _controller,
                (r) =>
                {
                    // display recipe callback
                    _updateDisplay?.Invoke();
                    _recipe = r;

                    if (r != null)
                        DisplayRecipe_();
                    else
                        _parent?.Children.Remove(this);
                }, _errorCallback
            );

            // ensure it is top most control
            PopupController.AboveAll(editWindow);
            _parent?.Children.Add(editWindow);
        }

        /// <summary>
        /// Event handler for the walkthrough button
        /// </summary>
        private void cmdWalkthrough_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // ensure recipe is valid
            if (_recipe == null)
                return;

            // check all ingredients are ok
            var ic = new IngredientChecklist(_recipe.GetIngredients());
            bool? state = ic.ShowDialog();

            // check the pre-check completed successfully
            if (state == true)
            {
                // show the walkthrough dialog
                var wt = new Walkthrough(_recipe.GetRecipeName(), _recipe.GetMethodSteps(), PlaybackMode.ClickThrough);
                wt.ShowDialog();
            }
        }

        /// <summary>
        /// Event handler for the configure time button
        /// </summary>
        private void cmdConfigureTime_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_recipe == null || _parent == null)
            {
                _errorCallback?.Invoke("Something went wrong.");
                return;
            }

            var tc = new TimeConfiguration(_recipe.GetMethodSteps());
            var result = tc.ShowDialog();

            // if confirmed, update the steps
            if (result == true)
            {
                var steps = tc.GetInstructions();
                _controller?.UpdateSteps(_recipe.GetRecipeId(), steps);
                grdConfigured.Visibility = Visibility.Visible;
            }
        }
    }
}
