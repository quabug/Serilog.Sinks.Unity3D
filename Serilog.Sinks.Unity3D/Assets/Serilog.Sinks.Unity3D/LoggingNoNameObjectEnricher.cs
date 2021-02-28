using Serilog.Core;
using Serilog.Events;

namespace Serilog.Sinks.Unity3D
{
    public class LoggingNoNameObjectEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            if (logEvent.Properties.TryGetValue(Constant.LOGGING_NO_NAME, out var value) && value is UnityObjectValue unityObjectValue)
                logEvent.AddPropertyIfAbsent(new LogEventProperty(Constant.UNITY_OBJECT_PROPERTY, unityObjectValue));
        }
    }
}