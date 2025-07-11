using System.Text;

namespace SkbKontur.NUnit.Retries.CiService
{
    public static class ServiceMessageReplacements
    {
        /// <summary>
        ///     Performs TeamCity-format escaping of a string.
        ///     https://github.com/JetBrains/TeamCity.ServiceMessages/blob/41f56446298b984719bd98b476f5b29f8aec8011/TeamCity.ServiceMessages/ServiceMessageReplacements.cs#L19
        /// </summary>
        public static string Encode(string value)
        {
            var sb = new StringBuilder(value.Length * 2);
            foreach (var ch in value)
            {
                switch (ch)
                {
                case '|':
                    sb.Append("||");
                    break; //
                case '\'':
                    sb.Append("|'");
                    break; //
                case '\n':
                    sb.Append("|n");
                    break; //
                case '\r':
                    sb.Append("|r");
                    break; //
                case '[':
                    sb.Append("|[");
                    break; //
                case ']':
                    sb.Append("|]");
                    break; //
                case '\u0085':
                    sb.Append("|x");
                    break; //\u0085 (next line)=>|x
                case '\u2028':
                    sb.Append("|l");
                    break; //\u2028 (line separator)=>|l
                case '\u2029':
                    sb.Append("|p");
                    break; //
                default:
                    if (ch > 127)
                    {
                        sb.Append($"|0x{(ulong)ch:x4}");
                    }
                    else
                    {
                        sb.Append(ch);
                    }

                    break;
                }
            }

            return sb.ToString();
        }
    }
}