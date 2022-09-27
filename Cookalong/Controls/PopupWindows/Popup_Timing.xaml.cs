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

        public Popup_Timing(uint existing, Action cancelCallback, Action<uint> confirmCallback)
        {
            InitializeComponent();
            _cancelCallback = cancelCallback;
            _confirmCallback = confirmCallback;

            inpNumeric.Initialise(false, 480, 3);
            inpNumeric.SetInputValue(existing);

            cmdCancel.Configure("Cancel");
            cmdConfirm.Configure("Confirm");
        }

        private void cmdCancel_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _cancelCallback?.Invoke();
        }

        private void cmdConfirm_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _confirmCallback?.Invoke((uint)inpNumeric.GetValue());
        }
    }
}
