// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeclareServices.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.DependencyInjection;

[assembly: DeclareTransientService(typeof(ILazyEnumerable<>), typeof(LazyEnumerable<>))]
[assembly: DeclareTransientService(typeof(ILazyEnumerable<,>), typeof(LazyEnumerable<,>))]
[assembly: DeclareTransientService(typeof(IFactoryEnumerable<>), typeof(FactoryEnumerable<>))]
[assembly: DeclareTransientService(typeof(IFactoryEnumerable<,>), typeof(FactoryEnumerable<,>))]
