using System;
using Microsoft.Extensions.Logging;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using Object = UnityEngine.Object;

public static class LoggerExtensions
{
    public static void LogDebug(this ILogger logger, Object obj, string message, params object[] args)
    {
        Log(logger, obj, () => logger.LogDebug(message, args));
    }

    public static void LogTrace(this ILogger logger, Object obj, string message, params object[] args)
    {
        Log(logger, obj, () => logger.LogTrace(message, args));
    }

    public static void LogInformation(this ILogger logger, Object obj, string message, params object[] args)
    {
        Log(logger, obj, () => logger.LogInformation(message, args));
    }

    public static void LogWarning(this ILogger logger, Object obj, string message, params object[] args)
    {
        Log(logger, obj, () => logger.LogWarning(message, args));
    }

    public static void LogError(this ILogger logger, Object obj, string message, params object[] args)
    {
        Log(logger, obj, () => logger.LogError(message, args));
    }

    public static void LogCritical(this ILogger logger, Object obj, string message, params object[] args)
    {
        Log(logger, obj, () => logger.LogCritical(message, args));
    }

    public static IDisposable BeginScope(this ILogger logger, string name, Object obj)
    {
        return logger.BeginScope((name, (object) obj));
    }

    public static IDisposable BeginUnityObject(this ILogger logger, Object obj)
    {
        return logger.BeginScope(("@__Object__", (object) obj));
    }

    static void Log(ILogger logger, Object obj, Action log)
    {
        using (logger.BeginUnityObject(obj))
        {
            log();
        }
    }

    public static ILogger CreateLogger(this ILoggerFactory factory, Object obj)
    {
        var logger = factory.CreateLogger(obj.name);
        // TODO: dispose?
        logger.BeginUnityObject(obj);
        return logger;
    }
}