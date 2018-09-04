// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFindCommand.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IFindCommand interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Commands
{
    using Kephas.Services;

    /// <summary>
    /// Contract for find commands.
    /// </summary>
    [AppServiceContract(AllowMultiple = true, MetadataAttributes = new[] { typeof(DataContextTypeAttribute) })]
    public interface IFindCommand : IDataCommand<IFindContext, IFindResult>
    {
    }
}