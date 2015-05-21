using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Telerik.Core
{
    public class StringToUpperConverter : IValueConverter
    {       
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var text = value as string;
            if (text != null)
            {
                return text.ToUpper();
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
