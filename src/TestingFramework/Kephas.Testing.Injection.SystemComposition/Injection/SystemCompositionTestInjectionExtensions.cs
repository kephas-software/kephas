// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SystemCompositionTestInjectionExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Defines the TestCompositionExtensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Testing.Injection
{
    using System.Composition.Hosting;
    using System.Diagnostics.CodeAnalysis;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Injection.SystemComposition.Hosting;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public static class SystemCompositionTestInjectionExtensions
    {
        public static SystemCompositionInjector CreateInjector(this ContainerConfiguration configuration)
        {
            Requires.NotNull(configuration, nameof(configuration));

            return new SystemCompositionInjector(configuration);
        }
    }
}