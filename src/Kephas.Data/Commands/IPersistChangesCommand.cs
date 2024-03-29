﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPersistChangesCommand.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IPersistChangesCommand interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Commands
{
    using Kephas.Operations;
    using Kephas.Services;

    /// <summary>
    /// Contract for persist changes commands.
    /// </summary>
    [AppServiceContract(AllowMultiple = true)]
    public interface IPersistChangesCommand : IDataCommand<IPersistChangesContext, IOperationResult>
    {
    }
}