// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenericServiceSourceBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.Lite.Internal
{
    using System;

    using Kephas.Reflection;
    using Kephas.Services;

    internal abstract class GenericServiceSourceBase : ServiceSourceBase
    {
        private readonly Type genericType;

        protected GenericServiceSourceBase(IAppServiceRegistry serviceRegistry, Type genericType)
            : base(serviceRegistry)
        {
            this.genericType = genericType;
        }

        public override bool IsMatch(Type contractType)
        {
            return contractType.IsConstructedGenericOf(this.genericType);
        }
    }
}