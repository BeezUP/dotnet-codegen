using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CodegenUP
{
    public static class StringHelpers
    {
        /// <summary>
        /// Pascal case the string
        /// </summary>
        public static string ToPascalCase(string toCase)
        {
            if (string.IsNullOrWhiteSpace(toCase))
                return "";

            var chars = toCase.Trim().ToArray();
            var seed = (upperNext: true, result: new StringBuilder());
            var aggregated = chars.Aggregate(seed, (s, c) =>
            {
                var (upperNext, sb) = s;

                if (c == ' ' || c == '_' || c == '-')
                {
                    return (upperNext: true, sb);
                }

                sb.Append(upperNext ?
                    c.ToString().ToUpperInvariant() :
                    c.ToString()
                    );

                return (false, sb);
            });

            var result = aggregated.result.ToString();
            return result;
        }

        /// <summary>
        /// Camel case the string
        /// </summary>
        public static string ToCamelCase(string toCase)
        {
            if (string.IsNullOrWhiteSpace(toCase))
                return "";

            var chars = toCase.Trim().ToArray();
            var seed = (upperNext: (bool?)false, result: new StringBuilder());
            var aggregated = chars.Aggregate(seed, (s, c) =>
            {
                var (upperNext, sb) = s;

                if (c == ' ' || c == '_' || c == '-')
                {
                    return (upperNext: true, sb);
                }

                if (upperNext.HasValue)
                {
                    sb.Append(upperNext.Value ?
                        c.ToString().ToUpperInvariant() :
                        c.ToString().ToLowerInvariant()
                        );
                }
                else
                {
                    sb.Append(c);
                }

                return (null, sb);
            });

            var result = aggregated.result.ToString();
            return result;
        }

        /// <summary>
        /// Snake case the string
        /// </summary>
        public static string ToSnakeCase(string toCase)
        {
            if (string.IsNullOrWhiteSpace(toCase))
                return "";

            var chars = toCase.Trim().ToArray();
            var seed = (previousUnderscore: true, previousUpper: true, result: new StringBuilder());
            var aggregated = chars.Aggregate(seed, (s, c) =>
            {
                var (previousUnderscore, previousUpper, sb) = s;

                if ((c == '_' || c == ' ' || c == '-'))
                {
                    if (!previousUnderscore)
                        sb.Append('_');
                    return (true, false, sb);
                }

                var isUpper = char.IsUpper(c);

                if (isUpper && !previousUnderscore && !previousUpper)
                {
                    sb.Append('_');
                }

                sb.Append(c.ToString().ToLower());

                return (false, isUpper, sb);
            });

            var result = aggregated.result.ToString();
            return result;
        }

        static readonly Regex _oneLinerRegex = new Regex(@" *[\r\n]+ *", RegexOptions.Compiled);
        /// <summary>
        /// taking out empty lines & meaningless whitespaces from the inner template
        /// </summary>
        public static string OnOneLine(string str, int? indent = null, bool? lineBreak = null)
        {
            str = _oneLinerRegex.Replace(str, "");
            str =
                new string(' ', indent ?? 0)
                + str.Trim()
                + (lineBreak ?? true ? "\n" : "")
                ;
            return str;
        }
    }
}
