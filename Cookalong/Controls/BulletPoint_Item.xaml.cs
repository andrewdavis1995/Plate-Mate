using System.Windows.Controls;

namespace Cookalong.Controls
{
    /// <summary>
    /// Interaction logic for BulletPoint_Item.xaml
    /// </summary>
    public partial class BulletPoint_Item : UserControl
    {
        public BulletPoint_Item()
        {
            InitializeComponent();
        }

        public BulletPoint_Item(string content)
        {
            InitializeComponent();
            txtContent.Text = content;
        }
    }
}
