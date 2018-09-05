// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Base64Data.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the base 64 class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Text.Encoding
{
    using System;
    using System.Text;

    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// A Base64 data.
    /// </summary>
    public class Base64Data
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Base64Data"/> class.
        /// </summary>
        /// <param name="base64String">The base64 string.</param>
        public Base64Data(string base64String)
        {
            Requires.NotNullOrEmpty(base64String, nameof(base64String));

            var indexOfComma = base64String.IndexOf(",", StringComparison.Ordinal);
            var plainString = base64String;
            if (indexOfComma > 0)
            {
                this.MimeType = base64String.Substring(0, indexOfComma);
                plainString = base64String.Substring(indexOfComma + 1);
            }

            this.Bytes = Convert.FromBase64String(plainString);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Base64Data"/> class.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <param name="mimeType">The MIME type (optional).</param>
        public Base64Data(byte[] bytes, string mimeType = null)
        {
            Requires.NotNull(bytes, nameof(bytes));

            this.Bytes = bytes;
            this.MimeType = mimeType;
        }

        /// <summary>
        /// Gets the MIME type.
        /// </summary>
        /// <value>
        /// The MIME type.
        /// </value>
        public string MimeType { get; }

        /// <summary>
        /// Gets the bytes.
        /// </summary>
        /// <value>
        /// The bytes.
        /// </value>
        public byte[] Bytes { get; }

        /// <summary>
        /// Initializes a new <see cref="Base64Data"/> from the given Base64 string.
        /// </summary>
        /// <param name="base64String">The base64 string.</param>
        /// <returns>
        /// A <see cref="Base64Data"/> instance representing the provided string.
        /// </returns>
        public static Base64Data FromBase64String(string base64String)
        {
            return new Base64Data(base64String);
        }

        /// <summary>
        /// Converts the provided bytes to a base 64 string.
        /// </summary>
        /// <param name="bytes">The bytes to convert.</param>
        /// <param name="mimeType">The MIME type (optional).</param>
        /// <returns>
        /// The given data converted to a Base64 string.
        /// </returns>
        public static string ToBase64String(byte[] bytes, string mimeType = null)
        {
            return new Base64Data(bytes, mimeType).ToBase64String();
        }

        /// <summary>
        /// Converts the provided plain string to a base 64 string.
        /// </summary>
        /// <param name="plainString">The plain string to convert.</param>
        /// <param name="mimeType">The MIME type (optional).</param>
        /// <returns>
        /// The given string converted to a Base64 string.
        /// </returns>
        public static string ToBase64String(string plainString, string mimeType = null)
        {
            Requires.NotNull(plainString, nameof(plainString));

            var bytes = Encoding.UTF8.GetBytes(plainString);

            return ToBase64String(bytes, mimeType);
        }

        /// <summary>
        /// Converts this object to a Base64 string.
        /// </summary>
        /// <returns>
        /// This object as a Base64 string.
        /// </returns>
        public string ToBase64String()
        {
            var sb = new StringBuilder();
            if (this.MimeType != null)
            {
                sb.Append(this.MimeType).Append(", ");
            }

            sb.Append(Convert.ToBase64String(this.Bytes));

            return sb.ToString();
        }
    }
}