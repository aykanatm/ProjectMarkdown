using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ProjectMarkdown.Converters
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class VisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var visibilityValue = Visibility.Visible;
            if (value != null)
            {
                var isSavedValue = (bool)value;

                if (isSavedValue)
                {
                    visibilityValue = Visibility.Hidden;
                }
            }

            return visibilityValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
