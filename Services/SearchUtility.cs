using System;
using System.Globalization;
using System.Linq;
using System.Text;

namespace summeringsmakker.Services
{
    public class SearchUtility
    {
        
        public static string NormalizeString(string input)
        {
            var withoutPunctuationOrSpaces = new string(input
            .Where(c => !char.IsPunctuation(c) && !char.IsWhiteSpace(c))
            .ToArray());

            var normalizedString = withoutPunctuationOrSpaces.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }
            return stringBuilder.ToString().Normalize(NormalizationForm.FormC).ToLowerInvariant();
        }
    }
}
