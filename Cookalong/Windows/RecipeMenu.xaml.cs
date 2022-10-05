using Andrew_2_0_Libraries.Controllers;
using Andrew_2_0_Libraries.Models;
using Cookalong.Controls;
using Cookalong.Controls.PopupWindows;
using Cookalong.Helpers;
using System;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;

namespace Cookalong
{
    /// <summary>
    /// Interaction logic for RecipeMenu.xaml
    /// </summary>
    public partial class RecipeMenu : Window
    {
        public static RecipeMenu Instance;
        public RecipeController Controller = new RecipeController();
        Popup_Confirmation? _confirmationPopup;

        float _columnCount = 1;
        Timer _filterTimer = new Timer();

        /// <summary>
        /// Constructor
        /// </summary>
        public RecipeMenu()
        {
            InitializeComponent();
            Instance = this;
            SetupButtons_();
            LoadBackups_();
        }

        /// <summary>
        /// Sets up the buttons with the correct data
        /// </summary>
        private void SetupButtons_()
        {
            // configure data
            _filterTimer.Elapsed += _filterTimer_Elapsed;
            _filterTimer.Interval = 1000;

            // configure filter checkboxes
            SetupFilterCheckbox_(chkGluten, "Gluten-Free Only");
            SetupFilterCheckbox_(chkVegetarian, "Vegetarian Only");
            SetupFilterCheckbox_(chkVegan, "Vegan Only");
            SetupFilterCheckbox_(chkDairy, "Dairy-Free Only");

            // configure buttons
            cmdNewRecipe.Configure("New Recipe", false);
        }

        /// <summary>
        /// Sets up a checkbox with correct data and callbacks
        /// </summary>
        /// <param name="chk">Checkbox to set</param>
        /// <param name="msg">Message to display</param>
        void SetupFilterCheckbox_(Input_Checkbox chk, string msg)
        {
            chk.UpdateDisplay(false);
            chk.AddTitle(msg, true);
            chk.AddCallbacks(UpdateDisplay, UpdateDisplay);
        }

        /// <summary>
        /// Called once the user stops typing for X seconds
        /// </summary>
        private void _filterTimer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            UpdateDisplay();
        }

        /// <summary>
        /// Callback when the page is fully loaded
        /// </summary>
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

            // work out which filter checkboxes are set
            if (chkVegetarian.IsChecked()) dietary += Recipe.FLAG_VEGGIE;
            if (chkVegan.IsChecked()) dietary += Recipe.FLAG_VEGAN;
            if (chkDairy.IsChecked()) dietary += Recipe.FLAG_DAIRY;
            if (chkGluten.IsChecked()) dietary += Recipe.FLAG_GLUTEN;

            // find recipes that match the filters
            var matchingRecipes = Controller.GetRecipes(filter, dietary);

            // set up the grid
            SetupRecipeGrid_(matchingRecipes.Count);

            var col = 1;
            var row = 1;

            // iterate through each recipe
            foreach (var recipe in matchingRecipes)
            {
                // create display
                AddRecipeDisplay(col, row, recipe);

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
        /// <param name="col">Column to add it in</param>
        /// <param name="row">Row to add it in</param>
        /// <param name="recipe">Recipe this represents</param>
        void AddRecipeDisplay(int col, int row, Recipe recipe)
        {
            var display = new RecipeDisplay(recipe);
            Grid.SetColumn(display, col);
            Grid.SetRow(display, row);
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

        /// <summary>
        /// Updates the recipe list with latest data
        /// </summary>
        public void UpdateDisplay()
        {
            Dispatcher.Invoke(() => DisplayRecipes_(txtFilter.Text));
        }

        /// <summary>
        /// Event hsndler for the exit button
        /// </summary>
        private void cmdExit_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // take a backup of the data
            BackupHandler.SaveBackup();

            // exit
            Environment.Exit(0);
        }

        /// <summary>
        /// Event handler for when the content of the filter search bar changes
        /// </summary>
        private void txtFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            // restart timer - waiting for the user to stop typing
            _filterTimer.Stop();
            _filterTimer.Start();
        }

        /// <summary>
        /// Event handler for the "new recipe" button
        /// </summary>
        private void cmdNewRecipe_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            NewRecipe_();
        }

        /// <summary>
        /// Displays an error message popup
        /// </summary>
        /// <param name="msg">The message to display</param>
        public void ShowError(string msg)
        {
            ErrorPopup.MoveOn(msg);
        }

        /// <summary>
        /// Load the backup data
        /// </summary>
        void LoadBackups_()
        {
            var backups = BackupHandler.GetBackups();

            foreach(var b in backups)
            {
                // create control
                var bo = new BackupOption(b);

                // assign callback
                bo.cmdRestore.MouseLeftButtonDown += (o, s) => 
                {
                    _confirmationPopup = new Popup_Confirmation("Confirm restore", "Are you sure you wish to restore this data? Current data will be overwritten.", () =>
                    {
                        // cancel callback
                        popupHolder?.Children.Remove(_confirmationPopup);
                    },
                    () =>
                    {
                        // confirm callback
                        popupHolder?.Children.Remove(_confirmationPopup);

                        // restore data
                        var success = BackupHandler.RestoreBackup_(b);

                        // show error if failed
                        if (success)
                        {
                            // hide popup
                            grdBackups.Visibility = Visibility.Collapsed;

                            // refresh data, and display
                            Controller.Refresh();
                            DisplayRecipes_(txtFilter.Text);
                        }
                        else
                        {
                            var err = new ErrorMessage();
                            popupHolder.Children.Add(err);
                            err.MoveOn("Could not restore data");
                        }
                    });

                    // ensure popup is always on top
                    PopupController.AboveAll(_confirmationPopup);
                    popupHolder.Children.Add(_confirmationPopup);
                };

                // add to display
                stckBackups.Children.Add(bo);
            }
        }

        /// <summary>
        /// Event handler for the recipe button
        /// </summary>
        private void RecipeButton_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            grdBackups.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Event handler for the backups button
        /// </summary>
        private void BackupsButton_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            grdBackups.Visibility = Visibility.Visible;
        }
    }
}
