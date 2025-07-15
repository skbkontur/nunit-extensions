using System;
using System.Collections.Generic;
using System.Text;

namespace SkbKontur.NUnit.Retries.CiService
{
    public static class GitlabMessageFormatter
    {
        public static string FormatMessage(string messageName, Dictionary<string, string> properties)
        {
            if (string.IsNullOrEmpty(messageName))
            {
                throw new ArgumentException("The message name must not be empty", nameof(messageName));
            }

            var sb = new StringBuilder();
            sb.Append(messageName);
            foreach (var property in properties)
            {
                if (string.IsNullOrEmpty(property.Key))
                {
                    throw new InvalidOperationException("The property name must not be empty");
                }

                sb.AppendFormat(" {0}='{1}'", property.Key, property.Value);
            }
            return sb.ToString();
        }
    }
}