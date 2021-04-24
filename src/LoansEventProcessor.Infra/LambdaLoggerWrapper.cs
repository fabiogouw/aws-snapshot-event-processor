using Amazon.Lambda.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace LoansEventProcessor.Infra
{
    public class LambdaLoggerWrapper : ILogger
    {
        public IDisposable BeginScope<TState>(TState state) => default;

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }
            var logRecord = string.Format("{0} [{1}] {2} {3}", "[" + DateTimeOffset.UtcNow.ToString("yyyy-MM-dd HH:mm:ss+00:00") + "]",
                logLevel.ToString(),
                formatter(state, exception),
                exception != null ? exception.StackTrace : "");
            LambdaLogger.Log(logRecord);
        }
    }
}
