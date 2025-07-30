using System;
using System.Collections.Generic;
using System.IO;

using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace SkbKontur.NUnit.Retries.CiService
{
    public static class TestContextExtensions
    {
        public static void WriteFailure(this TestExecutionContext context, DateTimeOffset start, int tryCount)
        {
            var props = GetTestProperties(context.CurrentTest);

            TestContext.Progress.TestStarted(props, start);
            TestContext.Progress.TestStdOut(props, context.CurrentResult.Output, context.CurrentRepeatCount + 1, tryCount);
            TestContext.Progress.TestFailed(props, context);
            TestContext.Progress.TestFinished(props);
        }

        private static void TestStarted(this TextWriter writer, IDictionary<string, string> props, DateTimeOffset start)
        {
            writer.WriteLine(GetMessage("testStarted", new Dictionary<string, string>(props)
                {
                    ["timestamp"] = $"{start:yyyy-MM-dd'T'HH:mm:ss.fff}+0000",
                }));
        }

        private static void TestStdOut(this TextWriter writer, IDictionary<string, string> props, string output, int attempt, int tryCount)
        {
            writer.WriteLine(GetMessage("testStdOut", new Dictionary<string, string>(props)
                {
                    ["timestamp"] = $"{DateTimeOffset.UtcNow:yyyy-MM-dd'T'HH:mm:ss.fff}+0000",
                    ["out"] = $"Test failed on attempt {attempt}/{tryCount}, will retry\n\n" + output,
                }));
        }

        private static void TestFailed(this TextWriter writer, IDictionary<string, string> props, TestExecutionContext context)
        {
            var message = context.CurrentResult.Message;
            var stackTrace = context.CurrentResult.StackTrace;
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

            writer.WriteLine(GetMessage("testFailed", testFailedProps));
        }

        private static void TestFinished(this TextWriter writer, IDictionary<string, string> props)
        {
            writer.WriteLine(GetMessage("testFinished", new Dictionary<string, string>(props)
                {
                    ["timestamp"] = $"{DateTimeOffset.UtcNow:yyyy-MM-dd'T'HH:mm:ss.fff}+0000",
                }));
        }

        private static Dictionary<string, string> GetTestProperties(ITest test)
        {
            var assembly = test.TypeInfo!.Assembly;
            var suiteName = assembly.GetName().Name!;

            return new Dictionary<string, string>
                {
                    ["name"] = $"{suiteName}: {test.FullName}",
                    ["captureStandardOutput"] = "false",
                    ["suiteName"] = suiteName,
                    ["testSource"] = assembly.Location,
                    ["displayName"] = test.Name,
                    ["fullyQualifiedName"] = test.FullName,
                    ["id"] = Guid.NewGuid().ToString(),
                    ["flowId"] = Guid.NewGuid().ToString(),
                };
        }

        private static string GetMessage(string messageName, Dictionary<string, string> properties)
        {
            return CiServiceExtensions.GetCurrentService() == CiServiceExtensions.CiService.TeamCity ?
                       TeamcityMessageFormatter.FormatMessage(messageName, properties) :
                       GitlabMessageFormatter.FormatMessage(messageName, properties);
        }
    }
}