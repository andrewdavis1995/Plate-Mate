using Andrew_2_0_Libraries.Models;
using Cookalong.Helpers;
using System;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Cookalong.Controls
{/// <summary>
 /// Interaction logic for SlidingTimeControl.xaml
 /// </summary>
    public partial class SlidingTimeControl : UserControl
    {
        readonly Timer _slideTimer;
        readonly MethodStep _instruction;

        readonly Action<int, bool> _durationChangedCallback;

        double _time = 0;
        static readonly int TIME_FACTOR = 1000;
        static readonly int SPEED_FACTOR = TIME_FACTOR / 50;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="instruction">The instruction to be represented by this display</param>
        /// <param name="index">The index of the step</param>
        /// <param name="durationChangedCallback">The callback to call when the duration of this instruction changes (+30 or DONE)</param>
        public SlidingTimeControl(MethodStep instruction, int index, Action<int, bool> durationChangedCallback)
        {
            InitializeComponent();

            _durationChangedCallback = durationChangedCallback;
            _instruction = instruction;

            // set colour
            Background = new SolidColorBrush(ColourFetcher.GetColour(index));

            // configure timer
            _slideTimer = new Timer();
            _slideTimer.Elapsed += SlideTimer_Elapsed;
            _slideTimer.Interval = 50;

            //display message
            lblContent.Text = instruction.GetMethod();
        }

        /// <summary>
        /// Accessor for time factor
        /// </summary>
        /// <returns></returns>
        public static int Get_TIME_FACTOR() { return TIME_FACTOR; }

        /// <summary>
        /// Callback for each "tick" of the timer
        /// </summary>
        private void SlideTimer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            ++_time;

            if (Dispatcher == null) return;

            // different thread, so need to use dispatcher
            Dispatcher.Invoke(() =>
            {
                // break time down into components
                var totalSeconds = (int)(_time / SPEED_FACTOR);
                var remaining = _instruction.GetDuration() - totalSeconds;

                // display time
                lblTime.Text = StringHelper.TimeConfigOutput(remaining, true) + " remaining";

                // set the height of the progress bar
                var jump = ActualHeight / _instruction.GetDuration();
                timeSlider.Height = jump * (_time / SPEED_FACTOR);

                // if we've reached 30 seconds, make the +30 and Done buttons visible
                if (totalSeconds == 30)
                {
                    grdButtons.Visibility = System.Windows.Visibility.Visible;
                }

                // kill timer once reached full height
                if (timeSlider.Height >= ActualHeight)
                    _slideTimer.Stop();
            });
        }
        /// <summary>
        /// Event handler for clicking the +30 button
        /// </summary>
        private void CmdThirty_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // add 30 seconds, and update the host
            _instruction.AddToDuration(30);
            _durationChangedCallback?.Invoke(_instruction.GetDuration(), false);
        }

        /// <summary>
        /// Event handler for clicking the Done button
        /// </summary>
        private void CmdDone_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // calculate the new duration (to the nearest 30s)
            var trueTime = (_time / SPEED_FACTOR);
            var nearest = ((int)Math.Round(trueTime / 30d)) * 30;

            // set duration, and update the host
            _instruction.UpdateTimes(_instruction.GetStart(), nearest);
            _durationChangedCallback?.Invoke(_instruction.GetDuration(), true);
        }

        /// <summary>
        /// Sets what state the timer should be in
        /// </summary>
        /// <param name="state">The state of the timer - TRUE = Go, FALSE = Stop</param>
        public void SetTimerState(bool state)
        {
            if (state)
                _slideTimer?.Start();
            else
                _slideTimer?.Stop();
        }
    }
}
