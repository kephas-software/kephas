// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppDimensionBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Base implementation of an application dimension.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Dimensions
{
    using System.Collections.Generic;

    /// <summary>
    /// Base implementation of an application dimension.
    /// </summary>
    public abstract class AppDimensionBase : IAppDimension
    {
        /// <summary>
        /// The name.
        /// </summary>
        private readonly string name;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppDimensionBase"/> class.
        /// </summary>
        protected AppDimensionBase()
        {
            // ReSharper disable once DoNotCallOverridableMethodsInConstructor
            this.name = this.GetName();
            this.Elements = new List<IModelElement>();
        }

        /// <summary>
        /// Gets the name of the model element.
        /// </summary>
        /// <value>
        /// The model element name.
        /// </value>
        public virtual string Name
        {
            get { return this.name; }
        }

        /// <summary>
        /// Gets the members of this model element.
        /// </summary>
        /// <value>
        /// The model element members.
        /// </value>
        IEnumerable<IModelElement> IModelElement.Members
        {
            get { return this.Elements; }
        }

        /// <summary>
        /// Gets a value indicating whether this dimension is aggregatable.
        /// </summary>
        /// <value>
        /// <c>true</c> if this dimension is aggregatable; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// A dimension is aggregatable if its members contains parts of an element which at runtime will be
        /// aggregated into one integral element. For example, this helps modelling aplication layers or aspects 
        /// which provide different logical views on the same element.
        /// </remarks>
        public abstract bool IsAggregatable { get; }

        /// <summary>
        /// Gets the dimension elements.
        /// </summary>
        /// <value>
        /// The dimension elements.
        /// </value>
        protected List<IModelElement> Elements { get; private set; }

        /// <summary>
        /// Gets the dimension name.
        /// </summary>
        /// <returns>The dimension name.</returns>
        /// <remarks>
        /// By default, the dimension name is computed from the type name where the Dimension suffix
        /// is trimmed away.
        /// </remarks>
        protected virtual string GetName()
        {
            const string Suffix = "Dimension";
            var dimName = this.GetType().Name;
            if (dimName.EndsWith(Suffix))
            {
                return dimName.Substring(0, dimName.Length - Suffix.Length);
            }

            return dimName;
        }
    }
}