using System;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;

namespace Cookalong.Controls
{
    /// <summary>
    /// Interaction logic for Input_Numeric.xaml
    /// </summary>
    public partial class Input_Numeric : UserControl
    {
        float _maxValue = 99;
        bool _decimal = false;

        /// <summary>
        /// Constructor
        /// </summary>
        public Input_Numeric()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Sets the parameters for max length and value
        /// </summary>
        /// <param name="canBeDecimal">Whether the input can include decimal numbers</param>
        /// <param name="maxValue">The highest value that this input can accept</param>
        /// <param name="maxLength">The highest number of characters that this input should accept</param>
        public void Initialise(bool canBeDecimal, float maxValue, int maxLength = 2)
        {
            _maxValue = maxValue;
            _decimal = canBeDecimal;
            txtContent.MaxLength = maxLength;
        }

        /// <summary>
        /// Sets the content of this control
        /// </summary>
        /// <param name="v"></param>
        internal void SetInputValue(float v)
        {
            txtContent.Text = v.ToString();
        }

        /// <summary>
        /// Returns the value entered into this input
        /// </summary>
        /// <returns>The user-entered value</returns>
        public float GetValue()
        {
            return float.Parse(txtContent.Text);
        }

        /// <summary>
        /// Event handler for when the content of the textbox changes
        /// </summary>
        private void txtContent_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // check that the value is a number (decimals only allowed if specified)
            var regexString = _decimal ? "[^0-9.]" : "[^0-9]";
            Regex regex = new Regex(regexString);
            bool handled = (regex.IsMatch(e.Text));

            if (!handled)
            {
                // check that the new value is not more than the maximum value
                var preSelection = txtContent.Text.Substring(0, txtContent.SelectionStart); // before highlighted text
                var postSelection = txtContent.Text.Substring(txtContent.SelectionStart + txtContent.SelectionLength);  // after highlighted text

                string newValue = (preSelection + postSelection).Insert(txtContent.SelectionStart, e.Text); // put new value in the middle

                // if jsut a decimal point, the casting will fail
                if (newValue == ".")
                {
                    // set to "0."
                    handled = true;
                    txtContent.Text = "0.";
                    // move to the end of the text
                    txtContent.CaretIndex = 2;
                }
                else
                {
                    // check data value is less than maximum
                    handled = (float.Parse(newValue) > _maxValue);
                }

                // check decimal points
                if (!handled && (preSelection + postSelection).Contains('.') && e.Text.Contains('.'))
                {
                    // can't have multiple decimals
                    handled = true;
                }
            }

            // if Handled = true, the input is ignored
            e.Handled = handled;
        }

        /// <summary>
        /// Event handler for when the left button is clicked on the Decrease button
        /// </summary>
        private void lblDecrease_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // decrease value until we reach 0
            var currentValue = float.Parse(txtContent.Text);
            if (currentValue >= 1)
            {
                txtContent.Text = (currentValue - 1).ToString();
            }
            else if (currentValue > 0)
            {
                // could be a decimal, so go to 0 (rather than a negative decimal)
                txtContent.Text = "0";
            }
        }

        /// <summary>
        /// Event handler for when the left button is clicked on the Decrease button
        /// </summary>
        private void lblIncrease_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // increase value until we reach maximum
            var currentValue = float.Parse(txtContent.Text);
            if (currentValue <= (_maxValue - 1))
            {
                txtContent.Text = (currentValue + 1).ToString();
            }
            else if (currentValue < _maxValue)
            {
                // could be a decimal, so go to maximum value (rather than a decimal more than the maximum)
                txtContent.Text = _maxValue.ToString();
            }
        }

        /// <summary>
        /// Event handler for when the text box loses focus
        /// </summary>
        private void txtContent_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            if (txtContent.Text.Length < 1 || float.Parse(txtContent.Text) == 0)
                txtContent.Text = "1";
        }

        /// <summary>
        /// Event handler for when items are copied or pasted from the text box - block pasting
        /// </summary>
        private void txtContent_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.Paste)
            {
                e.Handled = true;
            }
        }

        private void UserControl_IsEnabledChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            txtContent.IsEnabled = IsEnabled;
            Visibility = IsEnabled ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
        }
    }
}
