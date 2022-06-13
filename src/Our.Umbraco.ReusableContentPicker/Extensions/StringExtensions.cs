using System;
using System.Linq;
using Umbraco.Cms.Core;

namespace Our.Umbraco.ReusableContentPicker.Extensions
{
    internal static class StringExtensions
    {
        public static Udi[] ParseContentUdis(this string value)
            => value?
                   .Split(Constants.CharArrays.Comma, StringSplitOptions.RemoveEmptyEntries)
                   .Select(s => UdiParser.TryParse(s, out var udi) ? udi : null)
                   .Where(u => u != null && u.EntityType == Constants.UdiEntityType.Document)
                   .ToArray()
               ?? Array.Empty<Udi>();
    }
}
