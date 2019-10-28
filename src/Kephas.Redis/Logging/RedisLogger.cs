// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RedisLogger.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the redis logger class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Redis.Logging
{
    using System.IO;
    using System.Text;

    using Kephas.Logging;

    /// <summary>
    /// The Redis logger.
    /// </summary>
    public class RedisLogger : TextWriter
    {
        private readonly ILogger logger;
        private readonly StringBuilder stringBuilder = new StringBuilder();

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisLogger"/> class.
        /// </summary>
        /// <param name="logManager">The log manager.</param>
        public RedisLogger(ILogManager logManager)
        {
            this.logger = logManager.GetLogger(this.GetType());
        }

        /// <summary>
        /// Gets the character encoding in which the output is
        /// written.
        /// </summary>
        /// <value>
        /// The character encoding in which the output is written.
        /// </value>
        public override Encoding Encoding => Encoding.UTF8;

        /// <summary>
        /// Writes a character to the text string or stream.
        /// </summary>
        /// <param name="value">The character to write to the text stream.</param>
        public override void Write(char value)
        {
            this.stringBuilder.Append(value);
        }

        /// <summary>
        /// Writes a string to the text string or stream.
        /// </summary>
        /// <param name="value">The string to write.</param>
        public override void Write(string value)
        {
            this.stringBuilder.Append(value);
        }

        /// <summary>
        /// Clears all buffers for the current writer and causes any buffered data to be written to the
        /// underlying device.
        /// </summary>
        public override void Flush()
        {
            if (this.logger.IsTraceEnabled())
            {
                this.logger.Trace(this.stringBuilder.ToString());
            }

            this.stringBuilder.Clear();
        }
    }
}
