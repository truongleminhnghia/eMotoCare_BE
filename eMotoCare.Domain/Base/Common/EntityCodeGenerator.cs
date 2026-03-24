using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace eMotoCare.Domain.Base.Common
{
    public class EntityCodeGenerator
    {
        // là lớp hỗ trợ tạo mã định danh duy nhất cho các thực thể trong hệ thống, dựa trên một chuỗi đầu vào
        // (thường là tên hoặc mô tả) và đảm bảo rằng mã được tạo ra không trùng lặp với các mã đã tồn tại trong bảng dữ liệu hiện tại.
        // Không tạo instance mà chỉ gọi trực tiếp

        // tìm tất cả ký tự không phải IN hoa hay số, thay thế bằng '-'
        private static readonly Regex NonAlphaNumericRegex = new(@"[^A-Z0-9]+", RegexOptions.Compiled);
        // nếu 2 dấu '--' thì gôm lại 1.
        private static readonly Regex MultiDashRegex = new(@"-{2,}", RegexOptions.Compiled);

        public static string Generate(
            string rawInput,
            Func<string, bool> existsInCurrentTable,
            int startNumber = 1,
            int padding = 4)
        {
            if (existsInCurrentTable is null)
                throw new ArgumentNullException(nameof(existsInCurrentTable));

            if (padding < 1)
                throw new ArgumentOutOfRangeException(nameof(padding));

            var baseCode = NormalizeToBaseCode(rawInput);
            var current = Math.Max(1, startNumber);

            while (true)
            {
                var candidate = $"{baseCode}-{current.ToString($"D{padding}")}";

                if (!existsInCurrentTable(candidate))
                    return candidate;

                current++;
            }
        }

        private static string NormalizeToBaseCode(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new ArgumentException("Input cannot be empty.", nameof(input));

            var noDiacritics = RemoveDiacritics(input).ToUpperInvariant();
            var replaced = NonAlphaNumericRegex.Replace(noDiacritics, "-");
            var compact = MultiDashRegex.Replace(replaced, "-").Trim('-');

            if (string.IsNullOrWhiteSpace(compact))
                throw new ArgumentException("Input does not contain valid characters for code generation.", nameof(input));

            return compact;
        }

        private static string RemoveDiacritics(string text)
        {
            var normalized = text.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder(normalized.Length);

            foreach (var c in normalized)
            {
                var category = CharUnicodeInfo.GetUnicodeCategory(c);
                if (category != UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }

            return sb.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
