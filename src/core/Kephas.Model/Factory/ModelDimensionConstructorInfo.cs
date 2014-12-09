using System.Reflection;

namespace Kephas.Model.Factory
{
    /// <summary>
    /// Structure providing information about constructing model dimensions.
    /// </summary>
    public class ModelDimensionConstructorInfo : ElementConstructorInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModelDimensionConstructorInfo"/> class.
        /// </summary>
        /// <param name="nativeElement">The native element.</param>
        public ModelDimensionConstructorInfo(MemberInfo nativeElement)
            : base(nativeElement)
        {
        }

        /// <summary>
        /// Gets or sets the model dimension attribute.
        /// </summary>
        /// <value>
        /// The model dimension attribute.
        /// </value>
        public ModelDimensionAttribute ModelDimensionAttribute { get; set; }
    }
}