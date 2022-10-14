using Andrew_2_0_Libraries.Models;
using System;
using System.Collections.Generic;
using System.Timers;
using System.Windows;
using System.Windows.Controls;

namespace Cookalong.Controls
{
    /// <summary>
    /// Interaction logic for StepWarning.xaml
    /// </summary>
    public partial class StepWarning : UserControl
    {
        Timer _tmrCountdown = new();
        Timer _tmrAppear = new();
        Timer _tmrRemove = new();
        int _time = 0;
        Thickness _margin = new(0, 0, 0, 0);

        const int MOVE_SPEED = 10;

        List<MethodStep> _methods = new();
        Action<MethodStep> _addThirtyCallback;

        /// <summary>
        /// Contructor
        /// </summary>
        public StepWarning()
        {
            InitializeComponent();

            // initialise timers
            ConfigureTimer_(ref _tmrCountdown, TmrCountdown_Elapsed, SlidingTimeControl.Get_TIME_FACTOR());
            ConfigureTimer_(ref _tmrAppear, TmrAppear_Elapsed, 1);
            ConfigureTimer_(ref _tmrRemove, TmrRemove_Elapsed, 1);
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
        /// Event handler for each "tick" of the remove timer
        /// </summary>
        private void TmrRemove_Elapsed(object? sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                _margin.Left -= MOVE_SPEED;
                Margin = _margin;

                // stop once off the side
                if (Margin.Left < -ActualWidth)
                    _tmrRemove.Stop();
            });
        }

        /// <summary>
        /// Event handler for each "tick" of the appear timer
        /// </summary>
        private void TmrAppear_Elapsed(object? sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                _margin.Left += MOVE_SPEED;
                Margin = _margin;

                // stop once fully on
                if (Margin.Left >= 0)
                {
                    _tmrAppear.Stop();
                    _margin.Left = 0;
                    Margin = _margin;
                }
            });
        }

        /// <summary>
        /// Starts the countdown from the specified time
        /// </summary>
        /// <param name="time">Time to start countdown from</param>
        /// <param name="addThirtyCallback">Callback when the +30 button is pressed</param>
        public void StartCountdown(int time, Action<MethodStep> addThirtyCallback)
        {
            _time = time;
            _addThirtyCallback = addThirtyCallback;

            txtMessage.Text = $"Next step coming in... {_time} seconds";

            // update margin
            _margin = Margin;
            _margin.Left = -ActualWidth;

            // make sure the control starts off screen
            Margin = _margin;

            // start countdown
            _tmrCountdown.Start();
            _tmrRemove.Stop();
            _tmrAppear.Start();
        }

        /// <summary>
        /// Event handler for each "tick" of the countdown timer
        /// </summary>
        private void TmrCountdown_Elapsed(object? sender, ElapsedEventArgs e)
        {
            --_time;

            Dispatcher.Invoke(() =>
            {
                // display remaining time
                txtMessage.Text = $"Next step coming in... {_time} seconds";

                if (_time <= 0)
                {
                    _methods.Clear();
                    _tmrCountdown.Stop();
                    _tmrRemove.Start();
                }
            });
        }

        /// <summary>
        /// Links a method to this step
        /// </summary>
        /// <param name="step">The step to add</param>
        public void AddMethod(MethodStep step)
        {
            _methods.Add(step);
        }

        /// <summary>
        /// Event handler for the +30 button
        /// </summary>
        private void CmdThirty_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // perform the callback for each method
            foreach (var method in _methods)
                _addThirtyCallback(method);

            // move off screen
            _tmrCountdown.Stop();
            _tmrRemove.Start();

            // no more methods associated with this control
            _methods.Clear();
        }
    }
}
