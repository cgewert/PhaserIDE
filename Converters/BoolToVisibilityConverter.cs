using System.Globalization;

namespace PhaserIDE.Converters
{
    /// <summary>
    /// Converts a boolean value to a Visibility value.
    /// Defaults to Visible if true, Collapsed if false.
    /// </summary>
    internal class BoolToVisibilityConverter: System.Windows.Data.IValueConverter
    {
        public bool VisibilityFlag { get; set; } = true;
        public System.Windows.Visibility CollapsedValue { get; set; } = System.Windows.Visibility.Collapsed;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue == VisibilityFlag ? System.Windows.Visibility.Visible : CollapsedValue;
            }
            else
            {
                throw new ArgumentException("Value must be of type bool", nameof(value));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is System.Windows.Visibility visibility)
            {
                return visibility == System.Windows.Visibility.Visible;
            } else
            {
                throw new ArgumentException("Value must be of type Visibility", nameof(value));
            }
        }
    }
}
