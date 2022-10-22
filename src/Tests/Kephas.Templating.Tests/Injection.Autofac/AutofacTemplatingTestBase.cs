// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutofacTemplatingTestBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the scripting test base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Templating.Tests.Injection.Autofac
{
    using System.Collections.Generic;
    using System.Reflection;

    using Kephas.Testing.Services;

    public class AutofacTemplatingTestBase : AutofacInjectionTestBase
    {
        protected override IEnumerable<Assembly> GetAssemblies()
        {
            return new List<Assembly>(base.GetAssemblies())
            {
                typeof(DefaultTemplateProcessor).Assembly, /* Kephas.Templating */
            };
        }
    }
}
