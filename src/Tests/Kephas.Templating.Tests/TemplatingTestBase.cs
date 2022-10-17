// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TemplatingTestBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Templating.Tests
{
    using System.Collections.Generic;
    using System.Reflection;

    using Kephas.Testing.Injection;

    public class TemplatingTestBase  : TestBase
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
