using System;
using System.IO;
using JetBrains.Annotations;
using Serilog.Events;
using Object = UnityEngine.Object;

namespace Serilog.Sinks.Unity3D
{
    public class UnityObjectValue : LogEventPropertyValue
    {
        public readonly Object Value;

        public UnityObjectValue([NotNull] Object value)
        {
            Value = value;
        }

        public override void Render(TextWriter output, string format = null, IFormatProvider formatProvider = null)
        {
            try
            {
                output.Write(Value.ToString());
            }
            catch
            {
                output.Write(Constant.INVALID_OBJECT);
            }
        }
    }
}