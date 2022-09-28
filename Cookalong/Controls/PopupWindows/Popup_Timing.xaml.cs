using System;
using System.Windows.Controls;

namespace Cookalong.Controls.PopupWindows
{
    /// <summary>
    /// Interaction logic for Popup_Timing.xaml
    /// </summary>
    public partial class Popup_Timing : UserControl
    {
        Action? _cancelCallback;
        Action<uint> _confirmCallback;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="existing">The existing value (to edit)</param>
        /// <param name="cancelCallback">Function to call when cancel is pressed</param>
        /// <param name="confirmCallback">Function to call when confirm is pressed</param>
        public Popup_Timing(uint existing, Action cancelCallback, Action<uint> confirmCallback)
        {
            InitializeComponent();

            // store callbacks
            _cancelCallback = cancelCallback;
            _confirmCallback = confirmCallback;

            // configure numeric input
            inpNumeric.Initialise(false, 480, 3);
            inpNumeric.SetInputValue(existing);

            // configure buttons
            cmdCancel.Configure("Cancel");
            cmdConfirm.Configure("Confirm");
        }

        /// <summary>
        /// Event handler for the cancel button
        /// </summary>
        private void cmdCancel_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _cancelCallback?.Invoke();
        }

        /// <summary>
        /// Event handler for the confirm button
        /// </summary>
        private void cmdConfirm_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _confirmCallback?.Invoke((uint)inpNumeric.GetValue());
        }
    }
}
