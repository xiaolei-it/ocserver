﻿using System;
using System.Text;
using System.Threading;

using ClientLogger.Logging.Media;


namespace ClientLogger.Logging
{
    /// <summary>
    /// Writes messages to the console.
    /// </summary>
    /// <remarks>
    /// Colors the output
    /// </remarks>
    public sealed class Logger : ILogger
    {
        private static ConsoleColor defaultConsoleColor;
        private readonly static object lockObject = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="Logger"/> class.
        /// </summary>
        public Logger() 
        {
            defaultConsoleColor = Console.ForegroundColor;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Logger"/> class.
        /// </summary>
        /// <param name="loggerType">The linked type that uses the logger.</param>
        public Logger(Type loggerType)
        {
            defaultConsoleColor = Console.ForegroundColor;

            LoggerType = loggerType;
        }

        /// <summary>
        /// Gets the linked type that uses the logger.
        /// </summary>
        public Type LoggerType { get; private set; }

        /// <summary>
        /// Gets the color for a specified message-type.
        /// </summary>
        /// <param name="messageType">The message-type to get the according color for.</param>
        /// <returns>The <see cref="ConsoleColor"/>, according to the message-type.</returns>
        public static ConsoleColor GetColor(MessageType messageType)
        {
            switch (messageType)
            {
                case MessageType.Debug:
                    return MessageColor.DEBUG;
                case MessageType.Error:
                    return MessageColor.ERROR;
                case MessageType.Fatal:
                    return MessageColor.FATAL;
                case MessageType.Info:
                    return MessageColor.INFO;
                case MessageType.Trace:
                    return MessageColor.TRACE;
                case MessageType.Warning:
                    return MessageColor.WARNING;
            }

            return MessageColor.DEFAULT;
        }


        private static void WriteExtra(object state)
        {
            lock (lockObject)
            {
                var array = (state as object[]);
                var level = ((array[0] != null) ? (MessageType)array[0] : MessageType.Info);
                var message = array[1].ToString();
                var exception = (array[2] as Exception);
                var stringBuilder = new StringBuilder();

                stringBuilder.Append(DateTime.Now.ToString("G"));
                stringBuilder.Append(".");
                stringBuilder.Append(DateTime.Now.Millisecond.ToString("000"));
                stringBuilder.Append(" ");
                stringBuilder.Append(Thread.CurrentThread.ManagedThreadId.ToString("000"));
                stringBuilder.Append(" ");
                stringBuilder.Append(System.Reflection.MethodBase.GetCurrentMethod().Name);
                stringBuilder.Append(" | ");
                stringBuilder.Append(message);
                stringBuilder.AppendLine();
                stringBuilder.Append(exception);

                Console.ForegroundColor = GetColor(level);
                Console.WriteLine(stringBuilder.ToString());
                Console.ForegroundColor = defaultConsoleColor;
            }
        }

        private static void Write(object state)
        {
            lock (lockObject)
            {
                var array = (state as object[]);
                var level = ((array[0] != null) ? (MessageType)array[0] : MessageType.Info);
                var message = array[1].ToString();
                var stringBuilder = new StringBuilder();

                stringBuilder.Append(DateTime.Now.ToString("G"));
                stringBuilder.Append(".");
                stringBuilder.Append(DateTime.Now.Millisecond.ToString("000"));
                stringBuilder.Append(" ");
                stringBuilder.Append(Thread.CurrentThread.ManagedThreadId.ToString("000"));
                stringBuilder.Append(" ");
                stringBuilder.Append(System.Reflection.MethodBase.GetCurrentMethod().Name);
                stringBuilder.Append(" | ");
                stringBuilder.Append(message);

                Console.ForegroundColor = GetColor(level);
                Console.WriteLine(stringBuilder.ToString());
                Console.ForegroundColor = defaultConsoleColor;
            }
        }


        /// <summary>
        /// Write an entry to the console.
        /// </summary>
        /// <param name="messageType">The message-type of the message to log.</param>
        /// <param name="message">The text of the message to log.</param>
        public void Write(MessageType messageType, string message)
        {
            var thread = new Thread(() => Write(new Object[] {messageType, message}));
            thread.Name = ("_log_" + messageType.ToString());
            thread.Start();
        }

        /// <summary>
        /// Write an entry to the console.
        /// </summary>
        /// <param name="messageType">The message-type of the message to log.</param>
        /// <param name="message">The text of the message to log.</param>
        /// <param name="exception">The exception occured with the message to log.</param>
        public void Write(MessageType messageType, string message, Exception exception)
        {
            var thread = new Thread(() => Write(new Object[] { messageType, message }))
            {
                Name = ("_logEx_" + messageType.ToString())
            };
            thread.Start();
        }


        #region ILogger Members

        /// <summary>
        /// Write a Debug message to the console.
        /// </summary>
        /// <param name="message">The text of the message to log.</param>
        public void Debug(string message)
        {
            Write(MessageType.Debug, message);
        }

        /// <summary>
        /// Write a Debug message to the console.
        /// </summary>
        /// <param name="message">The text of the message to log.</param>
        /// <param name="exception">The exception occured with the message to log.</param>
        public void Debug(string message, Exception exception)
        {
            Write(MessageType.Debug, message, exception);
        }

        /// <summary>
        /// Write a Default message to the console.
        /// </summary>
        /// <param name="message">The text of the message to log.</param>
        public void Default(string message)
        {
            Write(MessageType.Default, message);
        }

        /// <summary>
        /// Write a Default message to the console.
        /// </summary>
        /// <param name="message">The text of the message to log.</param>
        /// <param name="exception">The exception occured with the message to log.</param>
        public void Default(string message, Exception exception)
        {
            Write(MessageType.Default, message, exception);
        }

        /// <summary>
        /// Write an Error message to the console.
        /// </summary>
        /// <param name="message">The text of the message to log.</param>
        public void Error(string message)
        {
            Write(MessageType.Error, message);
        }

        /// <summary>
        /// Write an Error message to the console.
        /// </summary>
        /// <param name="message">The text of the message to log.</param>
        /// <param name="exception">The exception occured with the message to log.</param>
        public void Error(string message, Exception exception)
        {
            Write(MessageType.Error, message, exception);
        }

        /// <summary>
        /// Write a Fatal message to the console.
        /// </summary>
        /// <param name="message">The text of the message to log.</param>
        public void Fatal(string message)
        {
            Write(MessageType.Fatal, message);
        }

        /// <summary>
        /// Write a Fatal message to the console.
        /// </summary>
        /// <param name="message">The text of the message to log.</param>
        /// <param name="exception">The exception occured with the message to log.</param>
        public void Fatal(string message, Exception exception)
        {
            Write(MessageType.Fatal, message, exception);
        }

        /// <summary>
        /// Write an Info message to the console.
        /// </summary>
        /// <param name="message">The text of the message to log.</param>
        public void Info(string message)
        {
            Write(MessageType.Info, message);
        }

        /// <summary>
        /// Write an Info message to the console.
        /// </summary>
        /// <param name="message">The text of the message to log.</param>
        /// <param name="exception">The exception occured with the message to log.</param>
        public void Info(string message, Exception exception)
        {
            Write(MessageType.Info, message, exception);
        }

        /// <summary>
        /// Write a Trace message to the console.
        /// </summary>
        /// <param name="message">The text of the message to log.</param>
        public void Trace(string message)
        {
            Write(MessageType.Trace, message);
        }

        /// <summary>
        /// Write a Trace message to the console.
        /// </summary>
        /// <param name="message">The text of the message to log.</param>
        /// <param name="exception">The exception occured with the message to log.</param>
        public void Trace(string message, Exception exception)
        {
            Write(MessageType.Trace, message, exception);
        }

        /// <summary>
        /// Write a Warning message to the console.
        /// </summary>
        /// <param name="message">The text of the message to log.</param>
        public void Warning(string message)
        {
            Write(MessageType.Warning, message);
        }

        /// <summary>
        /// Write an Warning message to the console.
        /// </summary>
        /// <param name="message">The text of the message to log.</param>
        /// <param name="exception">The exception occured with the message to log.</param>
        public void Warning(string message, Exception exception)
        {
            Write(MessageType.Warning, message, exception);
        }

        #endregion
    }
}
