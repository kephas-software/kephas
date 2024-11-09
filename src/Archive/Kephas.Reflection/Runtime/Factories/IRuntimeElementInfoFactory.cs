// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRuntimeElementInfoFactory.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Runtime.Factories
{
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;

    using Kephas.Logging;
    using Kephas.Services;

    /// <summary>
    /// Marker interface for <see cref="IRuntimeElementInfo"/> factories.
    /// </summary>
    public interface IRuntimeElementInfoFactory
    {
        /// <summary>
        /// Tries to create the runtime element information for the provided raw reflection element.
        /// </summary>
        /// <param name="registry">The root type registry.</param>
        /// <param name="reflectInfo">The raw reflection element.</param>
        /// <param name="position">Optional. The position in the declaring container.</param>
        /// <param name="logger">Optional. The logger.</param>
        /// <returns>
        /// The matching runtime type information type, or <c>null</c> if a runtime type info could not be created.
        /// </returns>
        IRuntimeElementInfo? TryCreateElementInfo(IRuntimeTypeRegistry registry, MemberInfo reflectInfo, int position = -1, ILogger? logger = null);
    }

    /// <summary>
    /// Marker interface for <see cref="IRuntimeElementInfo"/> factories.
    /// </summary>
    /// <typeparam name="TElement">The runtime element type.</typeparam>
    /// <typeparam name="TReflectElement">The reflection element type.</typeparam>
    /// <remarks>
    /// Although this interface is marked as application service,
    /// its implementations are not retrieved through dependency injection/composition
    /// as they are required earlier in the application lifecycle. This is provided as
    /// a simple mean of collecting meta information about this kind of services.
    /// </remarks>
    [SingletonAppServiceContract(ContractType = typeof(IRuntimeElementInfoFactory), AllowMultiple = true)]
    public interface IRuntimeElementInfoFactory<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] out TElement, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] in TReflectElement> : IRuntimeElementInfoFactory
        where TElement : IRuntimeElementInfo
        where TReflectElement : MemberInfo
    {
        /// <summary>
        /// Tries to create the runtime element information for the provided raw reflection element.
        /// </summary>
        /// <param name="registry">The root type registry.</param>
        /// <param name="reflectInfo">The raw reflection element.</param>
        /// <param name="position">Optional. The position in the declaring container.</param>
        /// <param name="logger">Optional. The logger.</param>
        /// <returns>
        /// The matching runtime type information type, or <c>null</c> if a runtime type info could not be created.
        /// </returns>
        TElement? TryCreateElementInfo(IRuntimeTypeRegistry registry, TReflectElement reflectInfo, int position = -1, ILogger? logger = null);
    }

    /// <summary>
    /// Base class for <see cref="IRuntimeTypeInfoFactory"/> implementations.
    /// </summary>
    /// <typeparam name="TElement">The runtime element type.</typeparam>
    /// <typeparam name="TReflectElement">The reflection element type.</typeparam>
    public abstract class
        RuntimeElementInfoFactoryBase<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TElement, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TReflectElement> : IRuntimeElementInfoFactory<TElement, TReflectElement>
        where TElement : class, IRuntimeElementInfo
        where TReflectElement : MemberInfo
    {
        /// <summary>
        /// Tries to create the runtime element information for the provided raw reflection element.
        /// </summary>
        /// <param name="registry">The root type registry.</param>
        /// <param name="reflectInfo">The raw reflection element.</param>
        /// <param name="position">Optional. The position in the declaring container.</param>
        /// <param name="logger">Optional. The logger.</param>
        /// <returns>
        /// The matching runtime type information type, or <c>null</c> if a runtime type info could not be created.
        /// </returns>
        public abstract TElement? TryCreateElementInfo(IRuntimeTypeRegistry registry, TReflectElement reflectInfo, int position = -1, ILogger? logger = null);

        /// <summary>
        /// Tries to create the runtime element information for the provided raw reflection element.
        /// </summary>
        /// <param name="registry">The root type registry.</param>
        /// <param name="reflectInfo">The raw reflection element.</param>
        /// <param name="position">Optional. The position in the declaring container.</param>
        /// <param name="logger">Optional. The logger.</param>
        /// <returns>
        /// The matching runtime type information type, or <c>null</c> if a runtime type info could not be created.
        /// </returns>
        public virtual IRuntimeElementInfo? TryCreateElementInfo(IRuntimeTypeRegistry registry, MemberInfo reflectInfo, int position = -1, ILogger? logger = null)
            => this.TryCreateElementInfo(registry, (TReflectElement) reflectInfo, position);
    }
}