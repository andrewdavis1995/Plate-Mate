using Andrew_2_0_Libraries.Models;
using Cookalong.Controls;
using Cookalong.Controls.PopupWindows;
using Cookalong.Helpers;
using System;
using System.Collections.Generic;
using System.Timers;
using System.Windows;
using System.Windows.Input;

namespace Cookalong.Windows
{
    /// <summary>
    /// Interaction logic for DragWindow.xaml
    /// </summary>
    public partial class TimeConfiguration : Window
    {
        const int CURSOR_OFFSET = 25;
        const int ZOOM_CHANGE_MINUTES = 30;
        const int MAX_ZOOM_MINUTES = 150;
        const int MIN_ZOOM_MINUTES = 60;
        const double SEGMENT_STEP_MINS = 0.5d;

        Timer _timer;   // control moving/dragging

        bool _leftBound = false;
        bool _rightBound = false;
        double _offset = 0f;
        double _minLeft = 0;

        double STEP_SIZE = 20d;
        int MAX_MINS = 60;

        DraggableObjectGantt? _dragging = null;  // currently being moved
        Popup_Confirmation? _confirmationPopup = null;

        List<MethodStep> _steps = new List<MethodStep>();
        List<MethodStep> _completeInstructions = new List<MethodStep>();

        /// <summary>
        /// Constructor
        /// </summary>
        public TimeConfiguration(List<MethodStep> steps)
        {
            InitializeComponent();

            // configure buttons
            cmdSave.Configure("Save and Exit");
            cmdReset.Configure("Reset");
            cmdCancel.Configure("Cancel");

            _steps = steps;

            // initialise timer
            _timer = new Timer(0.01f);
            _timer.Elapsed += _timer_Elapsed;
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
            _minLeft = 0;

            // update appearance of the control
            _dragging.Highlight.Visibility = Visibility.Visible;

            // start following the mouse
            _timer.Start();

            Point relativePoint = draggableObject.TransformToAncestor(stckData)
                          .Transform(new Point(0, 0));

            // show the time popup
            timePopup.Margin = new Thickness(draggableObject.Margin.Left, relativePoint.Y - draggableObject.ActualHeight, 0, 0);
            timePopup.Visibility = !(leftBound || rightBound) ? Visibility.Visible : Visibility.Collapsed;

            // highlight the title
            var title = stckTitles.Children[index] as GanttTitle;
            if (title != null)
            {
                title.SetColour();
            }
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

                Point mousePos = Mouse.GetPosition(stckData);

                if (_leftBound)
                {
                    var mouseX = RoundValue_(mousePos.X - CURSOR_OFFSET);

                    // adjust width on left
                    var widthDiff = (_dragging.Margin.Left - mouseX);

                    _dragging.Margin = new Thickness(mouseX, _dragging.Margin.Top, 0, 0);

                    if ((_dragging.Width + widthDiff) > STEP_SIZE)
                        _dragging.Width += widthDiff;

                    _dragging.txtData.Text = StringHelper.TimeConfigOutput(Duration_(_dragging), true);
                }
                else if (_rightBound)
                {
                    // adjust width on right
                    var widthDiff = RoundValue_(mousePos.X - (_dragging.Margin.Left + _dragging.Width));

                    if ((_dragging.Width + widthDiff) > STEP_SIZE)
                        _dragging.Width += widthDiff;

                    _dragging.txtData.Text = StringHelper.TimeConfigOutput(Duration_(_dragging), true);
                }
                else
                {
                    mousePos.X -= _offset;

                    if (RightBoundCheck_(mousePos) && LeftBoundCheck_(mousePos))
                    {
                        // set margin/position of the control to same as mouse (use width and height to centralise the control on the mouse)
                        _dragging.Margin = new Thickness(RoundValue_(mousePos.X - _dragging.Width / 2), _dragging.Margin.Top, 0, 0);
                    }

                    lblTime.Content = StringHelper.TimeConfigOutput(StartTime_(_dragging));
                    lblTimeLabel.Content = "Start Time";
                }
                timePopup.Margin = new Thickness(_dragging.Margin.Left + (_dragging.ActualWidth / 2) - (timePopup.ActualWidth / 2), timePopup.Margin.Top, 0, 0);
            }
            );
        }

        /// <summary>
        /// Rounds a value to the nearest step
        /// </summary>
        /// <param name="value">The value to round</param>
        /// <returns>The rounded value</returns>
        double RoundValue_(double value)
        {
            return (STEP_SIZE * (Math.Round(value / STEP_SIZE)));
        }

        /// <summary>
        /// Checks the value is not beyond the right bound
        /// </summary>
        /// <param name="mousePos">Position of the mouse</param>
        /// <returns>Whether the mouse is within the bounds</returns>
        bool RightBoundCheck_(Point mousePos)
        {
            if (_dragging == null) return false;

            // check if in bounds
            var valid = (mousePos.X - _dragging.Width / 2 + _dragging.Width) < stckData.ActualWidth;
            return valid;
        }

        /// <summary>
        /// Checks the value is not beyond the left bound
        /// </summary>
        /// <param name="mousePos">Position of the mouse</param>
        /// <returns>Whether the mouse is within the bounds</returns>
        bool LeftBoundCheck_(Point mousePos)
        {
            if (_dragging == null) return false;

            // check if in bounds
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

        /// <summary>
        /// When the mouse gets released
        /// </summary>
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

            // reset colour of titles
            foreach (GanttTitle gt in stckTitles.Children)
            {
                gt.ResetColour();
            }
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
            // create a control for the specified step
            var newObject = new DraggableObjectGantt(this, step, stckData.Children.Count)
            {
                Width = (STEP_SIZE * 10),
                HorizontalAlignment = HorizontalAlignment.Left
            };

            // update width if already set
            if (step.GetDuration() > 0)
            {
                newObject.Width = Width_(step);
                newObject.Margin = new Thickness(Left_(step), newObject.Margin.Top, newObject.Margin.Right, newObject.Margin.Bottom);
            }

            // display text
            newObject.txtData.Text = StringHelper.TimeConfigOutput(Duration_(newObject), true);

            // add control
            stckData.Children.Add(newObject);

            // add a label
            stckTitles.Children.Add(new GanttTitle(step.GetMethod()) { Height = newObject.Height });
        }

        /// <summary>
        /// Event handler for when the window loads
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SetStepSize_();
            foreach (var item in _steps)
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

            // update time markers
            SetMarkerPositions_();
            SetMarkerTimes_();
        }

        /// <summary>
        /// Sets the displays on the time markers
        /// </summary>
        void SetMarkerTimes_()
        {
            markStart.SetContent("START");             // start
            markHalf.SetTime((MAX_MINS * 60) / 2);     // quarter
            markQtr.SetTime((MAX_MINS * 60) / 4);      // half
            mark3Qtr.SetTime((MAX_MINS * 60) / 4 * 3); // three-quarters
            markEnd.SetTime(MAX_MINS * 60);            // end
        }

        /// <summary>
        /// Sets the positions on the time markers
        /// </summary>
        void SetMarkerPositions_()
        {
            var gridLeft = colLHS.Width.Value + grdContent.Margin.Left;
            var offset = markStart.ActualWidth / 2;

            // set margins of the time markers
            markStart.Margin = new Thickness(gridLeft - offset, markStart.Margin.Top, 0, 0);
            markQtr.Margin = new Thickness(gridLeft - offset + (stckData.ActualWidth / 4), markStart.Margin.Top, 0, 0);
            markHalf.Margin = new Thickness(gridLeft - offset + (stckData.ActualWidth / 2), markStart.Margin.Top, 0, 0);
            mark3Qtr.Margin = new Thickness(gridLeft - offset + (stckData.ActualWidth / 4 * 3), markStart.Margin.Top, 0, 0);
            markEnd.Margin = new Thickness(gridLeft - offset + stckData.ActualWidth, markStart.Margin.Top, 0, 0);
        }

        /// <summary>
        /// Event handler for when the mouse leaves the scrollviewer
        /// </summary>
        private void ScrollViewer_MouseLeave(object sender, MouseEventArgs e)
        {
            // ensure we stop dragging objects
            MouseReleased();
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
            var duration = (int)Math.Round(((obj.Width / STEP_SIZE) * SEGMENT_STEP_MINS * 60));
            return duration;
        }

        /// <summary>
        /// Calculate the margin based on the start time
        /// </summary>
        /// <param name="step">The step to display</param>
        /// <returns>The margin to set</returns>
        private double Left_(MethodStep step)
        {
            // calculate start time
            var start = (step.GetStart() / (60 * SEGMENT_STEP_MINS)) * STEP_SIZE;
            return start;
        }

        /// <summary>
        /// Calculate the width based on the duration
        /// </summary>
        /// <param name="step">The step to display</param>
        /// <returns>The width to set</returns>
        private double Width_(MethodStep step)
        {
            // calculate duration
            var start = (step.GetDuration() / (60 * SEGMENT_STEP_MINS)) * STEP_SIZE;
            return start;
        }

        /// <summary>
        /// Calculate the duration of a step based on the width of the control
        /// </summary>
        /// <param name="obj">The control to get duration for</param>
        /// <returns>The duration of this step</returns>
        private int Margin_(DraggableObjectGantt obj)
        {
            // calculate duration
            var duration = (int)Math.Round(((obj.ActualWidth / STEP_SIZE) * SEGMENT_STEP_MINS * 60));
            return duration;
        }

        /// <summary>
        /// Event handler for save button
        /// </summary>
        private void cmdSave_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _completeInstructions = new List<MethodStep>();

            // loop through each object, and create an instruction for each
            foreach (DraggableObjectGantt obj in stckData.Children)
            {
                var i = new MethodStep(obj.GetText(), StartTime_(obj), Duration_(obj));
                _completeInstructions.Add(i);
            }

            DialogResult = true;
            Close();
        }

        /// <summary>
        /// Event handler for clicking the reset button
        /// </summary>
        private void cmdReset_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _confirmationPopup = new Popup_Confirmation("Confirm reset", "Are you sure you want to reset the timings?", () =>
            {
                // cancel callback
                MainGrid.Children.Remove(_confirmationPopup);
                if (_confirmationPopup != null)
                    _confirmationPopup.Visibility = Visibility.Collapsed;
            },
            () =>
            {
                // confirm callback
                MainGrid.Children.Remove(_confirmationPopup);

                // remove existing controls
                stckData.Children.Clear();
                stckTitles.Children.Clear();

                // add new controls
                foreach (var step in _steps)
                {
                    var start = step.GetStart();
                    var duration = step.GetDuration();

                    // reset times first
                    step.UpdateTimes(0, 0);
                    NewStep_(step);

                    // restore times in the background (in case time input is cancelled)
                    step.UpdateTimes(start, duration);
                }
            });

            // show popup
            PopupController.AboveAll(_confirmationPopup);
            MainGrid.Children.Add(_confirmationPopup);
        }

        /// <summary>
        /// Event handler for clicking the cancel button
        /// </summary>
        private void cmdCancel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _confirmationPopup = new Popup_Confirmation("Cancel?", "Are you sure you want to cancel? All unsaved changes will be lost", () =>
            {
                // cancel callback
                MainGrid.Children.Remove(_confirmationPopup);
                if (_confirmationPopup != null)
                    _confirmationPopup.Visibility = Visibility.Collapsed;
            },
            () =>
            {
                // confirm callback
                MainGrid.Children.Remove(_confirmationPopup);
                DialogResult = false;
                Close();
            });

            // show popup
            PopupController.AboveAll(_confirmationPopup);
            MainGrid.Children.Add(_confirmationPopup);
        }

        /// <summary>
        /// Event handler for the mouse button
        /// </summary>
        private void Help_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var popup = new Popup_Message("Help", "Drag the centre of each step to change the start time of the step. " +
                "Drag the left and right edges of the control to change the duration of the step.", MainGrid);
            PopupController.AboveAll(popup);
            MainGrid.Children.Add(popup);
        }

        /// <summary>
        /// Changes the maximum number of minutes
        /// </summary>
        /// <param name="offset">How much to change by</param>
        void ChangeMaxTime(int offset)
        {
            _steps.Clear();

            // store times
            foreach (DraggableObjectGantt obj in stckData.Children)
            {
                var start = StartTime_(obj);
                if (start >= (MAX_MINS * 60))
                    start = 0;

                var i = new MethodStep(obj.GetText(), start, Duration_(obj));
                _steps.Add(i);
            }

            // add time
            MAX_MINS += offset;

            SetStepSize_();

            stckData.Children.Clear();
            stckTitles.Children.Clear();

            // re-display with correct times
            foreach (var ins in _steps)
            {
                NewStep_(ins);
            }
        }

        /// <summary>
        /// Event handler for the the zoom out button
        /// </summary>
        private void Zoom_Out(object sender, MouseButtonEventArgs e)
        {
            if (MAX_MINS >= MAX_ZOOM_MINUTES) return;

            // add minutes
            ChangeMaxTime(ZOOM_CHANGE_MINUTES);
        }

        /// <summary>
        /// Event handler for the the zoom in button
        /// </summary>
        private void Zoom_In(object sender, MouseButtonEventArgs e)
        {
            if (MAX_MINS < MIN_ZOOM_MINUTES) return;

            // remove minutes
            ChangeMaxTime(-ZOOM_CHANGE_MINUTES);
        }
    }
}