// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExportFactoryServiceSource.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the export service source class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Internal
{
    using System;
    using System.Reflection;

    using Kephas.Composition;
    using Kephas.Composition.ExportFactories;
    using Kephas.Reflection;

    internal class ExportFactoryServiceSource : IServiceSource
    {
        private static readonly MethodInfo GetServiceMethod =
            ReflectionHelper.GetGenericMethodOf(_ => ExportFactoryServiceSource.GetService<string>(null));

        private readonly IServiceProvider parent;

        public ExportFactoryServiceSource(IServiceProvider parent)
        {
            this.parent = parent;
        }

        public object GetService(Type serviceType)
        {
            var innerType = serviceType.GetGenericArguments()[0];
            var getService = GetServiceMethod.MakeGenericMethod(innerType);
            return getService.Call(null, this.parent);
        }

        public bool IsMatch(Type contractType)
        {
            return contractType.IsConstructedGenericOf(typeof(IExportFactory<>));
        }

        private static IExportFactory<T> GetService<T>(IServiceProvider parent)
            where T : class
        {
            return new ExportFactory<T>(() => parent.GetService<T>());
        }
    }
}