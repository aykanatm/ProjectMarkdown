using System;
using System.Globalization;
using System.Windows.Data;

namespace ProjectMarkdown.Converters
{
    [ValueConversion(typeof(bool), typeof(int))]
    public class LockTabBarConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var fixedHeaderCount = 0;

            if (value != null)
            {
                var isTabBarLocked = (bool)value;
                if (isTabBarLocked)
                {
                    fixedHeaderCount = 50000;
                }
            }

            return fixedHeaderCount;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
