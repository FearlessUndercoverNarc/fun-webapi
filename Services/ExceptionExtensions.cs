using System;

namespace Services
{
    public static class ExceptionExtensions
    {
        public static string ToPrettyString(this Exception exception)
        {
            if (exception is null)
            {
                return "Null exception";
            }

            var message = exception.Message;
            if (message.Length > 4096)
            {
                message = message[..4096];
            }

            var stacktrace = exception.StackTrace ?? "Stack trace empty";

            var result = $"Exception:```\n{message}\n{stacktrace}\n```";
            if (result.Length > 2048)
            {
                stacktrace = stacktrace[..^(result.Length - 2048)];
                result = $"Exception:```\n{message}\n{stacktrace}\n```";
            }

            return result;
        }
    }
}