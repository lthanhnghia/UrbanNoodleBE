namespace UrbanNoodle.Utils
{
    public class UtilService
    {
        public static string NormalizeText(string text)
        {
            var normalized = text.Normalize(System.Text.NormalizationForm.FormD);
            var chars = normalized
                .Where(c => System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c)
                    != System.Globalization.UnicodeCategory.NonSpacingMark)
                .ToArray();

            return new string(chars)
                .Normalize(System.Text.NormalizationForm.FormC)
                .ToLower();
        }
    }
}
