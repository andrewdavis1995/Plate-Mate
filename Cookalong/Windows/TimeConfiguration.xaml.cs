using Andrew_2_0_Libraries.Models;
using Cookalong.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Cookalong.Windows
{/// <summary>
 /// Interaction logic for DragWindow.xaml
 /// </summary>
    public partial class TimeConfiguration : Window
    {
        Timer _timer;   // control moving/dragging

        bool _leftBound = false;
        bool _rightBound = false;
        double _offset = 0f;
        double _minLeft = 0;

        double STEP_SIZE = 20d;

        const int CURSOR_OFFSET = 25;
        const double SEGMENT_STEP_MINS = 0.5d;
        const int MAX_MINS = 90;

        DraggableObjectGantt? _dragging = null;  // currently being moved

        List<MethodStep> _steps = new List<MethodStep>();

        List<MethodStep> _completeInstructions = new List<MethodStep>();

        /// <summary>
        /// Constructor
        /// </summary>
        public TimeConfiguration(List<MethodStep> steps)
        {
            InitializeComponent();
            _steps = steps;

            // initialise timer
            _timer = new Timer(0.01f);
            _timer.Elapsed += _timer_Elapsed;
        }

        /// <summary>
        /// Deletes the specified object from the list
        /// </summary>
        /// <param name="draggableObject">Object to delete</param>
        internal void DeleteMethodStep(DraggableObjectGantt draggableObject)
        {
            stckData.Children.Remove(draggableObject);
        }

        /// <summary>
        /// A control has started to be dragged
        /// </summary>
        internal void StartDrag(DraggableObjectGantt draggableObject, bool leftBound, bool rightBound, double offset, int index)
        {
            _dragging = draggableObject;
            _leftBound = leftBound;
            _rightBound = rightBound;
            _offset = offset;

            // this logic should be re-enabled (remove false check) if decide to stop items being dragged before previous one
            // likely behaviour will be that when timing is saved, it updates the order of the previous list
            if (index > 0 && false)
            {
                var oneUp = stckData.Children[index - 1] as DraggableObjectGantt;
                _minLeft = oneUp != null ? oneUp.Margin.Left : 0d;
            }
            else
            {
                _minLeft = 0;
            }

            // update appearance of the control
            _dragging.Highlight.Visibility = Visibility.Visible;

            // start following the mouse
            _timer.Start();

            Point relativePoint = draggableObject.TransformToAncestor(stckData)
                          .Transform(new Point(0, 0));

            timePopup.Margin = new Thickness(draggableObject.Margin.Left, relativePoint.Y - draggableObject.ActualHeight, 0, 0);
            timePopup.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Called per "tick" of the timer (for making the control follow the mouse)
        /// </summary>
        private void _timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            // separate thread, so need to use Dispatcher
            _dragging?.Dispatcher.Invoke(() =>
            {
                // just in case
                if (_dragging == null) return;

                Point mousePos = Mouse.GetPosition(this);

                if (_leftBound)
                {
                    var mouseX = RoundValue_(mousePos.X - CURSOR_OFFSET);

                    // adjust width on left
                    var widthDiff = (_dragging.Margin.Left - mouseX);

                    _dragging.Margin = new Thickness(mouseX, _dragging.Margin.Top, 0, 0);

                    if ((_dragging.Width + widthDiff) > STEP_SIZE)
                        _dragging.Width += widthDiff;

                    lblTime.Content = TimeOutput(Duration_(_dragging));
                    lblTimeLabel.Content = "Duration";
                }
                else if (_rightBound)
                {
                    // adjust width on right
                    var widthDiff = RoundValue_(mousePos.X - (_dragging.Margin.Left + _dragging.Width));

                    if ((_dragging.Width + widthDiff) > STEP_SIZE)
                        _dragging.Width += widthDiff;

                    lblTime.Content = TimeOutput(Duration_(_dragging));
                    lblTimeLabel.Content = "Duration";
                }
                else
                {
                    mousePos.X -= _offset;

                    if (RightBoundCheck_(mousePos) && LeftBoundCheck_(mousePos))
                    {
                        // set margin/position of the control to same as mouse (use width and height to centralise the control on the mouse)
                        _dragging.Margin = new Thickness(RoundValue_(mousePos.X - _dragging.Width / 2), _dragging.Margin.Top, 0, 0);
                    }

                    lblTime.Content = TimeOutput(StartTime_(_dragging));
                    lblTimeLabel.Content = "Start Time";
                }
                timePopup.Margin = new Thickness(_dragging.Margin.Left + (_dragging.ActualWidth / 2) - (timePopup.ActualWidth / 2), timePopup.Margin.Top, 0, 0);
            }
            );
        }

        double RoundValue_(double value)
        {
            return (STEP_SIZE * (Math.Round(value / STEP_SIZE)));
        }

        bool RightBoundCheck_(Point mousePos)
        {
            var valid = (mousePos.X - _dragging.Width / 2 + _dragging.Width) < stckData.ActualWidth;
            return valid;
        }

        bool LeftBoundCheck_(Point mousePos)
        {
            var valid = (mousePos.X - _dragging.Width / 2) > _minLeft;
            return valid;
        }

        /// <summary>
        /// Event handler for when the mouse is released on the window
        /// </summary>
        private void stckData_MouseUp(object sender, MouseButtonEventArgs e)
        {
            MouseReleased();
        }

        public void MouseReleased()
        {
            // only do this if something is being dragged
            if (_dragging == null) return;

            // stop following the mouse
            _timer.Stop();

            // change appearance of the control back to normal
            _dragging.Highlight.Visibility = Visibility.Collapsed;

            // ensure we don't go off the edge
            if (_dragging.Width + _dragging.Margin.Left > stckData.ActualWidth)
                _dragging.Width = stckData.ActualWidth - _dragging.Margin.Left;
            if (_dragging.Margin.Left < 0)
                _dragging.Margin = new Thickness(0, _dragging.Margin.Top, _dragging.Margin.Right, _dragging.Margin.Bottom);

            // make sure it has the latest times
            _dragging.UpdateTimes(StartTime_(_dragging), Duration_(_dragging));

            // nothing is being dragged any more
            _dragging = null;

            timePopup.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Event handler for when the mouse leaves the main window
        /// </summary>
        private void MouseLeftWindow(object sender, MouseEventArgs e)
        {
            MouseReleased();
        }

        /// <summary>
        /// Add a new step to the list
        /// </summary>
        private void NewStep_(MethodStep step)
        {
            // TODO: need to pass in an Instruction object, and load existing start/durations

            // create a control for the specified step
            var newObject = new DraggableObjectGantt(this, step, stckData.Children.Count)
            {
                Width = (STEP_SIZE * 30),
                HorizontalAlignment = HorizontalAlignment.Left
            };

            // add control
            stckData.Children.Add(newObject);
        }

        /// <summary>
        /// Event handler for when the window loads
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SetStepSize_();
            foreach(var item in _steps)
            {
                NewStep_(item);
            }
        }

        /// <summary>
        /// Event handler for when the size of the window changes
        /// </summary>
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // update the physical size of each "step"
            SetStepSize_();
        }

        /// <summary>
        /// Sets the size of each "step" - changes when window size changes
        /// </summary>
        private void SetStepSize_()
        {
            STEP_SIZE = (stckData.ActualWidth / MAX_MINS) * SEGMENT_STEP_MINS;
        }

        /// <summary>
        /// Event handler for when the mouse leaves the scrollviewer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScrollViewer_MouseLeave(object sender, MouseEventArgs e)
        {
            // ensure we stop dragging objects
            MouseReleased();
        }

        /// <summary>
        /// Event handler for clicking the "Begin" button
        /// </summary>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _completeInstructions = new List<MethodStep>();

            // loop through each object, and create an instruction for each
            foreach (DraggableObjectGantt obj in stckData.Children)
            {
                var i = new MethodStep(obj.txtData.Text, StartTime_(obj), Duration_(obj));
                _completeInstructions.Add(i);
            }

            DialogResult = true;
            Close();
        }

        /// <summary>
        /// Returns the configured instructions
        /// </summary>
        /// <returns>Configured list</returns>
        public List<MethodStep> GetInstructions()
        {
            return _completeInstructions;
        }

        /// <summary>
        /// Calculate the start time of a step based on the margin of the control
        /// </summary>
        /// <param name="obj">The control to get start time for</param>
        /// <returns>The start time of this step</returns>
        private int StartTime_(DraggableObjectGantt obj)
        {
            // calculate start time
            var start = (int)Math.Round(((obj.Margin.Left / STEP_SIZE) * SEGMENT_STEP_MINS * 60));
            return start;
        }

        /// <summary>
        /// Calculate the duration of a step based on the width of the control
        /// </summary>
        /// <param name="obj">The control to get duration for</param>
        /// <returns>The duration of this step</returns>
        private int Duration_(DraggableObjectGantt obj)
        {
            // calculate duration
            var duration = (int)Math.Round(((obj.ActualWidth / STEP_SIZE) * SEGMENT_STEP_MINS * 60));
            return duration;
        }

        /// <summary>
        /// Gets the output for the time in a nicer format - different to the standard one
        /// </summary>
        /// <param name="totalSeconds">The total number of seconds</param>
        /// <returns>Formatted string</returns>
        string TimeOutput(int totalSeconds)
        {
            // break into individual components
            var hours = (totalSeconds / 60) / 60;
            var minutes = (totalSeconds - (hours * 60 * 60)) / 60;
            var seconds = totalSeconds - (hours * 60 * 60) - (minutes * 60);

            // construct string
            var str = "";
            if (hours > 0) str += hours + "hrs ";
            str += minutes + "mins ";
            str += seconds + "s";

            return str.Trim();
        }
    }
}
