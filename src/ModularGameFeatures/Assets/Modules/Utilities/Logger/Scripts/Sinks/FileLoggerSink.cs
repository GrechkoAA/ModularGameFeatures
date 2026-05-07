using System;
using System.IO;
using System.Text;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Modules.Utilities
{
    /// <summary>
    /// Sink для записи логов в файл.
    /// Поддерживает логирование сообщений, exception и Unity context objects.
    /// </summary>
    public class FileLoggerSink : ILoggerSink, IDisposable
    {
        private readonly string _path;
        private readonly StringBuilder _buffer = new(1024);
        private readonly object _lock = new();

        private bool _disposed;

        public FileLoggerSink(string fileName = "log.txt")
        {
            _path = Path.Combine(Application.persistentDataPath, fileName);
            WriteLine($"--- LOG START {DateTime.Now} ---");
        }

        public void Log(LogLevelType level, in LogMessage message, Object context)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(FileLoggerSink));

            lock (_lock)
            {
                _buffer.Clear();

                AppendHeader(level);
                AppendTag(message.Tag);
                _buffer.Append(message.Message);
                AppendContext(context);

                WriteBuffer();
            }
        }

        public void Exception(Exception exception, Object context)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(FileLoggerSink));

            lock (_lock)
            {
                _buffer.Clear();

                AppendHeader("EXCEPTION");

                AppendException(exception);
                AppendContext(context);

                _buffer.Append('\n');

                WriteBuffer();
            }
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            WriteLine($"--- LOG END {DateTime.Now} ---");
        }

        private void AppendHeader(object level)
        {
            _buffer.Append('[');
            _buffer.Append(DateTime.Now.ToString("HH:mm:ss"));
            _buffer.Append("] [");
            _buffer.Append(level);
            _buffer.Append("] ");
        }

        private void AppendTag(string tag)
        {
            if (string.IsNullOrEmpty(tag))
                return;

            _buffer.Append('[');
            _buffer.Append(tag);
            _buffer.Append("] ");
        }

        private void AppendContext(Object context)
        {
            if (context == null)
                return;

            _buffer.Append(" (");
            _buffer.Append(context.name);
            _buffer.Append(')');
        }

        private void AppendException(Exception ex)
        {
            while (ex != null)
            {
                _buffer.Append(ex.GetType().Name);
                _buffer.Append(": ");
                _buffer.Append(ex.Message);
                _buffer.Append('\n');

                if (!string.IsNullOrEmpty(ex.StackTrace))
                {
                    _buffer.Append(ex.StackTrace);
                    _buffer.Append('\n');
                }

                ex = ex.InnerException;

                if (ex != null)
                    _buffer.Append("INNER: ");
            }
        }

        private void WriteBuffer()
        {
            _buffer.Append('\n');
            File.AppendAllText(_path, _buffer.ToString(), Encoding.UTF8);
        }

        private void WriteLine(string text) => 
            File.AppendAllText(_path, text + "\n", Encoding.UTF8);
    }
}