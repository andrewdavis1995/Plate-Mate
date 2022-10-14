
using System.Windows.Media;

namespace Cookalong.Helpers
{
    internal static class ColourFetcher
    {
        static readonly Color[] _colours = new Color[]
        {
            Color.FromRgb(11, 147, 232),
            Color.FromRgb(232, 76, 134),
            Color.FromRgb(0, 210, 214),
            Color.FromRgb(62, 104, 221),
            Color.FromRgb(145, 23, 232),
            Color.FromRgb(232, 0, 194),
            Color.FromRgb(0, 227, 161),
            Color.FromRgb(29, 95, 232),
            Color.FromRgb(232, 40, 82)
        };

        /// <summary>
        /// Returns the colour at the specified index
        /// </summary>
        /// <param name="index">Index to get colour for</param>
        /// <returns>The colour to display</returns>
        internal static Color GetColour(int index)
        {
            return _colours[index % _colours.Length];
        }
    }
}
