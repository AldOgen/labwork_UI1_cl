using System;
using System.Globalization;
using System.Numerics;
using System.Text.RegularExpressions;
using System.Windows.Data;


namespace labworkUI_2 {
    [ValueConversion(typeof(double), typeof(string))]
    public class MinMaxToStr : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => $"Minimum = {(((Complex, Complex))value).Item1}\tMaximum = {(((Complex, Complex))value).Item2}";
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }
}
