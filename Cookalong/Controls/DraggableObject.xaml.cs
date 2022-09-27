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
        Timer _clickDragTimer;
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

        private void _clickDragTimer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            _clickDragTimer.Stop();

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

        private void ClickStart_()
        {
            _mouseDown = true;
            _clickDragTimer.Start();
            _timerRunning = true;
        }

        private void ClickEnd_()
        {
            if (_timerRunning)
            {
                _timerRunning = false;
                _clickDragTimer.Stop();

                _parent?.EditStep(this);
            }
            else
            {
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

        private void UserControl_TouchDown(object sender, TouchEventArgs e)
        {
            ClickStart_();
        }

        private void UserControl_TouchUp(object sender, TouchEventArgs e)
        {
            ClickEnd_();
        }
    }
}
