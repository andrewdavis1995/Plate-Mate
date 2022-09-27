using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Color = System.Windows.Media.Color;

namespace Cookalong.Controls
{
    /// <summary>
    /// Interaction logic for Input_Checkbox.xaml
    /// </summary>
    public partial class Input_Checkbox : UserControl
    {
        private bool _switchState = true;
        Action CheckedCallback;
        Action UncheckedCallback;

        public Input_Checkbox()
        {
            InitializeComponent();
        }

        private void imgSwitch_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _switchState = !_switchState;
            UpdateDisplay(_switchState);
        }

        public void UpdateDisplay(bool state)
        {
            _switchState = state;

            if (_switchState)
            {
                CheckedCallback?.Invoke();
            }
            else
            {
                UncheckedCallback?.Invoke();
            }

            imgTick.Visibility = _switchState ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
            cmdCheck.Background = _switchState ? cmdCheck.BorderBrush : new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
        }

        internal void AddTitle(string content, bool changeColour = false)
        {
            txtCaption.Text = content;

            if(changeColour)
            {
                var col = Application.Current.Resources["GradientBackground"] as GradientBrush;
                txtCaption.Foreground = col;
            }
        }

        public bool IsChecked()
        {
            return _switchState;
        }

        internal void AddCallbacks(Action cbChecked, Action cbUnchecked)
        {
            CheckedCallback = cbChecked;
            UncheckedCallback = cbUnchecked;
        }
    }
}
