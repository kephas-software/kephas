using System;

namespace Kephas.Model
{
    /// <summary>
    /// Attribute for identifying dimensions.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    public class ModelDimensionAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModelDimensionAttribute" /> class.
        /// </summary>
        /// <param name="index">The index.</param>
        public ModelDimensionAttribute(int index)
            : this(false, index)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelDimensionAttribute" /> class.
        /// </summary>
        /// <param name="isAggregatable">If set to <c>true</c> the dimension is aggregatable.</param>
        /// <param name="index">The index.</param>
        protected ModelDimensionAttribute(bool isAggregatable, int index)
        {
            this.IsAggregatable = isAggregatable;
            Index = index;
        }

        /// <summary>
        /// Gets a value indicating whether the dimension is aggregatable.
        /// </summary>
        /// <value>
        /// <c>true</c> if the dimension is aggregatable; otherwise, <c>false</c>.
        /// </value>
        public bool IsAggregatable { get; private set; }

        /// <summary>
        /// Gets the dimension index.
        /// </summary>
        /// <value>
        /// The dimension index.
        /// </value>
        public int Index { get; private set; }
    }
}