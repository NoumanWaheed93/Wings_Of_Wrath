using System;
using UnityEngine;

namespace LogUtility
{
    public class CustomLogHandler : ILogHandler
    {
        private readonly ILogHandler _default;

        public CustomLogHandler()
        {
            _default = Debug.unityLogger.logHandler;
        }

        public void LogException(Exception exception, UnityEngine.Object context)
        {
            _default.LogException(exception, context);
        }

        public void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args)
        {
#if DEBUG
            var caller = GetCaller();
            var color = GetColor(caller);
            _default.LogFormat(logType, context, $"<color=#{color}><b>[{caller}]</b></color> {format}", args);
#else
            _default.LogFormat(logType, context, format, args);
#endif
        }

        private static string GetColor(string name)
        {
            var hue = (uint)name.GetHashCode() / (float)uint.MaxValue;
            var color = Color.HSVToRGB(hue, 0.6f, 1f);
            return ColorUtility.ToHtmlStringRGB(color);
        }

        private static string GetCaller()
        {
            var stack = new System.Diagnostics.StackTrace(2, false);
            for(int i = 0; i < stack.FrameCount; i++)
            {
                var type = stack.GetFrame(i)?.GetMethod()?.DeclaringType;

                if(type is not null && type.Namespace?.StartsWith("UnityEngine") != true)
                {
                    return type.Name;
                }
            }

            return "Default";
        }

    }

}
