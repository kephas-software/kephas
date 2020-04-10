// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DateTimeJsonConverter.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   A date time JSON converter.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization.Json.Converters
{
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// A date time JSON converter.
    /// </summary>
    public class DateTimeJsonConverter : IsoDateTimeConverter, IJsonConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeJsonConverter"/> class.
        /// </summary>
        public DateTimeJsonConverter()
        {
            this.DateTimeFormat = "yyyy-MM-dd\\THH:mm:ss.fffK";
        }
    }
}