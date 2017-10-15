using System;
using System.Windows;
using LogUtils;

namespace ProjectMarkdown.Services
{
    public static class ThemeSetter
    {
        public static void Set(string primaryColor, string accentColor)
        {
            Logger.GetInstance().Debug("Set() >>");
            try
            {
                Application.Current.Resources.MergedDictionaries.Clear();
                Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary()
                {
                    Source = new Uri("pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.Light.xaml")
                });
                Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary()
                {
                    Source = new Uri("pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.Defaults.xaml")
                });
                Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary()
                {
                    Source = new Uri("pack://application:,,,/MaterialDesignColors;component/Themes/Recommended/Primary/MaterialDesignColor." + primaryColor + ".xaml")
                });

                Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary()
                {
                    Source = new Uri("pack://application:,,,/MaterialDesignColors;component/Themes/Recommended/Accent/MaterialDesignColor." + accentColor + ".xaml")
                });
            }
            catch (Exception e)
            {
                throw e;
            }
            Logger.GetInstance().Debug("<< Set()");
        }
    }
}
