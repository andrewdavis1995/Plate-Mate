using Andrew_2_0_Libraries.Models;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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

        public Popup_Ingredient(Ingredient existing, Action cancelCallback, Action<Ingredient> confirmCallback)
        {
            InitializeComponent();
            _cancelCallback = cancelCallback;
            _confirmCallback = confirmCallback;

            txtName.Text = existing != null ? existing.GetName() : string.Empty;

            // configure button appearance
            ConfigureButtons_();

            inpCalories.Initialise(false, 1000, 4);
            inpValue.Initialise(false, 2000, 4);

            LoadTypes_();
            LoadIcons_();

            if (existing?.GetCalories() > 0)
                inpCalories.SetInputValue(existing.GetCalories());

            if (existing != null)
            {
                inpValue.SetInputValue(existing.GetValue());

                foreach (var item in cmbType.Items)
                {
                    if (item != null)
                    {
                        var tag = (MeasurementUnit)(item as ComboBoxItem).Tag;
                        if (tag == existing.GetUnit())
                            cmbType.SelectedItem = item;
                    }
                }

                (stckImages.Children[(int)existing.GetImageIndex()] as Ingredient_Image_Option)?.Select();
            }

            chkCalories.AddTitle("Count calories?", true);
            chkCalories.AddCallbacks(UpdateCalorieInput_, UpdateCalorieInput_);
            chkCalories.UpdateDisplay(existing?.GetCalories() != -1);
            inpCalories.IsEnabled = existing?.GetCalories() != -1;
        }

        int GetSelectedImageIndex()
        {
            int index = 0;
            foreach (Ingredient_Image_Option iio in stckImages.Children)
            {
                if(iio.Selected())
                {
                    return index;
                }
                ++index;
            }

            return 0;
        }

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

        void DeselectAllIcons_()
        {
            foreach(Ingredient_Image_Option iio in stckImages.Children)
            {
                iio.Deselect();
            }
        }

        private void UpdateCalorieInput_()
        {
            inpCalories.IsEnabled = chkCalories.IsChecked();
        }

        /// <summary>
        /// Configures the buttons
        /// </summary>
        private void ConfigureButtons_()
        {
            SetButtonAppearance_(cmdCancel, "Cancel");
            SetButtonAppearance_(cmdConfirm, "Confirm");
        }

        /// TODO: Refactor this to a new file (used multiple places)
        /// <summary>
        /// Sets the appearance of a button
        /// </summary>
        /// <param name="button">The button to update</param>
        /// <param name="msg">Message to display</param>
        void SetButtonAppearance_(Input_Button button, string msg)
        {
            button.lblMessage.Content = msg;

            try
            {
                var res = FindResource("Button" + msg);
                button.bigBorder.BorderBrush = res as SolidColorBrush;
            }
            catch (Exception)
            {
                var res = FindResource("ButtonDefault");
                button.bigBorder.BorderBrush = res as SolidColorBrush;
            }
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

        private void cmdConfirm_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (txtName.Text.Length < MIN_LENGTH)
            {
                RecipeMenu.Instance.ShowError($"Name is too short. Must be at least {MIN_LENGTH} characters");
            }
            else
            {
                var measurement = (MeasurementUnit)Enum.Parse(typeof(MeasurementUnit), (cmbType.SelectedItem as ComboBoxItem)?.Tag.ToString());
                int cals = chkCalories.IsChecked() ? (int)inpCalories.GetValue() : -1;

                var i = new Ingredient(txtName.Text, measurement, inpValue.GetValue(), cals, GetSelectedImageIndex());
                _confirmCallback?.Invoke(i);
            }
        }

        private void UserControl_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                scrllIcons.LineLeft();
            else
                scrllIcons.LineRight();
            e.Handled = true;
        }
    }
}
