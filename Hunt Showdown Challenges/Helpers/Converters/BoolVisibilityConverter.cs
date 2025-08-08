using System.Windows;
using System.Windows.Data;

namespace Hunt_Showdown_Challenges.Helpers.Converters
{
    [ValueConversion(typeof(bool), typeof(bool))]
    public class BoolVisibilityConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            if (value == null) { return Visibility.Collapsed; }
            if (targetType != typeof(Visibility))
                throw new InvalidOperationException("The target must be a boolean");

            if ((bool)value) { return Visibility.Visible; }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}