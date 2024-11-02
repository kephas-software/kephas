// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IServiceInfoCollection.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.DependencyInjection.Reflection;

/// <summary>
/// The collection of <see cref="IServiceInfo"/>.
/// </summary>
public interface IServiceInfoCollection : IList<IServiceInfo>;