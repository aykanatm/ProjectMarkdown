using ProjectMarkdown.Views;

namespace ProjectMarkdown.Windows
{
    public class ProductionWindowFactory : IWindowFactory
    {
        public enum WindowTypes
        {
            About,
            Settings
        }

        public void CreateWindow(WindowTypes windowType)
        {
            if (windowType == WindowTypes.About)
            {
                var aboutWindow = new About();
                aboutWindow.Show();
            }
        }
    }
}
