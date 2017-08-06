using ProjectMarkdown.Views;

namespace ProjectMarkdown.Windows
{
    public class ProductionWindowFactory : IWindowFactory
    {
        public enum WindowTypes
        {
            About,
            Preferences
        }

        public void CreateWindow(WindowTypes windowType)
        {
            if (windowType == WindowTypes.About)
            {
                var aboutWindow = new About();
                aboutWindow.Show();
            }
            else if (windowType == WindowTypes.Preferences)
            {
                var preferencesWindow = new Preferences();
                preferencesWindow.Show();
            }
        }
    }
}
