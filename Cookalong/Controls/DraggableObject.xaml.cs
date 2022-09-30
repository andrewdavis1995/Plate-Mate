using System;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Cookalong.Controls
{
    /// <summary>
    /// Interaction logic for DraggableObject.xaml
    /// </summary>
    public partial class DraggableObject : UserControl
    {
        DragWindow? _parent;
        Timer _clickDragTimer = new Timer();
        bool _mouseDown;
        bool _timerRunning = false;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parent">The window that the control can be dragged around</param>
        /// <param name="content">The data to display on the control</param>
        public DraggableObject(DragWindow parent, string content)
        {
            InitializeComponent();
            txtData.Text = content;
            _parent = parent;

            // configure timer
            _clickDragTimer = new Timer();
            _clickDragTimer.Interval = 500;
            _clickDragTimer.Elapsed += _clickDragTimer_Elapsed;
        }

        /// <summary>
        /// Blank constructor
        /// </summary>
        public DraggableObject()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Event handler for each "tick" of the timer which checks for click or drag
        /// </summary>
        private void _clickDragTimer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            // stop timer
            _clickDragTimer.Stop();

            // if the mouse is still down, begin a drag
            if (_mouseDown)
                Dispatcher.Invoke(() => _parent?.StartDrag(this));

            _timerRunning = false;
        }


        /// <summary>
        /// Event handler for when the user presses the mouse button on the control
        /// </summary>
        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ClickStart_();
        }

        /// <summary>
        /// When the mouse button gets pressed on the control
        /// </summary>
        private void ClickStart_()
        {
            // record the click
            _mouseDown = true;
            _clickDragTimer.Start();
            _timerRunning = true;
        }

        /// <summary>
        /// When the mouse button gets released on the control
        /// </summary>
        private void ClickEnd_()
        {
            // if the timer is still running (checking for click or drag), this is a click
            if (_timerRunning)
            {
                // stop timer, and do an edit
                _timerRunning = false;
                _clickDragTimer.Stop();
                _parent?.EditStep(this);
            }
            else
            {
                // stop dragging
                _parent?.MouseReleased();
            }

            _mouseDown = false;
        }

        /// <summary>
        /// Event handler for when the user releases the mouse button on the control
        /// </summary>
        private void UserControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ClickEnd_();
        }

        /// <summary>
        /// Event handler for the touch pad press
        /// </summary>
        private void UserControl_TouchDown(object sender, TouchEventArgs e)
        {
            ClickStart_();
        }

        /// <summary>
        /// Event handler for the touch pad release
        /// </summary>
        private void UserControl_TouchUp(object sender, TouchEventArgs e)
        {
            ClickEnd_();
        }
    }
}
