using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Modules.Utilities
{
    /// <summary>
    /// Реализация ILoggerSink для вывода логов в Unity Console
    /// через Debug.Log, Debug.LogWarning, Debug.LogError и Debug.LogException.
    /// </summary>
    public class UnityConsoleSink : ILoggerSink
    {
        [HideInCallstack]
        public void Log(LogLevelType level, in LogMessage message, Object context)
        {
            switch (level)
            {
                case LogLevelType.Warning:
                    Debug.LogWarning(Compose(message), context);
                    break;
                case LogLevelType.Error:
                    Debug.LogError(Compose(message), context);
                    break;
                default:
                    Debug.Log(Compose(message), context);
                    break;
            }
        }

        [HideInCallstack]
        public void Exception(Exception ex, Object context) => 
            Debug.LogException(ex, context);

        private string Compose(in LogMessage msg)
        {
            if (string.IsNullOrEmpty(msg.Tag))
                return msg.Message;

            return $"[{msg.Tag}] {msg.Message}";
        }
    }
}