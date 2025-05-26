using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace FinanceApp.Converters;

public class AmountToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value is double d && d < 0 ? Brushes.Red : Brushes.Green;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
