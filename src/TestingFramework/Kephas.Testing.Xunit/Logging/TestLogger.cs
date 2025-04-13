// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestLogger.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Text;
using Microsoft.Extensions.Logging;

namespace Kephas.Testing.Logging;

/// <summary>
/// Logger for testing purposes.
/// </summary>
/// <typeparam name="T">The associated type to log.</typeparam>
public class TestLogger<T> : ILogger<T>
{
    private readonly LogLevel? minimumLevel;
    private readonly Action<LogLevel, Exception?, string> callback;
    private readonly StringBuilder sb;

    /// <summary>
    /// Initializes a new instance of the <see cref="TestLogger{T}"/> class.
    /// </summary>
    public TestLogger(StringBuilder? stringBuilder, LogLevel? minimumLevel = null)
        : this(stringBuilder, null, minimumLevel)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TestLogger{T}"/> class.
    /// </summary>
    public TestLogger(Action<LogLevel, Exception?, string>? callback = null, LogLevel? minimumLevel = null)
        : this(null, callback, minimumLevel)
    {
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="TestLogger{T}"/> class.
    /// </summary>
    protected TestLogger(StringBuilder? stringBuilder, Action<LogLevel, Exception?, string>? callback, LogLevel? minimumLevel)
    {
        this.minimumLevel = minimumLevel;
        this.sb = stringBuilder ?? new StringBuilder();
        this.callback = callback ?? ((level, ex, message) =>
        {
            sb.Append(ex is null ? null : ex.GetType().Name + " ").AppendLine($"{level}: {message}");
        });
    }

    /// <summary>Writes a log entry.</summary>
    /// <param name="logLevel">Entry will be written on this level.</param>
    /// <param name="eventId">Id of the event.</param>
    /// <param name="state">The entry to be written. Can be also an object.</param>
    /// <param name="exception">The exception related to this entry.</param>
    /// <param name="formatter">Function to create a <see cref="T:System.String" /> message of the <paramref name="state" /> and <paramref name="exception" />.</param>
    /// <typeparam name="TState">The type of the object to be written.</typeparam>
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (this.IsEnabled(logLevel))
        {
            this.callback(logLevel, exception, formatter(state, exception));
        }
    }

    /// <summary>
    /// Checks if the given <paramref name="logLevel" /> is enabled.
    /// </summary>
    /// <param name="logLevel">level to be checked.</param>
    /// <returns><c>true</c> if enabled.</returns>
    public bool IsEnabled(LogLevel logLevel)
    {
        return minimumLevel is null || minimumLevel.Value <= logLevel;
    }

    /// <summary>Begins a logical operation scope.</summary>
    /// <param name="state">The identifier for the scope.</param>
    /// <typeparam name="TState">The type of the state to begin scope for.</typeparam>
    /// <returns>An <see cref="T:System.IDisposable" /> that ends the logical operation scope on dispose.</returns>
    public IDisposable BeginScope<TState>(TState state)
        where TState : notnull
    {
        return new Scope();
    }

    /// <summary>Returns a string that represents the current object.</summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString() => sb.ToString();

    private sealed class Scope : IDisposable
    {
        public void Dispose()
        {
            // no need to dispose anything.
        }
    }
}
