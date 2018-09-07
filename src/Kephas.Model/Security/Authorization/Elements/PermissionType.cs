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
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    using Kephas.Collections;
    using Kephas.Model.Construction;
    using Kephas.Model.Elements;
    using Kephas.Model.Resources;
    using Kephas.Model.Security.Authorization.AttributedModel;
    using Kephas.Reflection;
    using Kephas.Runtime;
    using Kephas.Security.Authorization;
    using Kephas.Security.Authorization.AttributedModel;

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
        public IEnumerable<IPermissionType> GrantedPermissions { get; private set; }

        /// <summary>
        /// Gets the scoping.
        /// </summary>
        /// <value>
        /// The scoping.
        /// </value>
        public Scoping Scoping { get; private set; }

        /// <summary>
        /// Called when the construction is complete.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        protected override void OnCompleteConstruction(IModelConstructionContext constructionContext)
        {
            base.OnCompleteConstruction(constructionContext);

            this.GrantedPermissions = new ReadOnlyCollection<IPermissionType>(this.BaseMixins.OfType<IPermissionType>().ToList());
            var permTypeAttr = this.Parts
                .OfType<ITypeInfo>()
                .Select(p => p.GetAttribute<PermissionTypeAttribute>())
                .FirstOrDefault();
            this.Scoping = permTypeAttr?.Scoping ?? Scoping.None;
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

            var grantedPermNames = new HashSet<string>(
                grantedAttrs
                    .Where(a => a.Permissions != null)
                    .SelectMany(a => a.Permissions));
            var grantedPermTypes = new HashSet<ITypeInfo>(
                grantedAttrs
                    .Where(a => a.Permissions != null)
                    .SelectMany(a => a.PermissionTypes)
                    .Select(t => t.AsRuntimeTypeInfo()));

            var grantedTypes = constructionContext.ModelSpace.Classifiers
                                .OfType<IPermissionType>()
                                .Where(c => grantedPermNames.Any(n => c.Name == n || c.FullName == n)
                                            || grantedPermTypes.Any(t => c.Parts.Contains(t)))
                                .Cast<ITypeInfo>();
            baseTypes = baseTypes.AddRange(grantedTypes).Distinct().ToList();

            var notFoundPermNames = grantedPermNames
                .Where(n => baseTypes.All(t => t.Name != n && t.FullName != n))
                .ToList();
            var baseClassifiers = baseTypes.OfType<IClassifier>().ToList();
            var notFoundPermTypes = grantedPermTypes
                .Where(t => baseClassifiers.All(c => !c.Parts.Contains(t)))
                .ToList();

            if (notFoundPermNames.Count > 0 || notFoundPermTypes.Count > 0)
            {
                notFoundPermNames.AddRange(notFoundPermTypes.Select(t => t.FullName));
                throw new ModelConstructionException(string.Format(Strings.PermissionType_MissingGrantedPermissions_Exception, this.FullName, string.Join(", ", notFoundPermNames)));
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
        protected override IClassifier ComputeBaseClassifier(IModelConstructionContext constructionContext, IEnumerable<ITypeInfo> baseTypes)
        {
            return null;
        }
    }
}