// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataStreamHelper.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the data stream helper class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.IO.DataStreams
{
    using System.IO;
    using System.Text;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Net.Mime;

    /// <summary>
    /// A data stream helper.
    /// </summary>
    public static class DataStreamHelper
    {
        /// <summary>
        /// Creates a <see cref="DataStream"/> from a string containing JSON data.
        /// </summary>
        /// <param name="data">The JSON data.</param>
        /// <param name="name">The name (optional).</param>
        /// <returns>
        /// A DataStream.
        /// </returns>
        public static DataStream FromJsonString(string data, string name = null)
        {
            return new DataStream(
                new MemoryStream(Encoding.UTF8.GetBytes(data)),
                name,
                MediaTypeNames.Application.Json,
                Encoding.UTF8,
                ownsStream: true);
        }

        /// <summary>
        /// Creates a <see cref="DataStream"/> from a string containing JSON data.
        /// </summary>
        /// <param name="filePath">The path to the file containing JSON data.</param>
        /// <param name="name">The name (optional).</param>
        /// <returns>
        /// A DataStream.
        /// </returns>
        public static DataStream FromJsonFile(string filePath, string name = null)
        {
            Requires.NotNullOrEmpty(filePath, nameof(filePath));

            var fileStream = File.OpenRead(filePath);

            return new DataStream(
                fileStream,
                name ?? Path.GetFileName(filePath),
                MediaTypeNames.Application.Json,
                Encoding.UTF8,
                ownsStream: true);
        }

        /// <summary>
        /// Creates a <see cref="DataStream"/> from a string containing XML data.
        /// </summary>
        /// <param name="data">The XML data.</param>
        /// <param name="name">The name (optional).</param>
        /// <returns>
        /// A DataStream.
        /// </returns>
        public static DataStream FromXmlString(string data, string name = null)
        {
            return new DataStream(
                new MemoryStream(Encoding.UTF8.GetBytes(data)),
                name,
                MediaTypeNames.Application.Xml,
                Encoding.UTF8,
                ownsStream: true);
        }

        /// <summary>
        /// Creates a <see cref="DataStream"/> from a string containing XML data.
        /// </summary>
        /// <param name="filePath">The path to the file containing XML data.</param>
        /// <param name="name">The name (optional).</param>
        /// <returns>
        /// A DataStream.
        /// </returns>
        public static DataStream FromXmlFile(string filePath, string name = null)
        {
            Requires.NotNullOrEmpty(filePath, nameof(filePath));

            var fileStream = File.OpenRead(filePath);

            return new DataStream(
                fileStream,
                name ?? Path.GetFileName(filePath),
                MediaTypeNames.Application.Xml,
                Encoding.UTF8,
                ownsStream: true);
        }
    }
}