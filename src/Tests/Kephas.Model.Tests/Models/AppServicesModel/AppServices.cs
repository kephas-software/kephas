// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServices.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the application services class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Tests.Models.AppServicesModel
{
    using System;

    using Kephas.Services;

    [ScopeSharedAppServiceContract]
    interface ISimpleService {}

    interface IDisposableService : IDisposable {}

    [SharedAppServiceContract(ContractType = typeof(IDisposableService))]
    interface IGenericDisposableService<TArg> : IDisposableService, ICloneable {}

    [AppServiceContract(AsOpenGeneric = true)]
    interface IOpenGenericService<TArg> {}

    [AppServiceContract()]
    interface IClosedGenericService<TArg> {}
}