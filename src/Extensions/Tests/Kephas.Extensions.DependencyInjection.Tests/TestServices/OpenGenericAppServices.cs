// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OpenGenericAppServices.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.DependencyInjection;
using Kephas.DependencyInjection.Metadata;

namespace Kephas.Extensions.Tests.TestServices;

[SingletonService(RegistrationKind.AsOpenGeneric)]
public interface IOpen<T> { }

public class DefaultOpen<T> : IOpen<T> { }

[Priority(Priority.High)]
public class MoreOpen<T> : IOpen<T> { }

[Priority(Priority.High)]
public class MoreOpenWithDependency<T> : IOpen<T>
{
    public MoreOpenWithDependency(Dependency dep)
    {
    }
}

[TransientService]
public class Dependency { }
