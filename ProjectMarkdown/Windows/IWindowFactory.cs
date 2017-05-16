namespace ProjectMarkdown.Windows
{
    public interface IWindowFactory
    {
        void CreateWindow(ProductionWindowFactory.WindowTypes windowType);
    }
}
