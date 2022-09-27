using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Cookalong
{
    internal class PopupController
    {
        public static void AboveAll(UserControl control)
        {
            Panel.SetZIndex(control, 1000);
            Grid.SetColumnSpan(control, 1000);
            Grid.SetRowSpan(control, 1000);
        }
    }
}
