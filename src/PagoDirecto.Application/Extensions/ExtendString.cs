using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagoDirecto.Application.Extensions
{
    public static class ExtendString
    {
        public static string ToTitleCase(this string? _text)
        {
            if (string.IsNullOrWhiteSpace(_text))
                return _text ?? string.Empty;

            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            TextInfo textInfo = cultureInfo.TextInfo;

            return textInfo.ToTitleCase(_text.ToLower());
        }
    }
}

