// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServices.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
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
    interface ISimpleService
    {
        void DoSomething(IDisposableService disposableService);

        string GetSomething(string name);
    }

    interface IDisposableService : IDisposable {}

    [SharedAppServiceContract(ContractType = typeof(IDisposableService))]
    interface IGenericDisposableService<TArg> : IDisposableService, ICloneable {}

    [AppServiceContract(AsOpenGeneric = true)]
    interface IOpenGenericService<TArg> {}

    [AppServiceContract()]
    interface IClosedGenericService<TArg> {}
}