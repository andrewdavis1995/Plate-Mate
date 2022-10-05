using Cookalong.Controls;
using System;
using System.Windows.Controls;
using System.Windows.Media;

namespace Cookalong.Helpers
{
    internal class ControlHelper : UserControl
    {
        /// <summary>
        /// Sets the appearance of a button
        /// </summary>
        /// <param name="button">The button to update</param>
        /// <param name="msg">Message to display</param>
        public static void SetButtonAppearance_(Input_Button button, string msg)
        {
            button.lblMessage.Content = msg;

            try
            {
                // specified button
                var res = button.FindResource("Button" + msg);
                button.bigBorder.BorderBrush = res as SolidColorBrush;
            }
            catch (Exception)
            {
                // default
                var res = button.FindResource("ButtonDefault");
                button.bigBorder.BorderBrush = res as SolidColorBrush;
            }
        }
    }
}
