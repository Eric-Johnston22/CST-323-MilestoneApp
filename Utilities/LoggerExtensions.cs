using Serilog.Context;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace CST_323_MilestoneApp.Utilities
{
    public static class LoggerExtensions
    {
        public static void LogWithContext(this ILogger logger, LogLevel level, string message, Exception exception = null, [CallerMemberName] string memberName = "")
        {
            using (LogContext.PushProperty("MemberName", memberName))
            {
                if (exception == null)
                {
                    logger.Log(level, message);
                }
                else
                {
                    logger.Log(level, exception, message);
                }
            }
        }

        public static void LogInformationWithContext(this ILogger logger, string message, [CallerMemberName] string memberName = "")
        {
            logger.LogWithContext(LogLevel.Information, message, null, memberName);
        }

        public static void LogErrorWithContext(this ILogger logger, string message, Exception exception, [CallerMemberName] string memberName = "")
        {
            logger.LogWithContext(LogLevel.Error, message, exception, memberName);
        }

        public static void LogWarningWithContext(this ILogger logger, string message, [CallerMemberName] string memberName = "")
        {
            logger.LogWithContext(LogLevel.Warning, message, null, memberName);
        }

        public static IDisposable LogMethodEntry(this ILogger logger, [CallerMemberName] string memberName = "", params object[] parameters)
        {
            var paramStr = string.Join(", ", parameters.Select(p => p?.ToString() ?? "null"));
            logger.LogInformationWithContext($"Entering method: {memberName}() with parameters: {paramStr}", memberName);
            return new MethodLogger(logger, memberName);
        }

        private class MethodLogger : IDisposable
        {
            private readonly ILogger _logger;
            private readonly string _memberName;
            private readonly Stopwatch _stopwatch;

            public MethodLogger(ILogger logger, string memberName)
            {
                _logger = logger;
                _memberName = memberName;
                _stopwatch = Stopwatch.StartNew();
            }

            public void Dispose()
            {
                _stopwatch.Stop();
                _logger.LogInformationWithContext($"Exiting method: {_memberName}. Duration: {_stopwatch.ElapsedMilliseconds}ms", _memberName);
            }
        }
    }
}
