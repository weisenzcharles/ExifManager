using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace MagicFile.Converters
{
    internal class LocalizationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var localizationKey = value?.GetType().GetCustomAttributes(typeof(LocalizationKeyAttribute), true)
                .FirstOrDefault();
            return localizationKey == null
                ? value?.GetType().Name
                : StringTable.SharedStrings[(localizationKey as LocalizationKeyAttribute)?.LocalizationKey];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
