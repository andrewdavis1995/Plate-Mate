using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Cookalong.Controls
{
    public enum PlaybackMode { ClickThrough, Timing }

    /// <summary>
    /// Interaction logic for Walkthrough.xaml
    /// </summary>
    public partial class Walkthrough : Window
    {
        // status variables
        int _time = -1;
        int _instructionIndex = 0;
        bool _moving = false;
        string _pendingMessage = "";
        PlaybackMode _mode = PlaybackMode.ClickThrough;

        List<MethodStep> _instructions = new List<MethodStep>();

        Timer _tmrTime = new Timer();                   // controls tmie (ticks every 1 second)
        Timer _tmrMsgAppear = new Timer();              // controls moving items on to the screen (from left)
        Timer _tmrMsgRemove = new Timer();              // controls moving item off the screen (to top)
        Timer _tmrPreviousInstructions = new Timer();   // controls position of the previous instructions (slide items down)

        // const values
        const int WARNING_GAP = 60;
        const float MOVE_SPEED = 7.75f;
        const float GROW_SPEED = 0.01f;
        const float OPACITY_SPEED = 0.02f;
        const int RHS_HEIGHT = 30;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="recipeName">The name of the recipe</param>
        /// <param name="instructions">List of instructions to display</param>
        /// <param name="mode">The mode to use</param>
        public Walkthrough(string recipeName, List<MethodStep> instructions, PlaybackMode mode)
        {
            InitializeComponent();
            SetData(instructions, mode);
            txtTitle.Text = recipeName;

            ConfigureTimer_(ref _tmrTime, _timer_Elapsed, 1000);
            ConfigureTimer_(ref _tmrMsgAppear, _msgAppear_Elapsed, 1);
            ConfigureTimer_(ref _tmrMsgRemove, _msgRemove_Elapsed, 1);
            ConfigureTimer_(ref _tmrPreviousInstructions, _tmrRHS_Elapsed, 1, true);

            stckPrevious.Margin = new Thickness(0, RHS_HEIGHT, 0, 0);

            // start timer if necessary
            if (_mode == PlaybackMode.Timing)
                _tmrTime.Start();
            else
                NextStep_();
        }

        /// <summary>
        /// Configures the specified timerwith the correct values
        /// </summary>
        /// <param name="tmr">The timer to configure</param>
        /// <param name="callback">The callback function to call on each "tick"</param>
        /// <param name="interval">How many ms between each "tick"</param>
        /// <param name="start">Whether to start the timer</param>
        void ConfigureTimer_(ref Timer tmr, ElapsedEventHandler callback, int interval, bool start = false)
        {
            // configure
            tmr = new Timer();
            tmr.Elapsed += callback;
            tmr.Interval = interval;

            // start if necessary
            if (start) tmr.Start();
        }

        /// <summary>
        /// Event handler for controlling the display of previous steps on the RHS
        /// </summary>
        private void _tmrRHS_Elapsed(object? sender, ElapsedEventArgs e)
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
        /// Event handler for the timer which contrlos removing steps from the screen
        /// </summary>
        private void _msgRemove_Elapsed(object? sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                // if reached the top, stop
                if (grdMessage.Margin.Top <= -(grdMessage.ActualHeight))
                {
                    // stop movement
                    _moving = false;
                    _tmrMsgRemove.Stop();

                    // add label to previous column
                    var lbl = new Label() { Content = lblMsg.Text, Height = RHS_HEIGHT };
                    lbl.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                    stckPrevious.Children.Insert(0, lbl);

                    // update RHS display
                    stckPrevious.Margin = new Thickness(0, -RHS_HEIGHT, 0, 0);
                    UpdateRhsOpacity_();

                    // show pending step
                    ShowStep_(_pendingMessage);
                }
                else
                {
                    // shrink smaller
                    newInstructionScale.ScaleX -= GROW_SPEED;
                    newInstructionScale.ScaleY -= GROW_SPEED;

                    // move off
                    grdMessage.Margin = new Thickness(grdMessage.Margin.Left + 1, grdMessage.Margin.Top - MOVE_SPEED * 2, 0, 0);

                    // fade out
                    grdMessage.Opacity -= OPACITY_SPEED;
                }
            });
        }

        /// <summary>
        /// Event handler for the timer which controls the display of next step
        /// </summary>
        private void _msgAppear_Elapsed(object? sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                // if reached middle, stop
                if (grdMessage.Margin.Left >= (ActualWidth / 2) - ((grdMessage.ActualWidth * newInstructionScale.ScaleX) / 2))
                {
                    // show instructions
                    grdInstructions.Visibility = _mode == PlaybackMode.ClickThrough ? Visibility.Visible : Visibility.Collapsed;

                    // stop movement
                    grdMessage.Margin = new Thickness((ActualWidth / 2) - ((grdMessage.ActualWidth * newInstructionScale.ScaleX) / 2), (ActualHeight / 2) - (grdMessage.ActualHeight / 2) * newInstructionScale.ScaleY, 0, 0);
                    _moving = false;
                    _tmrMsgAppear.Stop();
                }
                else
                {
                    // grow bigger
                    newInstructionScale.ScaleX += GROW_SPEED;
                    newInstructionScale.ScaleY += GROW_SPEED;

                    // move on
                    grdMessage.Margin = new Thickness(grdMessage.Margin.Left + MOVE_SPEED, (ActualHeight / 2) - (grdMessage.ActualHeight / 2) * newInstructionScale.ScaleY, 0, 0);

                    // fade in 
                    grdMessage.Opacity += OPACITY_SPEED;
                }
            });
        }

        /// <summary>
        /// Event handler for the TIME timer
        /// </summary>
        private void _timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            // increase time
            ++_time;

            Dispatcher.Invoke(() =>
            {
                // display total time
                // TODO: format as proper time
                lblTime.Content = _time.ToString();

                // loop through instructions
                foreach (var i in _instructions)
                {
                    // if the time is the start time of the structions, display
                    if (_time == i.Start)
                    {
                        var lbl = new Label();
                        lbl.Content = i.Message + " [Starts]";
                        stckUpdates.Children.Add(lbl);
                        ShowStep_(i.Message);
                    }
                    // if the time is the end time of the structions, display
                    else if (_time == i.Start + i.Duration)
                    {
                        var lbl = new Label();
                        lbl.Content = i.Message + " [Ends]";
                        stckUpdates.Children.Add(lbl);
                    }
                    // if the time is the warning time of the structions, display
                    else if (_time == i.Start - WARNING_GAP)
                    {
                        var lbl = new Label();
                        lbl.Content = i.Message + " [Warning]";
                        stckUpdates.Children.Add(lbl);

                        CheckItemsRequired_(i);
                    }
                }
            });
        }

        /// <summary>
        /// Starts the process of displaying a step
        /// </summary>
        /// <param name="theMessage">Message to display</param>
        private void ShowStep_(string theMessage)
        {
            // set text
            lblMsg.Text = theMessage;

            // set to correct size and position
            grdMessage.Margin = new Thickness(-200, 0, 0, 0);
            newInstructionScale.ScaleX = 1;
            newInstructionScale.ScaleY = 1;

            // invisible to start with
            grdMessage.Opacity = 0;

            // hide instructions
            grdInstructions.Visibility = Visibility.Collapsed;

            // start movement
            _moving = true;
            _tmrMsgAppear.Start();
        }

        /// <summary>
        /// Checks which utensils/items are required for this step
        /// </summary>
        /// <param name="ins">The instruction to check</param>
        private void CheckItemsRequired_(MethodStep ins)
        {
            // lists - must match up
            var searchStrings = new string[] { "Teaspoon", "Oven", "Tablespoon", "Grate" };
            var objectsReqd = new string[] { "a teaspoon", "an oven", "a tablespoon", "a grater" };
            Debug.Assert(searchStrings.Length == objectsReqd.Length, "Item check arrays are of different lengths");

            // loop through each item, checking for it in the instruction string
            for (var i = 0; i < searchStrings.Length && i < objectsReqd.Length; i++)
            {
                // if it is there, add a display
                if (ins.Message.ToLower().Contains(searchStrings[i].ToLower()))
                {
                    // TODO: add control with image
                    stckUpdates.Children.Add(new Label() { Content = " you will need " + objectsReqd[i], Margin = new Thickness(5, 0, 5, 0) });
                }
            }

            // TODO: display icon for linked ingredients

            // TODO: check measurement of linked ingredient
        }

        /// <summary>
        /// Sets the ordered list of instructions, and the mode to use
        /// </summary>
        /// <param name="instructions">List of instructions to display</param>
        /// <param name="mode">The mode to use for dislaying instructions</param>
        public void SetData(List<MethodStep> instructions, PlaybackMode mode)
        {
            _instructions = instructions.OrderBy(e => e.Start).ToList();

            // opening message
            _instructions.Insert(0, new MethodStep("Let's begin!", -1, 1));

            // final message
            if (_instructions.Count > 0)
                _instructions.Add(new MethodStep("Enjoy!", _instructions.Last().Start + _instructions.Last().Duration, 1));
            _mode = mode;
        }

        /// <summary>
        /// Event handler for when a key is pressed on this screen
        /// </summary>
        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            // Space - next step
            if (e.Key == System.Windows.Input.Key.Space)
            {
                // only do this if we are doing a click through
                if (_mode != PlaybackMode.ClickThrough) return;

                NextStep_();
            }
            // Backspace - previous step
            else if (e.Key == System.Windows.Input.Key.Back)
            {
                // only do this if we are doing a click through
                if (_mode != PlaybackMode.ClickThrough) return;

                PreviousStep_();
            }
            // Escape - quit
            else if (e.Key == System.Windows.Input.Key.Escape)
            {
                Close();
            }
        }

        /// <summary>
        /// Goes back to the previous step
        /// </summary>
        private void PreviousStep_()
        {
            // don't do anything if already moving
            if (_moving) return;

            // only do this if there have been previous steps
            if (stckPrevious.Children.Count > 0)
            {
                // previous step
                _instructionIndex--;

                // show the previous one
                ShowStep_((stckPrevious.Children[0] as Label).Content.ToString());

                // remove from RHS
                stckPrevious.Children.RemoveAt(0);
                UpdateRhsOpacity_();
            }
        }

        /// <summary>
        /// Move to the next step in the list
        /// </summary>
        private void NextStep_()
        {
            // don't do anything if already moving
            if (_moving) return;

            // check if we've reached the end
            if (_instructionIndex < _instructions.Count)
            {
                // display next step (after removing if necessary)
                _pendingMessage = _instructions[_instructionIndex].Message;

                if (_instructionIndex == 0)
                    ShowStep_(_pendingMessage);
                else
                    _tmrMsgRemove.Start();

                // point to next one
                ++_instructionIndex;
            }
            else
            {
                // TODO: remove this control from the parent
                Close();
            }
        }

        /// <summary>
        /// Sets the opacity of the items in the RHS to fade out older ones
        /// </summary>
        private void UpdateRhsOpacity_()
        {
            double opacity = 1;

            // loop through each item, updating the opacity
            foreach (Label lbl in stckPrevious.Children)
            {
                lbl.Opacity = opacity;
                opacity -= (RHS_HEIGHT / ActualHeight);
            }
        }

        /// <summary>
        /// Event handler for when the window closes
        /// </summary>
        private void Window_Closed(object sender, EventArgs e)
        {
            // kill all timers
            _tmrPreviousInstructions.Stop();
            _tmrMsgRemove.Stop();
            _tmrTime.Stop();
            _tmrMsgAppear.Stop();
        }
    }
}
