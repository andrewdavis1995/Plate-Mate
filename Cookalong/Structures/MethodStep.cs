namespace Cookalong.Structures
{
    public class MethodStep
    {
        public string Message { get; private set; }
        public int Start { get; private set; }
        public int Duration { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">The message to display</param>
        /// <param name="start">The start time</param>
        /// <param name="duration">Duration of the step</param>
        public MethodStep(string message, int start, int duration)
        {
            Message = message;
            Start = start;
            Duration = duration;
        }
    }
}
