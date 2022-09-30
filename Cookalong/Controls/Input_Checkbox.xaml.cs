using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Cookalong.Controls
{
    /// <summary>
    /// Interaction logic for Input_Checkbox.xaml
    /// </summary>
    public partial class Input_Checkbox : UserControl
    {
        private bool _switchState = true;
        Action ? CheckedCallback;
        Action ? UncheckedCallback;

        /// <summary>
        /// Constructor
        /// </summary>
        public Input_Checkbox()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Event handler for when the switch is toggled
        /// </summary>
        private void imgSwitch_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // toggle state and update UI
            _switchState = !_switchState;
            UpdateDisplay(_switchState);
        }

        /// <summary>
        /// Sets the correct appearance of the button
        /// </summary>
        /// <param name="state">The state to set</param>
        public void UpdateDisplay(bool state)
        {
            _switchState = state;

            // if now ON, call the checked callback
            if (_switchState)
            {
                CheckedCallback?.Invoke();
            }
            else
            {
                // otherwise, call the unchecked one
                UncheckedCallback?.Invoke();
            }

            // update UI
            imgTick.Visibility = _switchState ? Visibility.Visible : Visibility.Hidden;
            cmdCheck.Background = _switchState ? cmdCheck.BorderBrush : new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
        }

        /// <summary>
        /// Sets the text with the checkbox
        /// </summary>
        /// <param name="content">The content to display</param>
        /// <param name="changeColour">Whether to change the colour of the text (use TRUE when it's on a light background)</param>
        internal void AddTitle(string content, bool changeColour = false)
        {
            // set text
            txtCaption.Text = content;

            // set colour if appropriate
            if(changeColour)
            {
                var col = Application.Current.Resources["GradientBackground"] as GradientBrush;
                txtCaption.Foreground = col;
            }
        }

        /// <summary>
        /// Accessor for check state
        /// </summary>
        /// <returns>Whether the control is checked/ticked</returns>
        public bool IsChecked()
        {
            return _switchState;
        }

        /// <summary>
        /// Assigns callbacks to the control
        /// </summary>
        /// <param name="cbChecked">Function to call when the box is checked</param>
        /// <param name="cbUnchecked">Function to call when the box is unchecked</param>
        internal void AddCallbacks(Action cbChecked, Action cbUnchecked)
        {
            CheckedCallback = cbChecked;
            UncheckedCallback = cbUnchecked;
        }
    }
}
