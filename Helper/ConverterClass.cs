using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace test.Helper
{
    public class ConverterClass : IMultiValueConverter
    {
        private const double Ratio = 1.334007;

        // ViewModel -> UI
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // values[0] = W
            // values[1] = Distance1

            if (values[0] == DependencyProperty.UnsetValue ||
                values[1] == DependencyProperty.UnsetValue)
                return Binding.DoNothing;

            return values[0];
        }

        // UI -> ViewModel
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            double input = System.Convert.ToDouble(value);

            // parameter = "W" hoặc "H"
            string mode = parameter?.ToString();

            if (mode == "W")
            {
                // user sửa W → tính Distance1
                return new object[] { input, input / Ratio };
            }
            else // user sửa Distance1
            {
                return new object[] { input * Ratio, input };
            }
        }
    }
}
