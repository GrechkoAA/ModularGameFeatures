namespace Modules.Utilities
{
    /// <summary>
    /// Immutable-структура, содержащая данные лог-сообщения.
    /// Используется для передачи информации между Logger и ILoggerSink.
    /// </summary>
    public readonly struct LogMessage
    {
        public readonly string Message;
        public readonly string Tag;

        public LogMessage(string message, string tag)
        {
            Message = message;
            Tag = tag;
        }
    }
}