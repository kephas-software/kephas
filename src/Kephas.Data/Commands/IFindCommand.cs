// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFindCommand.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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