// --------------------------------------------------------------------------------------------------------------------
// <copyright file="INamedEntityBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the INamedEntityBase interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Foundation.Domain.Abstractions
{
    using Kephas.Data.Model.Abstractions;
    using Kephas.Model.AttributedModel;

    /// <summary>
    /// Interface for named entity base.
    /// </summary>
    [Abstract]
    public interface INamedEntityBase : IEntityBase, INamed, IDescribed
    {
    }
}