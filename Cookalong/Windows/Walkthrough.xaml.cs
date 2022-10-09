using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using System.Windows;
using Andrew_2_0_Libraries.Models;

namespace Cookalong.Controls
{
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

        List<MethodStep> _instructions = new();

        readonly Timer _tmrTime = new();                   // controls time (ticks every 1 second)
        readonly Timer _tmrMsgAppear = new();              // controls moving items on to the screen (from left)
        readonly Timer _tmrMsgRemove = new();              // controls moving item off the screen (to top)
        readonly Timer _tmrPreviousInstructions = new();   // controls position of the previous instructions (slide items down)

        // const values
        const int WARNING_GAP = 60;
        const float MOVE_SPEED = 7.75f;
        const float GROW_SPEED = 0.01f;
        const float OPACITY_SPEED = 0.02f;
        const int RHS_HEIGHT = 65;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="recipeName">The name of the recipe</param>
        /// <param name="instructions">List of instructions to display</param>
        public Walkthrough(string recipeName, List<MethodStep> instructions)
        {
            InitializeComponent();
            SetData(instructions);
            txtTitle.Text = recipeName;

            ConfigureTimer_(ref _tmrTime, Timer_Elapsed, 1000);
            ConfigureTimer_(ref _tmrMsgAppear, MsgAppear_Elapsed, 1);
            ConfigureTimer_(ref _tmrMsgRemove, MsgRemove_Elapsed, 1);
            ConfigureTimer_(ref _tmrPreviousInstructions, TmrRHS_Elapsed, 1, true);

            stckPrevious.Margin = new Thickness(0, RHS_HEIGHT, 0, 0);

            NextStep_();
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
        /// Event handler for the timer which contrlos removing steps from the screen
        /// </summary>
        private void MsgRemove_Elapsed(object? sender, ElapsedEventArgs e)
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
                    var lbl = new PreviousStepDisplay(lblMsg.Text);
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
                    grdMessage.Margin = new Thickness(grdMessage.Margin.Left + 5, grdMessage.Margin.Top - MOVE_SPEED * 2, 0, 0);

                    // fade out
                    grdMessage.Opacity -= OPACITY_SPEED;
                }
            });
        }

        /// <summary>
        /// Event handler for the timer which controls the display of next step
        /// </summary>
        private void MsgAppear_Elapsed(object? sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                // if reached middle, stop
                if (grdMessage.Margin.Left >= (ActualWidth / 2) - ((grdMessage.ActualWidth * newInstructionScale.ScaleX) / 2))
                {
                    // show instructions
                    grdInstructions.Visibility = Visibility.Visible;

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
        private void Timer_Elapsed(object? sender, ElapsedEventArgs e)
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
                    if (_time == i.GetStart())
                    {
                        // TODO
                        ShowStep_(i.GetMethod());
                    }
                    // if the time is the end time of the structions, display
                    else if (_time == i.GetStart() + i.GetDuration())
                    {
                        // TODO
                    }
                    // if the time is the warning time of the structions, display
                    else if (_time == i.GetStart() - WARNING_GAP)
                    {
                        // TODO
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
        static void CheckItemsRequired_(MethodStep ins)
        {
            // lists - must match up
            var searchStrings = new string[] { "Teaspoon", "Oven", "Tablespoon", "Grate" };
            var objectsReqd = new string[] { "a teaspoon", "an oven", "a tablespoon", "a grater" };
            Debug.Assert(searchStrings.Length == objectsReqd.Length, "Item check arrays are of different lengths");

            // loop through each item, checking for it in the instruction string
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
        /// Sets the ordered list of instructions, and the mode to use
        /// </summary>
        /// <param name="instructions">List of instructions to display</param>
        /// <param name="mode">The mode to use for dislaying instructions</param>
        public void SetData(List<MethodStep> instructions)
        {
            _instructions = instructions.OrderBy(e => e.GetStart()).ToList();

            // opening message
            _instructions.Insert(0, new MethodStep("Let's begin!", -1, 1));

            // final message
            if (_instructions.Count > 0)
                _instructions.Add(new MethodStep("Enjoy!", _instructions.Last().GetStart() + _instructions.Last().GetDuration(), 1));
        }

        /// <summary>
        /// Event handler for when a key is pressed on this screen
        /// </summary>
        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            // Space - next step
            if (e.Key == System.Windows.Input.Key.Space)
            {
                NextStep_();
            }
            // Backspace - previous step
            else if (e.Key == System.Windows.Input.Key.Back)
            {
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
                if (stckPrevious.Children[0] is PreviousStepDisplay child)
                    ShowStep_((child).txtContent.Text);

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
                _pendingMessage = _instructions[_instructionIndex].GetMethod();

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
            foreach (PreviousStepDisplay lbl in stckPrevious.Children)
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
