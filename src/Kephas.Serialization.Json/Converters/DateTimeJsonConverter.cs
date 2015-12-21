// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DateTimeJsonConverter.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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