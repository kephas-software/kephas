// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeclareServices.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.DependencyInjection;
using Kephas.Extensions.DependencyInjection;

// HACK Due to the fact that the analyzer ignores the *ServiceEnumerable, which override theier bases,
// we need to declare the services explicitly here.
// TODO remove these when analyzer gets fixed

[assembly: DeclareTransientService(typeof(ILazyEnumerable<>), typeof(LazyServiceEnumerable<>))]
[assembly: DeclareTransientService(typeof(ILazyEnumerable<,>), typeof(LazyServiceEnumerable<,>))]
[assembly: DeclareTransientService(typeof(IFactoryEnumerable<>), typeof(FactoryServiceEnumerable<>))]
[assembly: DeclareTransientService(typeof(IFactoryEnumerable<,>), typeof(FactoryServiceEnumerable<,>))]
