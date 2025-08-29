using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using ThinFilmCharacteristicMatrixCalculator.Models;

namespace ThinFilmCharacteristicMatrixCalculator.Converters
{
    /// <summary>
    /// 計算步驟狀態到圖示的轉換器
    /// </summary>
    public class StatusToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is CalculationStepStatus status)
            {
                return status switch
                {
                    CalculationStepStatus.Pending => "⏳",
                    CalculationStepStatus.Running => "🔄",
                    CalculationStepStatus.Completed => "✅",
                    CalculationStepStatus.Failed => "❌",
                    CalculationStepStatus.Warning => "⚠️",
                    _ => "❓"
                };
            }
            return "❓";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 計算步驟狀態到顏色的轉換器
    /// </summary>
    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is CalculationStepStatus status)
            {
                return status switch
                {
                    CalculationStepStatus.Pending => Brushes.Gray,
                    CalculationStepStatus.Running => Brushes.Blue,
                    CalculationStepStatus.Completed => Brushes.Green,
                    CalculationStepStatus.Failed => Brushes.Red,
                    CalculationStepStatus.Warning => Brushes.Orange,
                    _ => Brushes.Black
                };
            }
            return Brushes.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 狀態到進度條可見性的轉換器
    /// </summary>
    public class StatusToProgressVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is CalculationStepStatus status)
            {
                return status == CalculationStepStatus.Running ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 狀態到時間顯示可見性的轉換器
    /// </summary>
    public class StatusToTimeVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is CalculationStepStatus status)
            {
                return status == CalculationStepStatus.Completed || status == CalculationStepStatus.Failed 
                    ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 計算結果值的格式化轉換器
    /// </summary>
    public class CalculationResultValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value switch
            {
                Complex c => FormatComplex(c),
                double d => $"{d:F6}",
                ComplexMatrix m => $"[{m.Rows}×{m.Columns} Matrix]",
                _ => value?.ToString() ?? ""
            };
        }

        private string FormatComplex(Complex c)
        {
            if (Math.Abs(c.Imaginary) < 1e-10)
                return $"{c.Real:F4}";
            else if (c.Imaginary > 0)
                return $"{c.Real:F4} + {c.Imaginary:F4}i";
            else
                return $"{c.Real:F4} - {Math.Abs(c.Imaginary):F4}i";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}