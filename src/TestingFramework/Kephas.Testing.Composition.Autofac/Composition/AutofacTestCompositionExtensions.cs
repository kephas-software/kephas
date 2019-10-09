// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutofacTestCompositionExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Defines the TestCompositionExtensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Testing.Composition
{
    using System.Diagnostics.CodeAnalysis;

    using Autofac;

    using Kephas.Composition.Autofac.Hosting;
    using Kephas.Diagnostics.Contracts;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public static class AutofacTestCompositionExtensions
    {
        public static AutofacCompositionContainer CreateCompositionContainer(this ContainerBuilder configuration)
        {
            Requires.NotNull(configuration, nameof(configuration));

            return new AutofacCompositionContainer(configuration);
        }
    }
}