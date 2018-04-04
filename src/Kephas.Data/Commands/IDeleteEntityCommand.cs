﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDeleteEntityCommand.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IDeleteEntityCommand interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Commands
{
    using Kephas.Services;

    /// <summary>
    /// Contract for delete entity commands.
    /// </summary>
    [AppServiceContract(AllowMultiple = true, MetadataAttributes = new[] { typeof(DataContextTypeAttribute) })]
    public interface IDeleteEntityCommand : IDataCommand<IDeleteEntityContext, IDataCommandResult>, ISyncDataCommand
    {
    }
}