// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IClientDataContextProvider.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IClientDataContextProvider interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.ClientEntities
{
    using Kephas.Services;

    /// <summary>
    /// Shared application contract for providers of data contexts managing client entities (DTOs).
    /// </summary>
    [SharedAppServiceContract]
    public interface IClientDataContextProvider : IDataContextProvider
    {
    }
}