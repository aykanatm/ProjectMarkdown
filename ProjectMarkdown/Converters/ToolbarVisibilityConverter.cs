using System;
using System.Globalization;
using System.Windows.Data;
using ProjectMarkdown.Windows;

namespace ProjectMarkdown.Converters
{
    [ValueConversion(typeof(bool), typeof(int))]
    public class ToolbarVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var toolbarHeight = 0;

            if (WindowManager.GetInstance().IsSingleBand)
            {
                toolbarHeight = WindowManager.GetInstance().ToolbarTrayHeightSingleBand;
            }
            else
            {
                toolbarHeight = WindowManager.GetInstance().ToolbarTrayHeightDoubleBands;
            }
            

            if (value != null)
            {
                var isToolbarHidden = (bool) value;
                if (isToolbarHidden)
                {
                    toolbarHeight = 0;
                }
            }

            return toolbarHeight;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
