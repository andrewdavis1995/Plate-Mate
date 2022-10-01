using System.IO;
using System.Windows.Controls;

namespace Cookalong.Controls
{
    /// <summary>
    /// Interaction logic for BackupOption.xaml
    /// </summary>
    public partial class BackupOption : UserControl
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="filename">The name of the file to display</param>
        public BackupOption(string filename)
        {
            InitializeComponent();

            // check file exists
            if (File.Exists(filename))
            {
                // get date the file was written
                var date = File.GetLastWriteTime(filename);

                // display data
                lblDate.Content = date.ToString("dd/MM/yyyy HH:mm:ss");
                lblFilename.Content = Path.GetFileName(filename);
            }

            cmdRestore.Configure("Restore");
        }

        /// <summary>
        /// Event handler for when the mouse enters this control
        /// </summary>
        private void UserControl_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            cmdRestore.Visibility = System.Windows.Visibility.Visible;
        }

        /// <summary>
        /// Event handler for when the mouse leaves this control
        /// </summary>
        private void UserControl_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            cmdRestore.Visibility = System.Windows.Visibility.Collapsed;
        }
    }
}
