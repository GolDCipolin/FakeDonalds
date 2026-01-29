using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace assignment_2425.Models
{
    public static class SaveTheme
    {
        //save theme preference
        private const string ThemePrefKey = "DarkMode";

        // This property checks if dark mode is enabled
        public static bool IsDarkModeEnabled => Preferences.Get(ThemePrefKey, false);

        // This method sets the dark mode preference
        public static void SetDarkMode(bool isDark)
        {
            Preferences.Set(ThemePrefKey, isDark);
            Application.Current.UserAppTheme = isDark ? AppTheme.Dark : AppTheme.Light;
        }

        // This method applies the theme based on the saved preference
        public static void ApplyTheme()
        {
            SetDarkMode(IsDarkModeEnabled);
        }

        // This method applies the background image based on the theme
        public static void ApplyBackground(Image backgroundImage)
        {
            bool isDark = IsDarkModeEnabled;

            Application.Current.UserAppTheme = isDark ? AppTheme.Dark : AppTheme.Light;

            backgroundImage.Source = isDark ? "background_dark.png" : "background.png";
            backgroundImage.Opacity = isDark ? 1 : 0.5;
            backgroundImage.BackgroundColor = isDark ? Colors.Black : Colors.White;
        }
    }
}
