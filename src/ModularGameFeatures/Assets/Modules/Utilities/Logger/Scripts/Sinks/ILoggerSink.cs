using System;

namespace Modules.Utilities
{
    /// <summary>
    /// Определяет контракт для обработчиков логов.
    /// Реализации могут выводить логи в консоль, файл, сервер и другие источники.
    /// </summary>
    public interface ILoggerSink
    {
        void Log(LogLevelType level, in LogMessage message, UnityEngine.Object context);
    }

    public interface IExceptionSink
    {
        void Exception(Exception exception, UnityEngine.Object context);
    }
}