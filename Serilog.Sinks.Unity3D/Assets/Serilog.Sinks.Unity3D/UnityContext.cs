using System;
using Serilog.Context;

namespace Serilog.Sinks.Unity3D
{
    public static class UnityContext
    {
        public static IDisposable PushObject(Object obj)
        {
            return LogContext.PushProperty(Constant.UNITY_OBJECT_PROPERTY, obj, true);
        }
    }
}