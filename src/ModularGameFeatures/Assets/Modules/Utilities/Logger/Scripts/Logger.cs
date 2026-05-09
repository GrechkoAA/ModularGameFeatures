using System;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Modules.Utilities
{
    /// <summary>
    /// Централизованный Logger.
    /// Поддерживает различные типы логирования через ILoggerSink.
    /// Особенности:
    /// - Поддержка нескольких sinks одновременно
    /// - Логирование Info / Warning / Error / Exception
    /// - Автоматическое удаление логов из Release-сборок
    /// - Поддержка Unity context objects
    /// - Скрытие внутренних вызовов логгера из callstack
    /// </summary>
    public class Logger
    {
        private const string DEVELOPMENT_BUILD = "DEVELOPMENT_BUILD";
        private const string UNITY_EDITOR = "UNITY_EDITOR";

        private readonly ILoggerSink[] _sinks;
        private readonly IExceptionSink[] _exceptionSinks;

        public Logger(ILoggerSink[] sinks)
        {
            _sinks = sinks;
            _exceptionSinks = sinks.OfType<IExceptionSink>().ToArray();
        }

        public static Logger Create(params ILoggerSink[] sinks) =>
            new Logger(sinks);

        [HideInCallstack]
        [Conditional(UNITY_EDITOR), Conditional(DEVELOPMENT_BUILD)]
        public void Info(string message, string tag = null, Object context = null)
            => Log(LogLevelType.Info, message, tag, context);

        [HideInCallstack]
        [Conditional(UNITY_EDITOR), Conditional(DEVELOPMENT_BUILD)]
        public void Warning(string message, string tag = null, Object context = null)
            => Log(LogLevelType.Warning, message, tag, context);

        [HideInCallstack]
        [Conditional(UNITY_EDITOR), Conditional(DEVELOPMENT_BUILD)]
        public void Error(string message, string tag = null, Object context = null)
            => Log(LogLevelType.Error, message, tag, context);

        [HideInCallstack]
        public Exception Exception(Exception exception, Object context = null)
        {
            LogException(exception, context);

            return exception;
        }

        [HideInCallstack]
        [Conditional(UNITY_EDITOR), Conditional(DEVELOPMENT_BUILD)]
        private void LogException(Exception exception, Object context)
        {
            for (int i = 0; i < _exceptionSinks.Length; i++)
                _exceptionSinks[i].Exception(exception, context);
        }

        [HideInCallstack]
        private void Log(LogLevelType level, string message, string tag, Object context)
        {
            LogMessage log = new(message, tag);

            for (int i = 0; i < _sinks.Length; i++)
                _sinks[i].Log(level, log, context);
        }
    }
}