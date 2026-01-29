using assignment_2425.Models;

namespace assignment_2425
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            SaveTheme.ApplyTheme();
            Preferences.Remove("DarkMode");
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = new Window(new AppShell())
            {
                Title = "Assignment 2425"
            };

            return window;
        }
    }
}
