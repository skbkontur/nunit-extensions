using System;
using System.Collections.Generic;
using System.IO;

using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace SkbKontur.NUnit.Retries.TeamCity
{
    public static class TestContextExtensions
    {
        public static bool IsOnTeamCity()
        {
            return !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("TEAMCITY_VERSION"));
        }

        public static void WriteFailureForTeamCity(this TestExecutionContext context, DateTimeOffset start, int tryCount)
        {
            var props = GetTestProperties(context.CurrentTest);

            TestContext.Progress.TestStarted(props, start);
            TestContext.Progress.TestStdOut(props, context.CurrentResult.Output, context.CurrentRepeatCount + 1, tryCount);
            TestContext.Progress.TestFailed(props, context.CurrentResult.Message, context.CurrentResult.StackTrace);
            TestContext.Progress.TestFinished(props);
        }

        private static void TestStarted(this TextWriter writer, IDictionary<string, string> props, DateTimeOffset start)
        {
            writer.WriteLine(ServiceMessageFormatter.FormatMessage("testStarted", new Dictionary<string, string>(props)
                {
                    ["timestamp"] = $"{start:yyyy-MM-dd'T'HH:mm:ss.fff}+0000",
                }));
        }

        private static void TestStdOut(this TextWriter writer, IDictionary<string, string> props, string output, int attempt, int tryCount)
        {
            writer.WriteLine(ServiceMessageFormatter.FormatMessage("testStdOut", new Dictionary<string, string>(props)
                {
                    ["timestamp"] = $"{DateTimeOffset.UtcNow:yyyy-MM-dd'T'HH:mm:ss.fff}+0000",
                    ["out"] = $"Test failed on attempt {attempt}/{tryCount}, will retry\n\n" + output,
                }));
        }

        private static void TestFailed(this TextWriter writer, IDictionary<string, string> props, string? message, string? stackTrace)
        {
            var testFailedProps = new Dictionary<string, string>(props)
                {
                    ["timestamp"] = $"{DateTimeOffset.UtcNow:yyyy-MM-dd'T'HH:mm:ss.fff}+0000",
                };

            if (message != null)
            {
                testFailedProps["message"] = message;
            }

            if (stackTrace != null)
            {
                testFailedProps["details"] = stackTrace;
            }

            writer.WriteLine(ServiceMessageFormatter.FormatMessage("testFailed", testFailedProps));
        }

        private static void TestFinished(this TextWriter writer, IDictionary<string, string> props)
        {
            writer.WriteLine(ServiceMessageFormatter.FormatMessage("testFinished", new Dictionary<string, string>(props)
                {
                    ["timestamp"] = $"{DateTimeOffset.UtcNow:yyyy-MM-dd'T'HH:mm:ss.fff}+0000",
                }));
        }

        private static Dictionary<string, string> GetTestProperties(ITest test)
        {
            var assembly = test.TypeInfo!.Assembly;
            var suiteName = test.TypeInfo.Assembly.GetName().Name;
            var testSource = assembly.Location;
            var testId = Guid.NewGuid().ToString();

            return new Dictionary<string, string>
                {
                    ["name"] = $"{suiteName}: {test.FullName}",
                    ["captureStandardOutput"] = "false",
                    ["suiteName"] = suiteName,
                    ["testSource"] = testSource,
                    ["displayName"] = test.Name,
                    ["fullyQualifiedName"] = test.FullName,
                    ["id"] = testId,
                };
        }
    }
}