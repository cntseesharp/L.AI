using Microsoft.VisualStudio.PlatformUI;
using System;
using System.Globalization;
using System.Windows;

namespace L_AI.UI.ToolWindows.Converters
{
    public class IndexToVisibilityConverter : ValueConverter<int, Visibility>
    {
        protected override Visibility Convert(int value, object parameter, CultureInfo culture)
        {
            var param = int.Parse((string)parameter);
            return value == param ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
