using System.Windows.Controls;

namespace Cookalong.Helpers
{
    internal class PopupController
    {
        /// <summary>
        /// Moves the specified control to the front
        /// </summary>
        /// <param name="control">The control to display</param>
        public static void AboveAll(UserControl control)
        {
            Panel.SetZIndex(control, 1000);
            Grid.SetColumnSpan(control, 1000);
            Grid.SetRowSpan(control, 1000);
        }
    }
}
