// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestCompositionExtensions.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Defines the TestCompositionExtensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Mef
{
    using System.Composition.Hosting;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public static class TestCompositionExtensions
    {
        public static CompositionContainer CreateCompositionContainer(this ContainerConfiguration configuration)
        {
            Contract.Requires(configuration != null);

            return new CompositionContainer(configuration);
        }
    }
}