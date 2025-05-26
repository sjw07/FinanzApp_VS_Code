using System.Globalization;
using Microsoft.Maui.Controls;

namespace FinanzApp.Converters;

public class AmountToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value is double d && d < 0 ? Colors.Red : Colors.Green;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
