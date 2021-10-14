using System;
using Xamarin.Forms;
using System.Globalization;
using System.Text.RegularExpressions;

namespace PrescriptionFiller.Helpers
{
    public class PhoneNumberConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value == null)
                return String.Empty;

            string numbers = Regex.Replace(value.ToString(), @"\D", "");
            if (numbers.Length <= 3)
                return numbers;
            
            if (numbers.Length < 7)
                return string.Format("{0}-{1}", numbers.Substring(0, 3), numbers.Substring(3));

            string number = String.Format(
                "{0}-{1}-{2}", 
                numbers.Substring(0, 3), 
                numbers.Substring(3, 3), 
                numbers.Substring(6)
            );

            return number.Length > 12 ? number.Substring(0, 12) : number;
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return value == null
                ? String.Empty
                    : value;
        }
    }
}

