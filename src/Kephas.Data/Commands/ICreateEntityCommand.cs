// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICreateEntityCommand.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the ICreateEntityCommand interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Commands
{
    using Kephas.Services;

    /// <summary>
    /// Application service contract for commands creating an entity.
    /// </summary>
    [AppServiceContract(AllowMultiple = true)]
    public interface ICreateEntityCommand : IDataCommand<ICreateEntityContext, ICreateEntityResult>
    {
    }
}