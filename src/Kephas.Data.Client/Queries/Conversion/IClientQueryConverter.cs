// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IClientQueryConverter.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IClientQueryConverter interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Client.Queries.Conversion
{
    using System.Linq;

    using Kephas.Services;

    /// <summary>
    /// Contract for a service converting client queries to server-side executable queries.
    /// </summary>
    [SharedAppServiceContract]
    public interface IClientQueryConverter
    {
        /// <summary>
        /// Converts the provided client query to a queryable which can be executed.
        /// </summary>
        /// <param name="clientQuery">The client query.</param>
        /// <param name="context">The query conversion context.</param>
        /// <returns>
        /// The converted query.
        /// </returns>
        IQueryable ConvertQuery(ClientQuery clientQuery, IClientQueryConversionContext context);
    }
}