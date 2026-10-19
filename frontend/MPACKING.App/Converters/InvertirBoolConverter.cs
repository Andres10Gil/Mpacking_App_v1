using System.Globalization;

namespace MPACKING.App.Converters;

public class InvertirBoolConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value is bool valor && !valor;

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value is bool valor && !valor;
}
