// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MediaTypeNames.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
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

            /// <summary>
            /// The BSON media type.
            /// </summary>
            public const string Bson = "application/bson";
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

        /// <summary>
        /// The image group.
        /// </summary>
        public static class Image
        {
            /// <summary>
            /// The PNG media type.
            /// </summary>
            public const string Png = "image/png";

            /// <summary>
            /// The JPEG 2000 media type.
            /// </summary>
            public const string Jpeg2000 = "image/jp2";
        }
    }
}