using System.Windows.Controls;

namespace Cookalong.Controls
{
    /// <summary>
    /// Interaction logic for PreviousStepDisplay.xaml
    /// </summary>
    public partial class PreviousStepDisplay : UserControl
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">The message to display</param>
        public PreviousStepDisplay(string message)
        {
            InitializeComponent();
            txtContent.Text = message;
        }
    }
}
