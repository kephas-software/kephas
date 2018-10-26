// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataTestBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the data test base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Composition.Tests
{
    using System;
    using System.Reflection;

    using Kephas.Composition;
    using Kephas.Testing.Composition.Mef;

    public class DataTestBase : CompositionTestBase
    {
        public ICompositionContext CreateContainerForData()
        {
            var container = this.CreateContainer(
                new[] { typeof(IDataSpace).GetTypeInfo().Assembly });

            return container;
        }
    }
}