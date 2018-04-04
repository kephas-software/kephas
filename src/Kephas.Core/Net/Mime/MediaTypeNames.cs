// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MediaTypeNames.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the media type names class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Net.Mime
{
    /// <summary>
    /// Provides media type names.
    /// </summary>
    public static class MediaTypeNames
    {
        /// <summary>
        /// The application group.
        /// </summary>
        public static class Application
        {
            /// <summary>
            /// The JSON media type.
            /// </summary>
            public const string Json = "application/json";

            /// <summary>
            /// The XML media type.
            /// </summary>
            public const string Xml = "application/xml";
        }

        /// <summary>
        /// The text group.
        /// </summary>
        public static class Text
        {
            /// <summary>
            /// The CSV media type.
            /// </summary>
            public const string Csv = "text/csv";

            /// <summary>
            /// The XML media type.
            /// </summary>
            public const string Xml = "text/xml";

            /// <summary>
            /// The plain text media type.
            /// </summary>
            public const string Plain = "text/plain";

            /// <summary>
            /// The INI media type.
            /// </summary>
            public const string Ini = "x-text/ini";
        }
    }
}