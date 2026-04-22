using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;


namespace TP.ConcurrentProgramming.PresentationView
{
  public class BoolToVisibilityConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      bool visible = (bool)value;

      if (parameter is string p && p.Equals("Invert", StringComparison.OrdinalIgnoreCase))
        visible = !visible;

      return visible ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      bool visible = (Visibility)value == Visibility.Visible;

      if (parameter is string p && p.Equals("Invert", StringComparison.OrdinalIgnoreCase))
        visible = !visible;

      return visible;
    }

  }

}