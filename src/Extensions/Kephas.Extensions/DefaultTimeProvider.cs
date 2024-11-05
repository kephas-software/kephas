// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimeProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.DependencyInjection;
using Kephas.Extensions;

[assembly: DeclareSingletonService<TimeProvider, DefaultTimeProvider>]

namespace Kephas.Extensions;

/// <summary>
/// The default time provider.
/// </summary>
public class DefaultTimeProvider : TimeProvider;