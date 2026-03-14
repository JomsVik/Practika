using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ShoeStore.Converters
{
    public class StatusColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return new SolidColorBrush(Colors.Black);

            string status = value.ToString();

            switch (status)
            {
                case "Новый":
                    return new SolidColorBrush(Colors.Blue);
                case "В обработке":
                    return new SolidColorBrush(Colors.Orange);
                case "Завершен":
                    return new SolidColorBrush(Colors.Green);
                case "Отменен":
                    return new SolidColorBrush(Colors.Red);
                default:
                    return new SolidColorBrush(Colors.Black);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}