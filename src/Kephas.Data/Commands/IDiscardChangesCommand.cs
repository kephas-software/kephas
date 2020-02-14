// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDiscardChangesCommand.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IDiscardChangesCommand interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Commands
{
    using Kephas.Services;

    /// <summary>
    /// Contract for discard changes commands.
    /// </summary>
    [AppServiceContract(AllowMultiple = true, MetadataAttributes = new[] { typeof(DataContextTypeAttribute) })]
    public interface IDiscardChangesCommand : IDataCommand<IDiscardChangesContext, IDataCommandResult>
#if NETSTANDARD2_1
#else
        , ISyncDataCommand<IDiscardChangesContext, IDataCommandResult>
#endif
    {
    }
}