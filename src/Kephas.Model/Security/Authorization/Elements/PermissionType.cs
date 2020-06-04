// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PermissionType.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the permission type class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Security.Authorization.Elements
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    using Kephas.Collections;
    using Kephas.Model.Construction;
    using Kephas.Model.Elements;
    using Kephas.Model.Resources;
    using Kephas.Reflection;
    using Kephas.Security.Authorization;
    using Kephas.Security.Authorization.AttributedModel;
    using Kephas.Security.Authorization.Reflection;

    /// <summary>
    /// Classifier class for permissions.
    /// </summary>
    public class PermissionType : ClassifierBase<IPermissionType>, IPermissionType
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PermissionType" /> class.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="name">The name.</param>
        public PermissionType(IModelConstructionContext constructionContext, string name)
            : base(constructionContext, name)
        {
        }

        /// <summary>
        /// Gets the granted permissions.
        /// </summary>
        /// <remarks>
        /// When this permission is granted, the permissions granted by this are also granted.
        /// Using this mechanism one can define a hierarchy of permissions.
        /// </remarks>
        /// <value>
        /// The granted permissions.
        /// </value>
        public IEnumerable<IPermissionInfo> GrantedPermissions { get; private set; }

        /// <summary>
        /// Gets the required permissions to access this permission.
        /// </summary>
        /// <value>
        /// The required permissions.
        /// </value>
        public IEnumerable<IPermissionInfo> RequiredPermissions { get; private set; }

        /// <summary>
        /// Gets the scoping.
        /// </summary>
        /// <value>
        /// The scoping.
        /// </value>
        public Scoping Scoping { get; private set; }

        /// <summary>
        /// Gets the token name.
        /// </summary>
        public string TokenName { get; private set; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return $"[{this.TokenName}] {base.ToString()}";
        }

        /// <summary>
        /// Called when the construction is complete.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        protected override void OnCompleteConstruction(IModelConstructionContext constructionContext)
        {
            base.OnCompleteConstruction(constructionContext);

            var permInfoPart = this.Parts
                .OfType<IPermissionInfo>()
                .FirstOrDefault();
            this.Scoping = permInfoPart?.Scoping ?? Scoping.Global;
            this.TokenName = permInfoPart?.TokenName ?? this.Name;

            this.GrantedPermissions = this.BaseMixins
                .OfType<IPermissionInfo>()
                .ToList()
                .AsReadOnly();
            this.RequiredPermissions = this.GetAttributes<RequiresPermissionAttribute>()
                .SelectMany(
                    attr => attr.PermissionTypes
                        .Select(
                            t => constructionContext.ModelSpace.TryGetClassifier(
                                t.AsRuntimeTypeInfo(constructionContext?.AmbientServices?.TypeRegistry),
                                constructionContext)))
                .OfType<IPermissionInfo>()
                .Distinct()
                .ToList()
                .AsReadOnly();
        }

        /// <summary>
        /// Calculates the base types.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="parts">The parts.</param>
        /// <returns>
        /// The calculated base types.
        /// </returns>
        protected override IList<ITypeInfo> ComputeBaseTypes(IModelConstructionContext constructionContext, IList<ITypeInfo> parts)
        {
            var grantedAttrs = this.GetAttributes<GrantsPermissionAttribute>().ToList();
            var baseTypes = base.ComputeBaseTypes(constructionContext, parts);
            if (grantedAttrs.Count == 0)
            {
                return baseTypes;
            }

            var grantedPermTypes = new HashSet<ITypeInfo>(
                grantedAttrs
                    .Where(a => a.PermissionTypes != null)
                    .SelectMany(a => a.PermissionTypes)
                    .Select(t => t.AsRuntimeTypeInfo(constructionContext?.AmbientServices?.TypeRegistry)));

            var grantedTypes = constructionContext.ModelSpace.Classifiers
                                .OfType<IPermissionType>()
                                .Where(c => grantedPermTypes.Any(t => c.Parts.Contains(t)))
                                .Cast<ITypeInfo>();
            baseTypes = baseTypes.AddRange(grantedTypes).Distinct().ToList();

            var baseClassifiers = baseTypes.OfType<IClassifier>().ToList();
            var notFoundPermTypes = grantedPermTypes
                .Where(t => baseClassifiers.All(c => !c.Parts.Contains(t)))
                .ToList();

            if (notFoundPermTypes.Count > 0)
            {
                throw new ModelConstructionException(string.Format(Strings.PermissionType_MissingGrantedPermissions_Exception, this.FullName, string.Join(", ", notFoundPermTypes.Select(t => t.FullName))));
            }

            return baseTypes;
        }

        /// <summary>
        /// Calculates the base mixins.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="baseTypes">List of base types.</param>
        /// <returns>
        /// The calculated base mixins.
        /// </returns>
        protected override IEnumerable<IClassifier> ComputeBaseMixins(IModelConstructionContext constructionContext, IEnumerable<ITypeInfo> baseTypes)
        {
            return new ReadOnlyCollection<IClassifier>(baseTypes.OfType<IClassifier>().ToList());
        }

        /// <summary>
        /// Calculates the base classifier.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="baseTypes">List of base types.</param>
        /// <returns>
        /// The calculated base classifier.
        /// </returns>
        protected override IClassifier? ComputeBaseClassifier(IModelConstructionContext constructionContext, IEnumerable<ITypeInfo> baseTypes)
        {
            return null;
        }
    }
}