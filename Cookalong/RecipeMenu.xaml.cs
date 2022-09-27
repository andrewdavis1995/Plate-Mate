using Andrew_2_0_Libraries.Controllers;
using Andrew_2_0_Libraries.Models;
using Cookalong.Controls;
using Cookalong.Controls.PopupWindows;
using System;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Cookalong
{
    /// <summary>
    /// Interaction logic for RecipeMenu.xaml
    /// </summary>
    public partial class RecipeMenu : Window
    {
        float _columnCount = 1;

        public RecipeController Controller = new RecipeController();

        public static RecipeMenu Instance;

        Timer _filterTimer = new Timer();

        /// <summary>
        /// Constructor
        /// </summary>
        public RecipeMenu()
        {
            InitializeComponent();
            Instance = this;
            SetupButtons_();
        }

        private void SetupButtons_()
        {
            _filterTimer.Elapsed += _filterTimer_Elapsed;
            _filterTimer.Interval = 1000;

            chkVegetarian.UpdateDisplay(false);
            chkVegan.UpdateDisplay(false);
            chkDairy.UpdateDisplay(false);
            chkGluten.UpdateDisplay(false);

            chkVegetarian.AddTitle("Vegetarian Only", true);
            chkVegetarian.AddCallbacks(UpdateDisplay, UpdateDisplay);
            chkVegan.AddTitle("Vegan Only", true);
            chkVegan.AddCallbacks(UpdateDisplay, UpdateDisplay);
            chkDairy.AddTitle("Dairy-Free Only", true);
            chkDairy.AddCallbacks(UpdateDisplay, UpdateDisplay);
            chkGluten.AddTitle("Gluten-Free Only", true);
            chkGluten.AddCallbacks(UpdateDisplay, UpdateDisplay);

            cmdNewRecipe.Configure("New Recipe", false);
        }

        private void _filterTimer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            UpdateDisplay();
        }

        /// <summary>
        /// Callback when the page is fully loaded
        /// </summary>
        /// <param name="sender">The object that triggered this event</param>
        /// <param name="e">Event arguments</param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DisplayRecipes_("");
        }

        /// <summary>
        /// Add the necessary columns and rows
        /// </summary>
        /// <param name="recipeCount">How many recipes there are</param>
        private void SetupRecipeGrid_(int recipeCount)
        {
            ResetSetup_();

            // calculate what is needed
            var colWidth = grdRecipes.ColumnDefinitions[1].Width.Value;
            var colHeight = grdRecipes.RowDefinitions[1].Height.Value;
            _columnCount = (int)(grdRecipes.ActualWidth / colWidth);
            var numRows = (int)(recipeCount / _columnCount);
            numRows += (recipeCount % _columnCount > 0 ? 1 : 0);

            // add columns
            for (int i = 1; i < _columnCount; i++)
            {
                grdRecipes.ColumnDefinitions.Insert(1, new ColumnDefinition() { Width = new GridLength(colWidth) });
            }

            // add rows
            for (int i = 1; i < numRows; i++)
            {
                grdRecipes.RowDefinitions.Insert(1, new RowDefinition() { Height = new GridLength(colHeight) });
            }

            // central align if there are enough recipes to fill the screen
            if (numRows >= 2)
                grdRecipes.ColumnDefinitions.First().Width = new GridLength(1, GridUnitType.Star);
            else
                grdRecipes.ColumnDefinitions.First().Width = new GridLength(20, GridUnitType.Pixel);
        }

        /// <summary>
        /// Resets the columns and rows to the default, before adding more new items
        /// </summary>
        private void ResetSetup_()
        {
            // reset columns
            var index = 2;
            while (index < grdRecipes.ColumnDefinitions.Count - 1)
            {
                grdRecipes.ColumnDefinitions.RemoveAt(index);
            }

            // reset rows
            index = 2;
            while (index < grdRecipes.RowDefinitions.Count - 1)
            {
                grdRecipes.RowDefinitions.RemoveAt(index);
            }
        }

        /// <summary>
        /// Displays the supplied list of recipes
        /// </summary>
        /// <param name="recipes">The recipes to display</param>
        private void DisplayRecipes_(string filter)
        {
            grdRecipes.Children.Clear();

            byte dietary = 0;

            if (chkVegetarian.IsChecked()) dietary += Recipe.FLAG_VEGGIE;
            if (chkVegan.IsChecked()) dietary += Recipe.FLAG_VEGAN;
            if (chkDairy.IsChecked()) dietary += Recipe.FLAG_DAIRY;
            if (chkGluten.IsChecked()) dietary += Recipe.FLAG_GLUTEN;

            var matchingRecipes = Controller.GetRecipes(filter, dietary);

            // set up the grid
            SetupRecipeGrid_(matchingRecipes.Count);

            var col = 1;
            var row = 1;

            // iterate through each recipe
            foreach (var recipe in matchingRecipes)
            {
                // create display
                var display = new RecipeDisplay(recipe);
                AddRecipeDisplay(display, col, row, recipe);

                // move to next column
                col++;
                if (col > _columnCount)
                {
                    // next row
                    col = 1;
                    row++;
                }
            }

            // "No recipes" message
            lblNoRecipes.Visibility = matchingRecipes.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Adds a display to the grid
        /// </summary>
        /// <param name="display">The display to show</param>
        /// <param name="col"></param>
        /// <param name="row"></param>
        void AddRecipeDisplay(RecipeDisplay display, int col, int row, Recipe? recipe)
        {
            Grid.SetColumn(display, col);
            Grid.SetRow(display, row);
            if (recipe != null)
                display.MouseLeftButtonDown += (sender, e) => ShowRecipe_(recipe);

            grdRecipes.Children.Add(display);
        }

        /// <summary>
        /// Creates a popup for creating a new recipe
        /// </summary>
        void NewRecipe_()
        {
            var popup = new Popup_NewRecipe(popupHolder, null, Controller, (r) => DisplayRecipes_(txtFilter.Text));
            PopupController.AboveAll(popup);
            popupHolder.Children.Insert(0, popup);
        }

        /// <summary>
        /// Shows the selected recipe
        /// </summary>
        /// <param name="recipe">The recipe to show</param>
        private void ShowRecipe_(Recipe recipe)
        {
            var popup = new Popup_Recipe(popupHolder, recipe);
            PopupController.AboveAll(popup);
            popupHolder.Children.Insert(0, popup);
        }

        public void UpdateDisplay()
        {
            Dispatcher.Invoke(() => DisplayRecipes_(txtFilter.Text));
        }

        private void cmdExit_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Environment.Exit(0);
        }

        private void txtFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            _filterTimer.Stop();
            _filterTimer.Start();
        }

        private void cmdNewRecipe_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            NewRecipe_();
        }

        public void ShowError(string msg)
        {
            ErrorPopup.MoveOn(msg);
        }
    }
}
