using System;
using System.Collections.Generic;
using System.Text;

namespace SkbKontur.NUnit.Retries.Gitlab
{
    public static class ServiceMessageFormatter
    {
        /// <summary>
        ///     Serializes single value service message
        ///     https://github.com/JetBrains/TeamCity.ServiceMessages/blob/41f56446298b984719bd98b476f5b29f8aec8011/TeamCity.ServiceMessages/Write/ServiceMessageFormatter.cs#L102
        /// </summary>
        /// <param name="messageName">message name</param>
        /// <param name="properties">params of service message properties</param>
        /// <returns>service message string</returns>
        public static string FormatMessage(string messageName, Dictionary<string, string> properties)
        {
            if (string.IsNullOrEmpty(messageName))
            {
                throw new ArgumentException("The message name must not be empty", nameof(messageName));
            }

            if (ServiceMessageReplacements.Encode(messageName) != messageName)
            {
                throw new ArgumentException("Message name contains illegal characters", nameof(messageName));
            }

            var sb = new StringBuilder();
            sb.Append(ServiceMessageConstants.ServiceMessageOpen);
            sb.Append(messageName);

            foreach (var property in properties)
            {
                if (string.IsNullOrEmpty(property.Key))
                {
                    throw new InvalidOperationException("The property name must not be empty");
                }

                if (ServiceMessageReplacements.Encode(property.Key) != property.Key)
                {
                    throw new InvalidOperationException($"The property name {property.Key} contains illegal characters");
                }

                sb.AppendFormat(" {0}='{1}'", property.Key, ServiceMessageReplacements.Encode(property.Value));
            }

            sb.Append(ServiceMessageConstants.ServiceMessageClose);
            return sb.ToString();
        }
    }
}
