// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestCompositionExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Defines the TestCompositionExtensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Testing.Composition.Mef
{
    using System.Composition.Hosting;
    using System.Diagnostics.CodeAnalysis;

    using Kephas.Composition.Mef.Hosting;
    using Kephas.Diagnostics.Contracts;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public static class TestCompositionExtensions
    {
        public static MefCompositionContainer CreateCompositionContainer(this ContainerConfiguration configuration)
        {
            Requires.NotNull(configuration, nameof(configuration));

            return new MefCompositionContainer(configuration);
        }
    }
}