// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the stream extensions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.IO
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Threading.Tasks;

    /// <summary>
    /// A stream extensions.
    /// </summary>
    public static class StreamExtensions
    {
        /// <summary>
        /// A Stream extension method that reads the content as a string.
        /// </summary>
        /// <param name="input">The input to act on.</param>
        /// <returns>
        /// The content as string.
        /// </returns>
        public static string ReadAllString(this Stream input)
        {
            input = input ?? throw new ArgumentNullException(nameof(input));

            using var reader = new StreamReader(input);
            return reader.ReadToEnd();
        }

        /// <summary>
        /// A Stream extension method that reads the content as a string asynchronously.
        /// </summary>
        /// <param name="input">The input to act on.</param>
        /// <returns>
        /// The content as string.
        /// </returns>
        public static async Task<string> ReadAllStringAsync(this Stream input)
        {
            input = input ?? throw new ArgumentNullException(nameof(input));

            using var reader = new StreamReader(input);
            return await reader.ReadToEndAsync().PreserveThreadContext();
        }

        /// <summary>
        /// A Stream extension method that reads all bytes.
        /// </summary>
        /// <param name="input">The input to act on.</param>
        /// <returns>
        /// An array of byte.
        /// </returns>
        public static byte[] ReadAllBytes(this Stream input)
        {
            input = input ?? throw new ArgumentNullException(nameof(input));

            using var mem = new MemoryStream();

            // read all bytes
            const int ChunkSize = 1024;
            var buffer = new byte[ChunkSize];
            var readLength = 0;
            while ((readLength = input.Read(buffer, 0, ChunkSize)) > 0)
            {
                mem.Write(buffer, 0, readLength);
            }

            return mem.ToArray();
        }

        /// <summary>
        /// A Stream extension method that reads all bytes asynchronously.
        /// </summary>
        /// <param name="input">The input to act on.</param>
        /// <param name="cancellationToken">Optional. the cancellation token.</param>
        /// <returns>
        /// An array of byte.
        /// </returns>
        public static async Task<byte[]> ReadAllBytesAsync(this Stream input, CancellationToken cancellationToken = default)
        {
            input = input ?? throw new ArgumentNullException(nameof(input));

            using var mem = new MemoryStream();

            // read all bytes
            const int ChunkSize = 1024;
            var buffer = new byte[ChunkSize];
            var readLength = 0;
            while ((readLength = await input.ReadAsync(buffer, 0, ChunkSize, cancellationToken).PreserveThreadContext()) > 0)
            {
                await mem.WriteAsync(buffer, 0, readLength, cancellationToken).PreserveThreadContext();
            }

            return mem.ToArray();
        }
    }
}