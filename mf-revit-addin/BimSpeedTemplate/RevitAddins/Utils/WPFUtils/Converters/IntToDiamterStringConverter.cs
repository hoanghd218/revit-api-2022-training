using System;
using System.Globalization;
using System.Windows.Data;

namespace RevitApiUtils.WPFUtils.Converters
{
   public class IntToDiameterStringConverter : IValueConverter
   {
      public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
      {
         if (value != null)
         {
            return "Փ" + System.Convert.ToInt32(value);
         }
         return "";
      }

      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
      {
         return System.Convert.ToInt32(value);
      }
   }
}