using Serilog.Core;
using Serilog.Events;

namespace Serilog.Sinks.Unity3D
{
    public class UnityObjectDestructuringPolicy : IDestructuringPolicy
    {
        public bool TryDestructure(object value, ILogEventPropertyValueFactory propertyValueFactory, out LogEventPropertyValue result)
        {
            var unityObject = value as UnityEngine.Object;
            result = unityObject == null ? null : new UnityObjectValue(unityObject);
            return unityObject != null;
        }
    }
}