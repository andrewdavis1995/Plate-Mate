using System.Windows.Controls;

namespace Cookalong.Controls
{
    /// <summary>
    /// Interaction logic for MethodStepItem.xaml
    /// </summary>
    public partial class MethodStepItem : UserControl
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public MethodStepItem()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="stepNum">The number of this step</param>
        /// <param name="content">The content to display</param>
        public MethodStepItem(int stepNum, string content)
        {
            InitializeComponent();
            txtContent.Text = content;
            txtStep.Text = "Step " + stepNum;
        }
    }
}
