using System;
using System.Globalization;
using System.Windows.Data;

namespace ProjectMarkdown.Converters
{
    [ValueConversion(typeof(bool), typeof(int))]
    public class ToolbarVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var toolbarHeight = 70;

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
