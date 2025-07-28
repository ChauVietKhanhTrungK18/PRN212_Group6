using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Task_Management_System.Converters
{
    public class ReadStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isRead)
            {
                return isRead ? new SolidColorBrush(Colors.Gray) : new SolidColorBrush(Colors.Blue);
            }
            return new SolidColorBrush(Colors.Gray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 