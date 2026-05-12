using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Cinema.GUI.Converters
{
    public class ImageConverter : IValueConverter
	{
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
			if (value == null)
				return null;

			string? path = value.ToString();

			try
			{
				// 1. Nếu là pack URI (ảnh embedded trong project)
				if (path.StartsWith("pack://"))
				{
					return new BitmapImage(new Uri(path, UriKind.Absolute));
				}

				// 2. Nếu là file system path (DB lưu relative)
				string fullPath = System.IO.Path.Combine(
					AppDomain.CurrentDomain.BaseDirectory,
					path
				);

				if (!File.Exists(fullPath))
					return null;

				return new BitmapImage(new Uri(fullPath, UriKind.Absolute))
				{
					CacheOption = BitmapCacheOption.OnLoad
				};
			}
			catch
			{
				return null;
			}
		}

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
