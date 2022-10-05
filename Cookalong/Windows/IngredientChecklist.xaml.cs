using Andrew_2_0_Libraries.Models;
using Cookalong.Controls;
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
using System.Windows.Shapes;

namespace Cookalong
{
    /// <summary>
    /// Interaction logic for IngredientChecklist.xaml
    /// </summary>
    public partial class IngredientChecklist : Window
    {
        float _columnCount = 1;
        List<Ingredient> _ingredients;
        List<IngredientDisplaySimple> _displays = new List<IngredientDisplaySimple>();
        int _selectIndex = 0;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ingredients">The ingredients to check</param>
        public IngredientChecklist(List<Ingredient> ingredients)
        {
            InitializeComponent();
            _ingredients = ingredients;
        }

        /// <summary>
        /// Resets the columns and rows to the default, before adding more new items
        /// </summary>
        private void ResetSetup_()
        {
            _selectIndex = 0;
            _displays.Clear();

            // reset columns
            var index = 2;
            while (index < grdIngredients.ColumnDefinitions.Count - 1)
            {
                grdIngredients.ColumnDefinitions.RemoveAt(index);
            }

            // reset rows
            index = 2;
            while (index < grdIngredients.RowDefinitions.Count - 1)
            {
                grdIngredients.RowDefinitions.RemoveAt(index);
            }
        }

        /// <summary>
        /// Adds a control for the specified control
        /// </summary>
        /// <param name="col">The column to add the control in</param>
        /// <param name="row">The row to add the control in</param>
        /// <param name="ingredient">The ingredient to represent</param>
        void AddIngredientDisplay_(int col, int row, Ingredient ingredient)
        {
            var lbl = new IngredientDisplaySimple(ingredient);
            Grid.SetColumn(lbl, col);
            Grid.SetRow(lbl, row);
            grdIngredients.Children.Add(lbl);
            _displays.Add(lbl);
        }

        /// <summary>
        /// Add the necessary columns and rows
        /// </summary>
        /// <param name="recipeCount">How many recipes there are</param>
        private void SetupRecipeGrid_(int recipeCount)
        {
            ResetSetup_();

            // calculate what is needed
            var colWidth = grdIngredients.ColumnDefinitions[1].Width.Value;
            var rowHeight = grdIngredients.RowDefinitions[1].Height.Value;
            _columnCount = (int)(grdIngredients.ActualWidth / colWidth);
            var numRows = (int)(recipeCount / _columnCount);
            numRows += (recipeCount % _columnCount > 0 ? 1 : 0);

            // add columns
            for (int i = 1; i < _columnCount; i++)
            {
                grdIngredients.ColumnDefinitions.Insert(1, new ColumnDefinition() { Width = new GridLength(colWidth) });
            }

            // add rows
            for (int i = 1; i < numRows; i++)
            {
                grdIngredients.RowDefinitions.Insert(1, new RowDefinition() { Height = new GridLength(rowHeight) });
            }

            // central align if there are enough recipes to fill the screen
            if (numRows >= 2)
                grdIngredients.ColumnDefinitions.First().Width = new GridLength(1, GridUnitType.Star);
            else
                grdIngredients.ColumnDefinitions.First().Width = new GridLength(20, GridUnitType.Pixel);
        }

        /// <summary>
        /// Displays the ingredients
        /// </summary>
        void DisplayData_()
        {
            SetupRecipeGrid_(_ingredients.Count);

            var col = 1;
            var row = 1;

            // iterate through each recipe
            foreach (var i in _ingredients)
            {
                // create display
                AddIngredientDisplay_(col, row, i);

                // move to next column
                col++;
                if (col > _columnCount)
                {
                    // next row
                    col = 1;
                    row++;
                }
            }

            _displays[_selectIndex].MarkAsActive();
        }

        /// <summary>
        /// Event handler for when the window loads
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DisplayData_();
        }

        /// <summary>
        /// Event handler for when a key is pressed
        /// </summary>
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            // if space is pressed, complete the selected item
            if(e.Key == Key.Space)
            {
                _displays[_selectIndex++].MarkAsDone();
            }

            // if there are no more ingredients, begin the recipe walkthrough
            if(_selectIndex >= _displays.Count)
            {
                DialogResult = true;
                Close();
            }
            else
            {
                // otherwise, move on to the next one
                _displays[_selectIndex].MarkAsActive();
            }
        }
    }
}
