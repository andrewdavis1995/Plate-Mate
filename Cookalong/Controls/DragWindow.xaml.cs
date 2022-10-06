using Cookalong.Controls.PopupWindows;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace Cookalong.Controls
{
    /// <summary>
    /// Interaction logic for DragWindow.xaml
    /// </summary>
    public partial class DragWindow : UserControl
    {
        readonly Timer _timer;   // control moving/dragging

        int _originalIndex = 0; // where it came from
        int _newIndex = -1; // where it is going

        DraggableObject? _dragging = null;  // currently being moved
        Popup_NewRecipe ? _popupNewRecipe = null;
        readonly DraggableObject _spacer;    // show the slot where the dragged item will be dropped

        /// <summary>
        /// Constructor
        /// </summary>
        public DragWindow()
        {
            InitializeComponent();

            // initialise timer
            _timer = new Timer(0.01f);
            _timer.Elapsed += Timer_Elapsed;

            // initialise the control for creating "gaps" in the stack panel
            _spacer = new()
            {
                Visibility = Visibility.Collapsed
            };
            stckData.Children.Add(_spacer);
        }

        /// <summary>
        /// Stores an instance of the popup
        /// </summary>
        /// <param name="newRecipe">The popup</param>
        public void Configure(Popup_NewRecipe newRecipe)
        {
            _popupNewRecipe = newRecipe;
        }

        /// <summary>
        /// Adds an item to the stack panel
        /// </summary>
        /// <param name="content">The content to add</param>
        public void AddItem(string content, Grid parentGrid)
        {
            var newObject = new DraggableObject(this, content, parentGrid);
            stckData.Children.Add(newObject);
        }

        /// <summary>
        /// A control has started to be dragged
        /// </summary>
        /// <param name="draggableObject">The item being dragged</param>
        internal void StartDrag(DraggableObject draggableObject)
        {
            _dragging = draggableObject;

            // align to top left corner (so margin can be used for position)
            _dragging.HorizontalAlignment = HorizontalAlignment.Left;
            _dragging.VerticalAlignment = VerticalAlignment.Top;

            // update appearance of the control
            ApplyShadow_();
            _dragging.Highlight.Visibility = Visibility.Visible;
            _dragging.Width = stckData.ActualWidth;

            // highlight the dragging area
            stckData.Background = new SolidColorBrush(Color.FromArgb(20, 80, 80, 255));

            // keep a record of where this item came from
            GetOriginalIndex();

            // remove from stack panel and move to main grid/window
            stckData.Children.Remove(_dragging);
            MainGrid.Children.Add(_dragging);

            // start following the mouse
            _timer.Start();

            // make the spacer (for gaps) active (but invisible)
            _spacer.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Adds a shadow to the dragged control
        /// </summary>
        private void ApplyShadow_()
        {
            // just in case
            if (_dragging == null) return;

            // Initialize shadow
            var myDropShadowEffect = new DropShadowEffect()
            {
                Color = Color.FromArgb(1, 0, 0, 0), // black
                Direction = 270d,   // down
                ShadowDepth = 5d,
                BlurRadius = 8f,
                Opacity = 0.2f
            };

            // apply effect to the control
            _dragging.Effect = myDropShadowEffect;
        }

        /// <summary>
        /// Find the control in the stack and store the position it is at
        /// </summary>
        private void GetOriginalIndex()
        {
            _originalIndex = 0;

            // iterate through all positions until a match is found, or we run out of elements
            for (; _originalIndex < stckData.Children.Count; _originalIndex++)
            {
                if (stckData.Children[_originalIndex] == _dragging)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Called per "tick" of the timer (for making the control follow the mouse)
        /// </summary>
        private void Timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            // separate thread, so need to use Dispatcher
            _dragging?.Dispatcher.Invoke(() =>
            {
                // just in case
                if (_dragging == null) return;

                // remove the spacer control (creates "gaps")
                stckData.Children.Remove(_spacer);

                // set margin/position of the control to same as mouse (use width and height to centralise the control on the mouse)
                Point mousePos = Mouse.GetPosition(this);
                _dragging.Margin = new Thickness(mousePos.X - _dragging.Width / 2, mousePos.Y - _dragging.Height / 2, 0, 0);

                // get position within the stack panel
                var posRelative = Mouse.GetPosition(stckData);

                // if within the stack panel, work out which index the item should be inserted at
                if (posRelative.Y > 0 && posRelative.X > 0)
                {
                    // find position in the stack
                    _newIndex = (int)(posRelative.Y / _dragging.ActualHeight);
                    if (_newIndex >= stckData.Children.Count)
                        _newIndex = stckData.Children.Count;

                    // insert in correct position
                    stckData.Children.Insert(_newIndex, _spacer);
                }
                else
                {
                    // otherwise, outwith the bounds. Add at end
                    _newIndex = -1;
                    stckData.Children.Add(_spacer);
                }
            }
            );
        }

        /// <summary>
        /// Returns the number of items in the data stack
        /// </summary>
        /// <returns></returns>
        internal int NumElements()
        {
            return stckData.Children.Count;
        }

        /// <summary>
        /// Event handler for when the mouse is released on the window
        /// </summary>
        private void StckData_MouseUp(object sender, MouseButtonEventArgs e)
        {
            MouseReleased();
        }

        /// <summary>
        /// When the mouse button is released
        /// </summary>
        public void MouseReleased()
        {
            // only do this if something is being dragged
            if (_dragging == null) return;

            // stop following the mouse
            _timer.Stop();

            // fill the content of the stack panel
            _dragging.HorizontalAlignment = HorizontalAlignment.Stretch;
            _dragging.Margin = new Thickness(0, 1, 0, 1);

            // reset drag area to usual colour
            stckData.Background = new SolidColorBrush(Color.FromArgb(0, 80, 80, 255));  // TODO: use resource

            // place back in the stack panel - index is based on current position
            MainGrid.Children.Remove(_dragging);
            stckData.Children.Insert(_newIndex >= 0 ? _newIndex : _originalIndex, _dragging);

            // change appearance of the control back to normal
            _dragging.Highlight.Visibility = Visibility.Collapsed;
            _dragging.Effect = null;

            // nothing is being dragged any more
            _dragging = null;

            // remove the spacer control (used to create gap in stack panel)
            _spacer.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Event handler for when the mouse leaves the main window
        /// </summary>
        private void MouseLeftWindow(object sender, MouseEventArgs e)
        {
            MouseReleased();
        }

        /// <summary>
        /// When a step is clicked and needs to be edited
        /// </summary>
        /// <param name="draggableObject">The control to edit</param>
        internal void EditStep(DraggableObject draggableObject)
        {
            _popupNewRecipe?.EditStep(draggableObject);
        }
    }
}
