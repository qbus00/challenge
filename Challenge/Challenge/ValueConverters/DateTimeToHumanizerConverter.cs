using System;
using System.Globalization;
using Humanizer;
using Xamarin.Forms;

namespace Challenge.ValueConverters
{
    public class DateTimeToHumanizerConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime dateTime)
            {
                return dateTime.Humanize(culture: CultureInfo.CurrentUICulture);
            }
            return "-";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}