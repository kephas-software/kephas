// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutofacTestInjectionExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Defines the TestCompositionExtensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Testing.Injection
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Autofac;
    using Kephas.Injection.Autofac;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public static class AutofacTestInjectionExtensions
    {
        public static AutofacServiceProvider CreateInjector(this ContainerBuilder configuration)
        {
            configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            return new AutofacServiceProvider(configuration, null);
        }
    }
}