// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceSelector.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection;

using Kephas.Services;

[OverridePriority(Priority.Low)]
public class ServiceSelector<T> : IServiceSelector<T>
{
    
}