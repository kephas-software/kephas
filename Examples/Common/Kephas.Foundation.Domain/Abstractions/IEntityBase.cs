// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEntityBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IEntityBase interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Foundation.Domain.Abstractions
{
    using Kephas.Data.Model.Abstractions;
    using Kephas.Model.AttributedModel;

    /// <summary>
    /// Interface for entity base.
    /// </summary>
    [Abstract]
    public interface IEntityBase : IIdentifiable<long>, ITagged
    {
    }
}