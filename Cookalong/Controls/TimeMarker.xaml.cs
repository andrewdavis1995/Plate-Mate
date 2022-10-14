using Cookalong.Helpers;
using System.Windows.Controls;

namespace Cookalong.Controls
{
    /// <summary>
    /// Interaction logic for TimeMarker.xaml
    /// </summary>
    public partial class TimeMarker : UserControl
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public TimeMarker()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Sets the text display to a string
        /// </summary>
        /// <param name="text">String to display</param>
        internal void SetContent(string text)
        {
            txtTime.Text = text;
        }

        /// <summary>
        /// Sets the text display to a time output
        /// </summary>
        /// <param name="text">Number of seconds to display</param>
        internal void SetTime(int totalSeconds)
        {
            txtTime.Text = StringHelper.TimeConfigOutput(totalSeconds, true);
        }
    }
}
