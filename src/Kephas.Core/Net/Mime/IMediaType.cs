// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMediaType.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Contract for serialization formats.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Net.Mime
{
    using Kephas.Services;

    /// <summary>
    /// Contract for serialization formats.
    /// </summary>
    [SingletonAppServiceContract(AllowMultiple = true)]
    public interface IMediaType
    {
    }
}