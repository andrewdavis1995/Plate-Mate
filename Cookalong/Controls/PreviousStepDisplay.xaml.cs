using System.Windows.Controls;

namespace Cookalong.Controls
{
    /// <summary>
    /// Interaction logic for PreviousStepDisplay.xaml
    /// </summary>
    public partial class PreviousStepDisplay : UserControl
    {
        public PreviousStepDisplay(string message)
        {
            InitializeComponent();
            txtContent.Text = message;
        }
    }
}
