using System.Globalization;
using System.Windows.Data;

namespace MetricClock
{
    public class EnumBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is string parameterString && Enum.IsDefined(value.GetType(), value))
            {
                return parameterString.Equals(value.ToString());
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is string parameterString && (bool)value)
            {
                return Enum.Parse(targetType, parameterString);
            }
            return System.Windows.Data.Binding.DoNothing;
        }
    }
}