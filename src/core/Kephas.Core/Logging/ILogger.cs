// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILogger.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Logger abstract interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Logging
{
    using System;

    /// <summary>
  /// Logger abstract interface.
  /// </summary>
  public interface ILogger
  {
    /// <summary>
    /// Logs fatal exceptions.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="exception">The exception.</param>
    void Fatal(object message, Exception exception = null);

    /// <summary>
    /// Logs the fatal format.
    /// </summary>
    /// <param name="messageFormat">The message format.</param>
    /// <param name="args">The arguments.</param>
    void FatalFormat(string messageFormat, params object[] args);

    /// <summary>
    /// Logs the error.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="exception">The exception.</param>
    void Error(object message, Exception exception = null);

    /// <summary>
    /// Logs the error format.
    /// </summary>
    /// <param name="messageFormat">The message format.</param>
    /// <param name="args">The arguments.</param>
    void ErrorFormat(string messageFormat, params object[] args);

    /// <summary>
    /// Logs the warning.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="exception">The exception.</param>
    void Warn(object message, Exception exception = null);

    /// <summary>
    /// Logs the warn format.
    /// </summary>
    /// <param name="messageFormat">The message format.</param>
    /// <param name="args">The arguments.</param>
    void WarnFormat(string messageFormat, params object[] args);

    /// <summary>
    /// Logs the information.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="exception">The exception.</param>
    void Info(object message, Exception exception = null);

    /// <summary>
    /// Logs the information format.
    /// </summary>
    /// <param name="messageFormat">The message format.</param>
    /// <param name="args">The arguments.</param>
    void InfoFormat(string messageFormat, params object[] args);

    /// <summary>
    /// Logs the debug.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="exception">The exception.</param>
    void Debug(object message, Exception exception = null);

    /// <summary>
    /// Logs the debug format.
    /// </summary>
    /// <param name="messageFormat">The message format.</param>
    /// <param name="args">The arguments.</param>
    void DebugFormat(string messageFormat, params object[] args);

    /// <summary>
    /// Logs the trace.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="exception">The exception.</param>
    void Trace(object message, Exception exception = null);

    /// <summary>
    /// Logs the trace format.
    /// </summary>
    /// <param name="messageFormat">The message format.</param>
    /// <param name="args">The arguments.</param>
    void TraceFormat(string messageFormat, params object[] args);
  }

  /// <summary>
  /// Extension methods for <see cref="ILogger"/>.
  /// </summary>
  public static class LoggerExtensions
  {
    /// <summary>
    /// Logs fatal exceptions.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="message">The message.</param>
    /// <param name="exception">The exception.</param>
    public static void SafeFatal(this ILogger logger, object message, Exception exception = null)
    {
      if (logger == null)
      {
        return;
      }

      logger.Fatal(message, exception);
    }

    /// <summary>
    /// Logs the fatal format.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="messageFormat">The message format.</param>
    /// <param name="args">The arguments.</param>
    public static void SafeFatalFormat(this ILogger logger, string messageFormat, params object[] args)
    {
      if (logger == null)
      {
        return;
      }

      logger.FatalFormat(messageFormat, args);
    }

    /// <summary>
    /// Logs the error.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="message">The message.</param>
    /// <param name="exception">The exception.</param>
    public static void SafeError(this ILogger logger, object message, Exception exception = null)
    {
      if (logger == null)
      {
        return;
      }

      logger.Error(message, exception);
    }

    /// <summary>
    /// Loes the error format.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="messageFormat">The message format.</param>
    /// <param name="args">The arguments.</param>
    public static void SafeErrorFormat(this ILogger logger, string messageFormat, params object[] args)
    {
      if (logger == null)
      {
        return;
      }

      logger.ErrorFormat(messageFormat, args);
    }

    /// <summary>
    /// Logs the warn.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="message">The message.</param>
    /// <param name="exception">The exception.</param>
    public static void SafeWarn(this ILogger logger, object message, Exception exception = null)
    {
      if (logger == null)
      {
        return;
      }

      logger.Warn(message, exception);
    }

    /// <summary>
    /// Logs the warn format.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="messageFormat">The message format.</param>
    /// <param name="args">The arguments.</param>
    public static void SafeWarnFormat(this ILogger logger, string messageFormat, params object[] args)
    {
      if (logger == null)
      {
        return;
      }

      logger.WarnFormat(messageFormat, args);
    }

    /// <summary>
    /// Logs the information.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="message">The message.</param>
    /// <param name="exception">The exception.</param>
    public static void SafeInfo(this ILogger logger, object message, Exception exception = null)
    {
      if (logger == null)
      {
        return;
      }

      logger.Info(message, exception);
    }

    /// <summary>
    /// Logs the information format.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="messageFormat">The message format.</param>
    /// <param name="args">The arguments.</param>
    public static void SafeInfoFormat(this ILogger logger, string messageFormat, params object[] args)
    {
      if (logger == null)
      {
        return;
      }

      logger.InfoFormat(messageFormat, args);
    }

    /// <summary>
    /// Logs the debug.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="message">The message.</param>
    /// <param name="exception">The exception.</param>
    public static void SafeDebug(this ILogger logger, object message, Exception exception = null)
    {
      if (logger == null)
      {
        return;
      }

      logger.Debug(message, exception);
    }

    /// <summary>
    /// Logs the debug format.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="messageFormat">The message format.</param>
    /// <param name="args">The arguments.</param>
    public static void SafeDebugFormat(this ILogger logger, string messageFormat, params object[] args)
    {
      if (logger == null)
      {
        return;
      }

      logger.DebugFormat(messageFormat, args);
    }
  }
}