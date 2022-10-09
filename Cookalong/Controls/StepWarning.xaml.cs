using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Cookalong.Controls
{
    /// <summary>
    /// Interaction logic for StepWarning.xaml
    /// </summary>
    public partial class StepWarning : UserControl
    {
        Timer _tmrCountdown;
        Timer _tmrAppear;
        Timer _tmrRemove;
        int _time = 0;

        const int MOVE_SPEED = 10;

        public StepWarning()
        {
            InitializeComponent();

            _tmrCountdown = new Timer();
            _tmrCountdown.Elapsed += _tmrCountdown_Elapsed;
            _tmrCountdown.Interval = SlidingTimeControl.TIME_FACTOR;

            _tmrAppear = new Timer();
            _tmrAppear.Elapsed += _tmrAppear_Elapsed;
            _tmrAppear.Interval = 1;

            _tmrRemove = new Timer();
            _tmrRemove.Elapsed += _tmrRemove_Elapsed;
            _tmrRemove.Interval = 1;
        }

        private void _tmrRemove_Elapsed(object? sender, ElapsedEventArgs e)
        {
            Debug.WriteLine("HERE???");

            Dispatcher.Invoke(() =>
            {
                Margin = new System.Windows.Thickness(Margin.Left - MOVE_SPEED, Margin.Top, Margin.Right, Margin.Bottom);

                // stop once off the side
                if (Margin.Left < -ActualWidth)
                    _tmrRemove.Stop();
            });
        }

        private void _tmrAppear_Elapsed(object? sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                Margin = new System.Windows.Thickness(Margin.Left + MOVE_SPEED, Margin.Top, Margin.Right, Margin.Bottom);

                // stop once off the side
                if (Margin.Left >= 0)
                {
                    _tmrAppear.Stop();
                    Margin = new System.Windows.Thickness(0, Margin.Top, Margin.Right, Margin.Bottom);
                }
            });
        }

        public void StartCountdown(int time)
        {
            _time = time;
            txtMessage.Text = $"Next step coming in... {_time} seconds";

            Margin = new System.Windows.Thickness(-ActualWidth, Margin.Top, Margin.Right, Margin.Bottom);

            _tmrCountdown.Start();
            _tmrRemove.Stop();
            _tmrAppear.Start();
        }

        private void _tmrCountdown_Elapsed(object? sender, ElapsedEventArgs e)
        {
            --_time;

            Dispatcher.Invoke(() =>
            {
                txtMessage.Text = $"Next step coming in... {_time} seconds";

                if (_time <= 0)
                {
                    _tmrCountdown.Stop();
                    _tmrRemove.Start();
                }
            });
        }
    }
}
