using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;

namespace Cookalong.Controls
{/// <summary>
 /// Interaction logic for ErrorMessage.xaml
 /// </summary>
    public partial class ErrorMessage : UserControl
    {
        public enum ErrorState { Inactive, MovingOn, Showing, MovingOff }

        ErrorState _state = ErrorState.Inactive;
        Timer _movementTimer;
        Task ? _waitTask;
        uint _errorIndex = 0;

        const int MOVEMENT_SPEED = 10;
        const int BOTTOM_MARGIN = 20;
        const int VISIBILITY_DURATION = 3;

        /// <summary>
        /// Constructor
        /// </summary>
        public ErrorMessage()
        {
            InitializeComponent();

            // configure timer
            _movementTimer = new Timer();
            _movementTimer.Interval = 1;
            _movementTimer.Elapsed += _movementTimer_Elapsed;
        }

        /// <summary>
        /// Event handler for every time the timer ticks
        /// </summary>
        private void _movementTimer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            switch (_state)
            {
                case ErrorState.MovingOn:
                {
                    // control is moving in from the side
                    Dispatcher.Invoke(() =>
                    {
                        // update margin
                        Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right + MOVEMENT_SPEED, Margin.Bottom);
                        if (Margin.Right >= 0)
                        {
                            // if reached end, wait
                            Margin = new Thickness(Margin.Left, Margin.Top, 0, Margin.Bottom);
                            _waitTask = Task.Run(Wait_);
                        }
                    });
                }
                break;
                case ErrorState.MovingOff:
                {
                    // control is moving out to the side
                    Dispatcher.Invoke(() =>
                    {
                        // update margin
                        Margin = new Thickness(Margin.Left, Margin.Top, Margin.Right - MOVEMENT_SPEED, Margin.Bottom);
                        if (Margin.Right <= -Width - 1)
                        {
                            // if reached end, done
                            Margin = new Thickness(Margin.Left, Margin.Top, Width - 1, Margin.Bottom);
                            Done_();
                        }
                    });
                }
                break;
                default:
                    // do nothing
                    break;
            }
        }

        /// <summary>
        /// Triggers the control to enter the screen
        /// </summary>
        /// <param name="msg">The message to display</param>
        public void MoveOn(string msg)
        {
            ++_errorIndex;

            // kill existing elements
            _movementTimer.Stop();
            if (_waitTask != null && !_waitTask.IsCompleted)
                _waitTask.Dispose();

            _state = ErrorState.MovingOn;
            txtMessage.Dispatcher.Invoke(() =>
            {
                // set control appearance
                Margin = new Thickness(0, 0, -Width - 1, BOTTOM_MARGIN);
                txtMessage.Text = msg;
                Visibility = Visibility.Visible;
            });
            _movementTimer.Start();
        }

        /// <summary>
        /// Waits a few seconds before moving off the screen again
        /// </summary>
        async void Wait_()
        {
            var activeIndex = _errorIndex;

            _movementTimer.Stop();
            _state = ErrorState.Showing;

            // wait
            await Task.Delay(VISIBILITY_DURATION * 1000);

            // moved on to another error - don't move off
            if (_errorIndex != activeIndex) return;

            // begin to move off
            MoveOff_();
        }

        /// <summary>
        /// Moves the control off the screen
        /// </summary>
        void MoveOff_()
        {
            // move off the screen
            _state = ErrorState.MovingOff;
            _movementTimer.Start();
        }

        /// <summary>
        /// The control is done
        /// </summary>
        void Done_()
        {
            // set control appearance
            txtMessage.Dispatcher.Invoke(() =>
            {
                Visibility = Visibility.Collapsed;
            });

            // stop the timer
            _movementTimer.Stop();
            _state = ErrorState.Inactive;
        }
    }
}
