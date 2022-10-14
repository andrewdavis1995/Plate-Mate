using System.Windows.Controls;
using System.Windows.Media;

namespace Cookalong.Controls
{
    /// <summary>
    /// Interaction logic for GanttTitle.xaml
    /// </summary>
    public partial class GanttTitle : UserControl
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data">Data to display</param>
        public GanttTitle(string data)
        {
            InitializeComponent();
            txtContent.Text = data;
        }

        /// <summary>
        /// Changes the colour of the control while it's object is being dragged
        /// </summary>
        public void SetColour()
        {
            Background = new SolidColorBrush(Color.FromArgb(50, 255, 255, 255));
        }

        /// <summary>
        /// Resets the colour of the control once it's object is no longer being dragged
        /// </summary>
        public void ResetColour()
        {
            Background = null;
        }
    }
}
