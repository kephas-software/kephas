// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypesServiceInfoProviderServices.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.DependencyInjection;

namespace Kephas.Tests.DependencyInjection.TestServices;

[TransientService]
internal interface ITransientService;

[ScopedService]
internal interface IScopedService;

[SingletonService]
public interface ISingletonService;

public abstract class MyAbstractClass;
    
public interface IMyService;

internal class MyService : IMyService;

[TransientService]
public interface IMyLogger<in T>;

internal class MyLogger<T> : IMyLogger<T>;
