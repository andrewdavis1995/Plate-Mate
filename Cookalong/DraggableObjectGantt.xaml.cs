using Andrew_2_0_Libraries.Models;
using Cookalong.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Cookalong
{/// <summary>
 /// Interaction logic for DraggableObject.xaml
 /// </summary>
    public partial class DraggableObjectGantt : UserControl
    {
        TimeConfiguration? _parent;
        bool _leftBound = false;
        bool _rightBound = false;
        int _index = 0;
        MethodStep _step;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parent">The window that the control can be dragged around</param>
        /// <param name="content">The data to display on the control</param>
        public DraggableObjectGantt(TimeConfiguration parent, MethodStep step, int index)
        {
            InitializeComponent();
            _step = step;
            txtData.Text = step.GetMethod();
            _parent = parent;
            _index = index;
        }

        /// <summary>
        /// Event handler for when the user presses the mouse button on the control
        /// </summary>
        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var offset = e.GetPosition(this).X - (ActualWidth / 2);
            _parent?.StartDrag(this, _leftBound, _rightBound, offset, _index);
        }

        /// <summary>
        /// Event handler for when the user releases the mouse button on the control
        /// </summary>
        private void UserControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _parent?.MouseReleased();
        }

        /// <summary>
        /// Event handler for when the delete button is pressed
        /// </summary>
        private void cmdDelete_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _parent?.DeleteMethodStep(this);
        }

        /// <summary>
        /// Event handler for when the mouse enters the left drag window
        /// </summary>
        private void LeftBorder_MouseEnter(object sender, MouseEventArgs e)
        {
            _leftBound = true;
        }

        /// <summary>
        /// Event handler for when the mouse leaves the left drag window
        /// </summary>
        private void LeftBorder_MouseLeave(object sender, MouseEventArgs e)
        {
            _leftBound = false;
        }

        /// <summary>
        /// Event handler for when the mouse enters the right drag window
        /// </summary>
        private void RightBorder_MouseEnter(object sender, MouseEventArgs e)
        {
            _rightBound = true;
        }

        /// <summary>
        /// Event handler for when the mouse leaves the right drag window
        /// </summary>
        private void RightBorder_MouseLeave(object sender, MouseEventArgs e)
        {
            _rightBound = false;
        }

        /// <summary>
        /// Updates the time values stored by this control
        /// </summary>
        /// <param name="start">The start time</param>
        /// <param name="duration">The duration of this step</param>
        public void UpdateTimes(int start, int duration)
        {
            _step.UpdateTimes(start, duration);
        }
    }
}
