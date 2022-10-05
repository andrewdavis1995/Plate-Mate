using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Markup;
using System.Windows.Media;

namespace Cookalong.Helpers
{
    public class FontExplorer : MarkupExtension
    {
        // ##############################################################################################################################
        // Properties
        // ##############################################################################################################################

        #region Properties

        // ##########################################################################################
        // Public Properties
        // ##########################################################################################

        public string Key { get; set; }

        // ##########################################################################################
        // Private Properties
        // ##########################################################################################

        private static readonly Dictionary<string, FontFamily> _CachedFonts = new Dictionary<string, FontFamily>();

        #endregion


        // ##############################################################################################################################
        // Constructor
        // ##############################################################################################################################

        #region Constructor

        static FontExplorer()
        {
            foreach (FontFamily fontFamily in Fonts.GetFontFamilies(new Uri("pack://application:,,,/"), "./Fonts/"))
            {
                _CachedFonts.Add(fontFamily.FamilyNames.First().Value, fontFamily);
            }
        }

        #endregion

        // ##############################################################################################################################
        // methods
        // ##############################################################################################################################

        #region methods

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return ReadFont();
        }

        private object ReadFont()
        {
            if (!string.IsNullOrEmpty(Key))
            {
                if (_CachedFonts.ContainsKey(Key))
                    return _CachedFonts[Key];
            }

            return new FontFamily("Comic Sans MS");
        }

        #endregion
    }
}
