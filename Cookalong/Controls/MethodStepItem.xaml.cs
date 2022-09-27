using System.Windows.Controls;

namespace Cookalong.Controls
{
    /// <summary>
    /// Interaction logic for MethodStepItem.xaml
    /// </summary>
    public partial class MethodStepItem : UserControl
    {
        public MethodStepItem()
        {
            InitializeComponent();
        }

        public MethodStepItem(int stepNum, string content)
        {
            InitializeComponent();
            txtContent.Text = content;
            txtStep.Text = "Step " + stepNum;
        }
    }
}
