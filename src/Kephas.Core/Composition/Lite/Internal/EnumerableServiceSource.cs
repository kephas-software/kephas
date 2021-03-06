﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumerableServiceSource.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the enumerable service source class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Lite.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using Kephas;
    using Kephas.Reflection;
    using Kephas.Runtime;

    /// <summary>
    /// An enumerable service source.
    /// </summary>
    internal class EnumerableServiceSource : ServiceSourceBase
    {
        private static readonly MethodInfo GetServiceMethod =
            ReflectionHelper.GetGenericMethodOf(_ => GetService<string>(null, null));

        public EnumerableServiceSource(IServiceRegistry serviceRegistry, IRuntimeTypeRegistry typeRegistry)
            : base(serviceRegistry, typeRegistry)
        {
        }

        public override bool IsMatch(Type contractType)
        {
            return contractType.IsConstructedGenericOf(typeof(IEnumerable<>));
        }

        public override IEnumerable<(IServiceInfo serviceInfo, Func<object> factory)> GetServiceDescriptors(
            IAmbientServices parent,
            Type serviceType)
        {
            var innerType = serviceType.GetGenericArguments()[0];
            return this.GetServiceDescriptors(parent, innerType, null);
        }

        public override object GetService(IAmbientServices parent, Type serviceType)
        {
            var descriptors = this.GetServiceDescriptors(parent, serviceType);
            var itemType = serviceType.GetGenericArguments()[0];
            var getService = GetServiceMethod.MakeGenericMethod(itemType);
            return getService.Call(null, parent, descriptors);
        }

        private static IEnumerable<T> GetService<T>(IServiceProvider parent, IEnumerable<(IServiceInfo serviceInfo, Func<object> factory)> descriptors)
            where T : class
        {
            foreach (var descriptor in descriptors)
            {
                yield return (T)descriptor.factory();
            }
        }
    }
}