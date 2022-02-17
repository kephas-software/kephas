// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NullIdGenerator.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data
{
    using Kephas.Services;

    /// <summary>
    /// A null <see cref="IIdGenerator"/> service.
    /// </summary>
    /// <seealso cref="IIdGenerator" />
    [OverridePriority(Priority.Lowest)]
    public class NullIdGenerator : IIdGenerator
    {
        /// <summary>
        /// Generates a unique identifier.
        /// </summary>
        /// <returns>
        /// The unique identifier.
        /// </returns>
        public long GenerateId()
        {
            throw new NullServiceException(typeof(IIdGenerator));
        }
    }
}