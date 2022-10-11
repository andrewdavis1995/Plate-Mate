using Andrew_2_0_Libraries.Models;
using Cookalong.Controls;
using Cookalong.Controls.PopupWindows;
using Cookalong.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Media;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Cookalong.Windows
{
    /// <summary>
    /// Interaction logic for DisplayWindow.xaml
    /// </summary>
    public partial class TimedWalkthrough : Window
    {
        // status variables
        int _time = -6; // start at -6 to allow a countdown
        int _overallTime = 0;
        bool _running = true;

        List<MethodStep> _methodSteps = new();
        List<MethodStep> _activeMethodSteps = new();

        List<Border> _dots = new();

        readonly Timer _tmrTime = new();
        readonly Timer _tmrPreviousInstructions = new();   // controls position of the previous instructions (slide items down)

        Popup_Confirmation? _pausePopup;

        readonly SoundPlayer _warningSound = new();
        readonly SoundPlayer _newStepSound = new();

        // const values
        const int WARNING_GAP = 10;
        const double COLUMN_GROW_SPEED = 0.05d;
        const int RHS_HEIGHT = 65;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="MethodSteps">The steps to show</param>
        public TimedWalkthrough(List<MethodStep> MethodSteps)
        {
            InitializeComponent();
            SetData(MethodSteps);

            // configure sounds
            _warningSound = new SoundPlayer(Properties.Resources.Warning);
            _newStepSound = new SoundPlayer(Properties.Resources.NewStep);

            // configure timers
            ConfigureTimer_(ref _tmrTime, Timer_Elapsed, SlidingTimeControl.Get_TIME_FACTOR());
            ConfigureTimer_(ref _tmrPreviousInstructions, TmrRHS_Elapsed, 1, true);

            // start timer
            _tmrTime.Start();

            stckPrevious.Margin = new Thickness(0, RHS_HEIGHT, 0, 0);
        }

        /// <summary>
        /// Adds an item to the previous steps list
        /// </summary>
        /// <param name="text">The text to display</param>
        void AddToPrevious_(string text)
        {
            // add label to previous column
            var lbl = new PreviousStepDisplay(text);
            stckPrevious.Children.Insert(0, lbl);

            // update RHS display
            var top = stckPrevious.Margin.Top < 0 ? stckPrevious.Margin.Top - RHS_HEIGHT : -RHS_HEIGHT;
            stckPrevious.Margin = new Thickness(0, top, 0, 0);
            UpdateRhsOpacity_();
        }


        /// <summary>
        /// Sets the opacity of the items in the RHS to fade out older ones
        /// </summary>
        private void UpdateRhsOpacity_()
        {
            double opacity = 1;

            // loop through each item, updating the opacity
            foreach (PreviousStepDisplay lbl in stckPrevious.Children)
            {
                lbl.Opacity = opacity;
                opacity -= (RHS_HEIGHT / ActualHeight);
            }
        }

        /// <summary>
        /// Event handler for controlling the display of previous steps on the RHS
        /// </summary>
        private void TmrRHS_Elapsed(object? sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                // if not in correct position, move down
                if (stckPrevious.Margin.Top < 0 && stckPrevious.Children.Count > 0)
                {
                    stckPrevious.Margin = new Thickness(0, stckPrevious.Margin.Top + 0.8f, 0, 0);
                }
                else
                {
                    // stay at the top
                    stckPrevious.Margin = new Thickness(0, 0, 0, 0);
                }
            });
        }

        /// <summary>
        /// Configures the specified timerwith the correct values
        /// </summary>
        /// <param name="tmr">The timer to configure</param>
        /// <param name="callback">The callback function to call on each "tick"</param>
        /// <param name="interval">How many ms between each "tick"</param>
        /// <param name="start">Whether to start the timer</param>
        static void ConfigureTimer_(ref Timer tmr, ElapsedEventHandler callback, int interval, bool start = false)
        {
            // configure
            tmr = new Timer();
            tmr.Elapsed += callback;
            tmr.Interval = interval;

            // start if necessary
            if (start) tmr.Start();
        }

        /// <summary>
        /// Event handler for the TIME timer
        /// </summary>
        private void Timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            // increase time
            ++_time;

            Dispatcher.Invoke(() =>
            {

                var perc = _time / (double)_overallTime;

                if (perc >= 0)
                {
                    // display time
                    lblTime.Content = StringHelper.TimeConfigOutput(_time, true);

                    // calculate progress
                    var percWidth = grdProgressBG.ActualWidth * perc;

                    // move the progress bar
                    grdProgress.Width = percWidth;

                    // move the progress circle
                    progressCircle.Margin = new Thickness(
                        progressGrid.Margin.Left + percWidth - (progressCircle.ActualWidth / 2)
                        , 0, 0, 0);
                    progressCircle.Visibility = Visibility.Visible;
                }
                else
                {
                    // if not started, update display
                    progressCircle.Visibility = Visibility.Collapsed;
                    lblTime.Content = "00:00";
                }

                int index = 0;

                // loop through MethodSteps
                foreach (var i in _methodSteps)
                {
                    // if the time is the start time of the MethodSteps, display
                    if (_time == i.GetStart())
                    {
                        AddStep_(i, index);
                    }
                    // if the time is the end time of the MethodSteps, display
                    else if (_time == i.GetStart() + i.GetDuration())
                    {
                        // end step
                        EndStep_(i);
                    }
                    // if the time is the warning time of the MethodSteps, display
                    else if (_time == i.GetStart() - WARNING_GAP)
                    {
                        // TODO: use required items
                        _warningSound.Play();
                        CheckItemsRequired_(i);
                        WarningMessage.StartCountdown(WARNING_GAP);
                    }

                    ++index;
                }
            });
        }

        /// <summary>
        /// A step has completed and should be removed from the screen
        /// </summary>
        /// <param name="step">The step that was completed</param>
        private void EndStep_(MethodStep step)
        {
            // find the control that relates to this MethodStep
            foreach (SlidingTimeControl lbl in grdActiveSteps.Children)
            {
                if (lbl.Tag == step)
                {
                    // work out which column holds this control
                    var cd = grdActiveSteps.ColumnDefinitions[Grid.GetColumn(lbl)];

                    // add to RHS
                    AddToPrevious_(step.GetMethod());

                    // start timer to shrink said column
                    Timer tmr = new();
                    tmr.Elapsed += (obj, e) => ShrinkLane_(cd, tmr);
                    tmr.Interval = 1;
                    tmr.Start();
                }
            }
        }

        /// <summary>
        /// Called once per tick of the timer which closes the column once the step is complete
        /// </summary>
        /// <param name="cd">The column to shrink</param>
        /// <param name="tmr">The timer that is controlling this (so it can be stopped once done)</param>
        private void ShrinkLane_(ColumnDefinition cd, Timer tmr)
        {
            Dispatcher.Invoke(() =>
            {
                // if the column is still visible (i.e. width is not 0), shrink it
                if (cd.Width.Value > COLUMN_GROW_SPEED)
                    cd.Width = new GridLength(cd.Width.Value - COLUMN_GROW_SPEED, GridUnitType.Star);
                else
                {
                    // when done, set to exactly 0 and stop the timer
                    cd.Width = new GridLength(0, GridUnitType.Star);
                    tmr.Stop();

                    // check if all steps are finished
                    CheckFinished_();
                }
            });
        }

        /// <summary>
        /// Checks if all steps are completed
        /// </summary>
        private void CheckFinished_()
        {
            // complete if all items have been added to the list, and all columns are of width 0
            if ((_methodSteps.Count <= grdActiveSteps.Children.Count) && grdActiveSteps.ColumnDefinitions.All(c => c.Width.Value == 0))
            {
                // close the dialog
                Close();
            }
        }

        /// <summary>
        /// Updates the duration of an MethodStep in the list, after +30, or Done, was pressed
        /// </summary>
        /// <param name="index">The index of the MethodStep to update</param>
        /// <param name="duration">The new duration of the MethodStep</param>
        /// <param name="done">Whether this MethodStep is now complete</param>
        void UpdateDuration(int index, int duration, bool done)
        {
            // update duration
            _methodSteps[index].UpdateTimes(_methodSteps[index].GetStart(), duration);

            // close this column if appropriate
            if (done) EndStep_(_methodSteps[index]);

            // update the timing of the "Enjoy!" message at the end - needs to line up with the end of the LAST MethodStep
            var last = _methodSteps.OrderByDescending(l => l.GetStart() + l.GetDuration()).ToList()[1];
            var msg = _methodSteps.Last().GetMethod();
            _methodSteps.Remove(_methodSteps.Last());
            var endDuration = Math.Max(last.GetStart() + last.GetDuration(), _time + 2);
            _methodSteps.Add(new MethodStep(msg, endDuration, 10));

            _overallTime = _methodSteps.Max(m => m.GetStart() + m.GetDuration());
            lblTimeTotal.Content = StringHelper.TimeConfigOutput(_overallTime, true);

            ShowDots_();
        }

        /// <summary>
        /// Adds a step to the display when it reaches its start time
        /// </summary>
        /// <param name="step">The step to add</param>
        /// <param name="index">The index of the MethodStep in the list</param>
        void AddStep_(MethodStep step, int index)
        {
            // don't do this if we have already processed this control
            if (_activeMethodSteps.Any(i => i == step))
                return;

            // create control
            var lbl = new SlidingTimeControl(step, index, (a, b) => UpdateDuration(index, a, b))
            {
                Tag = step
            };

            // create a column, and assign the new column to this control
            // can't start as width = 0, as this causes issues with checking if all finished
            var cd = new ColumnDefinition() { Width = new GridLength(COLUMN_GROW_SPEED) };
            grdActiveSteps.ColumnDefinitions.Add(cd);
            Grid.SetColumn(lbl, grdActiveSteps.ColumnDefinitions.Count - 1);
            grdActiveSteps.Children.Add(lbl);

            // start the timer to grow the column
            Timer tmr = new();
            tmr.Elapsed += (obj, e) => GrowLane_(cd, tmr);
            tmr.Interval = 1;
            tmr.Start();

            // start the timer to control the progress line on the control
            lbl.SetTimerState(true);

            _activeMethodSteps.Add(step);

            // play sound
            if (_time >= 0)
                _newStepSound.Play();
        }

        /// <summary>
        /// Makes a lane grow from 0 to an equal portion of the screen
        /// </summary>
        /// <param name="cd">Column to grow</param>
        /// <param name="tmr">The timer that is controlling this (so it can be stopped when complete)</param>
        void GrowLane_(ColumnDefinition cd, Timer tmr)
        {
            Dispatcher.Invoke(() =>
            {
                // if not reached full size yet, grow
                if (cd.Width.Value < 1d)
                    cd.Width = new GridLength(cd.Width.Value + COLUMN_GROW_SPEED, GridUnitType.Star);
                else
                {
                    // once done, set absolute width, and stop growing
                    cd.Width = new GridLength(1, GridUnitType.Star);
                    tmr.Stop();
                }
            });
        }

        /// <summary>
        /// Checks which utensils/items are required for this step
        /// </summary>
        /// <param name="ins">The MethodStep to check</param>
        private static void CheckItemsRequired_(MethodStep ins)
        {
            // lists - must match up
            var searchStrings = new string[] { "Teaspoon", "Oven", "Tablespoon", "Grate" };
            var objectsReqd = new string[] { "a teaspoon", "an oven", "a tablespoon", "a grater" };
            Debug.Assert(searchStrings.Length == objectsReqd.Length, "Item check arrays are of different lengths");

            // loop through each item, checking for it in the MethodStep string
            for (var i = 0; i < searchStrings.Length && i < objectsReqd.Length; i++)
            {
                // if it is there, add a display
                if (ins.GetMethod().ToLower().Contains(searchStrings[i].ToLower()))
                {
                    // TODO: add control with image

                }
            }

            // TODO: display icon for linked ingredients

            // TODO: check measurement of linked ingredient
        }

        /// <summary>
        /// Sets the ordered list of MethodSteps, and the mode to use
        /// </summary>
        /// <param name="MethodSteps">List of MethodSteps to display</param>
        public void SetData(List<MethodStep> MethodSteps)
        {
            _methodSteps = MethodSteps.OrderBy(e => e.GetStart()).ToList();

            // opening message
            _methodSteps.Insert(0, new MethodStep("Let's begin!", -5, 5));

            // final message
            if (_methodSteps.Count > 0)
            {
                var last = _methodSteps.OrderByDescending(l => l.GetStart() + l.GetDuration()).First();
                _methodSteps.Add(new MethodStep("Enjoy!", last.GetStart() + last.GetDuration(), 10));
            }

            _overallTime = MethodSteps.Max(m => m.GetStart() + m.GetDuration());
            lblTimeTotal.Content = StringHelper.TimeConfigOutput(_overallTime, true);
        }

        /// <summary>
        /// Show the dots on the progress bar
        /// </summary>
        private void ShowDots_()
        {
            // remove current dots
            foreach (var d in _dots)
            {
                progressGrid.Children.Remove(d);
            }

            // loop through method steps
            foreach (var item in _methodSteps)
            {
                var left = (item.GetStart() / (double)_overallTime) * progressGrid.ActualWidth - 1;

                if (left > 0)
                {
                    // display a circle
                    var brd = new Border()
                    {
                        Background = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
                        Width = 2,
                        Height = 2,
                        Margin = new Thickness(left, 0, 0, 0),
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        CornerRadius = new CornerRadius(10)
                    };

                    // add the circle
                    _dots.Add(brd);
                    progressGrid.Children.Insert(1, brd);
                }
            }
        }

        /// <summary>
        /// Event handler for when a key is pressed
        /// </summary>
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                // pause or resume
                TogglePause_();
            }
        }

        /// <summary>
        /// Sets the state of the run-through - paused or not
        /// </summary>
        private void TogglePause_()
        {
            _running = !_running;

            // stop the main time control
            if (!_running)
            {
                _tmrTime.Stop();

                // create popup for confirmation
                _pausePopup = new Popup_Confirmation("Paused", "Recipe walk-through paused.",
                () => { Close(); },
                () => { TogglePause_(); },
                "Exit", "Continue"
                );

                // add control above all
                PopupController.AboveAll(_pausePopup);
                GradBorder.Children.Add(_pausePopup);
            }
            else
            {
                // start the timer
                _tmrTime.Start();

                if (_pausePopup != null)
                    GradBorder.Children.Remove(_pausePopup);
            }

            // find the control that relates to this MethodStep
            foreach (SlidingTimeControl lbl in grdActiveSteps.Children)
            {
                lbl.SetTimerState(_running);
            }
        }

        /// <summary>
        /// Event handler for when the window is closing
        /// </summary>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _tmrTime.Stop();
            _tmrPreviousInstructions.Stop();

            // stop all timers
            foreach (SlidingTimeControl lbl in grdActiveSteps.Children)
            {
                lbl.SetTimerState(false);
            }
        }

        /// <summary>
        /// Event handler for when the window loads
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ShowDots_();
        }
    }
}
