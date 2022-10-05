using Andrew_2_0_Libraries.Models;
using Cookalong.Helpers;
using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Cookalong.Controls.PopupWindows
{
    /// <summary>
    /// Interaction logic for Popup_Ingredient.xaml
    /// </summary>
    public partial class Popup_Ingredient : UserControl
    {
        Action? _cancelCallback;
        Action<Ingredient> _confirmCallback;

        const int MIN_LENGTH = 3;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="existing">The existing ingredient to update</param>
        /// <param name="cancelCallback">Callback when cancel is clicked</param>
        /// <param name="confirmCallback">Callback when confirm is clicked</param>
        public Popup_Ingredient(Ingredient ? existing, Action cancelCallback, Action<Ingredient> confirmCallback)
        {
            InitializeComponent();

            // store callbacks
            _cancelCallback = cancelCallback;
            _confirmCallback = confirmCallback;

            txtName.Text = existing != null ? existing.GetName() : string.Empty;

            // configure button appearance
            ConfigureButtons_();

            // configure numeric inputs
            inpCalories.Initialise(false, 1000, 4);
            inpValue.Initialise(false, 2000, 4);
            chkCalories.UpdateDisplay(true);
            inpCalories.IsEnabled = true;

            LoadTypes_();
            LoadIcons_();

            // set existing data
            if (existing != null)
            {
                SetExistingData_(existing);
            }

            // configure checkbox
            chkCalories.AddTitle("Count calories?", true);
            chkCalories.AddCallbacks(UpdateCalorieInput_, UpdateCalorieInput_);
        }

        /// <summary>
        /// Sets the existing data of the ingredient
        /// </summary>
        /// <param name="existing">The data to set</param>
        private void SetExistingData_(Ingredient existing)
        {
            // calories
            if (existing.GetCalories() >= 0)
                inpCalories.SetInputValue(existing.GetCalories());
            chkCalories.UpdateDisplay(existing.GetCalories() != -1);
            inpCalories.IsEnabled = existing.GetCalories() != -1;
            inpCalories.Visibility = existing.GetCalories() != -1 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;

            // quantity
            inpValue.SetInputValue(existing.GetValue());

            // unit
            foreach (var item in cmbType.Items)
            {
                if (item != null)
                {
                    var tag = (MeasurementUnit)(item as ComboBoxItem).Tag;
                    if (tag == existing.GetUnit())
                        cmbType.SelectedItem = item;
                }
            }

            // icon
            (stckImages.Children[existing.GetImageIndex()] as Ingredient_Image_Option)?.Select();
        }

        /// <summary>
        /// Returns the image index that is selected
        /// </summary>
        /// <returns>Selecting index</returns>
        int GetSelectedImageIndex()
        {
            int index = 0;
            foreach (Ingredient_Image_Option iio in stckImages.Children)
            {
                // if it's selected, return the index
                if (iio.Selected())
                {
                    return index;
                }
                ++index;
            }

            return 0;
        }

        /// <summary>
        /// Loads the icons for the ingredient
        /// </summary>
        private void LoadIcons_()
        {
            var index = 0;
            var valid = true;
            do
            {
                Dispatcher.Invoke(() =>
                {
                    try
                    {
                        // create a display
                        var option = new Ingredient_Image_Option(index++, DeselectAllIcons_);
                        stckImages.Children.Add(option);
                    }
                    catch (Exception)
                    {
                        valid = false;
                    }
                });
            } while (valid);

            (stckImages.Children[0] as Ingredient_Image_Option)?.Select();
        }

        /// <summary>
        /// Sets all icons as not selected
        /// </summary>
        void DeselectAllIcons_()
        {
            foreach (Ingredient_Image_Option iio in stckImages.Children)
            {
                iio.Deselect();
            }
        }

        /// <summary>
        /// Enables/disables the calories input as appropriate
        /// </summary>
        private void UpdateCalorieInput_()
        {
            inpCalories.IsEnabled = chkCalories.IsChecked();
        }

        /// <summary>
        /// Configures the buttons
        /// </summary>
        private void ConfigureButtons_()
        {
            ControlHelper.SetButtonAppearance_(cmdCancel, "Cancel");
            ControlHelper.SetButtonAppearance_(cmdConfirm, "Confirm");
        }

        /// <summary>
        /// Event handler for cancel
        /// </summary>
        private void cmdCancel_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _cancelCallback?.Invoke();
        }

        /// <summary>
        /// Loads the types of measurements that can be used into the combobox
        /// </summary>
        private void LoadTypes_()
        {
            cmbType.Items.Clear();

            var types = Enum.GetValues(typeof(MeasurementUnit));
            foreach (var t in types)
            {
                // add an item for each type
                cmbType.Items.Add(new ComboBoxItem() { Content = Enum.GetName(typeof(MeasurementUnit), t), Tag = t });
            }
        }

        /// <summary>
        /// Event handler for when the confirm button is pressed
        /// </summary>
        private void cmdConfirm_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // check length of the name
            if (txtName.Text.Length < MIN_LENGTH)
            {
                RecipeMenu.Instance.ShowError($"Name is too short. Must be at least {MIN_LENGTH} characters");
            }
            else
            {
                // get measurement and calories
                var measurement = (MeasurementUnit)Enum.Parse(typeof(MeasurementUnit), (cmbType.SelectedItem as ComboBoxItem)?.Tag.ToString());
                int cals = chkCalories.IsChecked() ? (int)inpCalories.GetValue() : -1;

                // return the created ingredient
                var i = new Ingredient(txtName.Text, measurement, inpValue.GetValue(), cals, GetSelectedImageIndex());
                _confirmCallback?.Invoke(i);
            }
        }

        /// <summary>
        /// Event handler for when the mouse wheel is scrolled on the icon list
        /// </summary>
        private void UserControl_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            // move left or right, based on direction
            if (e.Delta > 0)
                scrllIcons.LineLeft();
            else
                scrllIcons.LineRight();

            // doesn't need to do anything else
            e.Handled = true;
        }

        private void txtName_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                // don't allow space if nothing else
                if (txtName.Text.Length == 0)
                    e.Handled = true;
                // don't allow double space
                else if (txtName.Text.Last() == ' ')
                    e.Handled = true;
            }
        }

        private void txtName_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // don't allow these characters
            char[] forbidden = { '@', '~', '\"', '#' };
            if (forbidden.Any(c => e.Text.Contains(c)))
                e.Handled = true;
        }
    }
}
