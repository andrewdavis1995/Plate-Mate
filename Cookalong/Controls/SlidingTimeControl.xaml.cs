﻿using Andrew_2_0_Libraries.Models;
using Cookalong.Helpers;
using System;
using System.Timers;
using System.Windows.Controls;
using System.Windows.Media;

namespace Cookalong.Controls
{/// <summary>
 /// Interaction logic for SlidingTimeControl.xaml
 /// </summary>
    public partial class SlidingTimeControl : UserControl
    {
        Timer _slideTimer;
        MethodStep _instruction;

        Action<int, bool> _durationChangedCallback;

        double _time = 0;
        public static int TIME_FACTOR = 1000;
        public static int SPEED_FACTOR = TIME_FACTOR / 50;

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
            _slideTimer.Elapsed += _slideTimer_Elapsed;
            _slideTimer.Interval = 50;

            //display message
            lblContent.Text = instruction.GetMethod();
        }

        /// <summary>
        /// Callback for each "tick" of the timer
        /// </summary>
        private void _slideTimer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            ++_time;

            if (Dispatcher == null) return;

            // different thread, so need to use dispatcher
            Dispatcher.Invoke(() =>
            {
                // break time down into components
                var totalSeconds = (_time / SPEED_FACTOR);
                var minutes = (int)(totalSeconds / 60);
                var seconds = (int)(totalSeconds - (minutes * 60));

                // display time
                lblTime.Text = minutes.ToString("00") + ":" + seconds.ToString("00") + " remaining";

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
