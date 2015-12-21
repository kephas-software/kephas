namespace Kephas.Model.Runtime.Construction.Annotations
{
    using Kephas.Model.AttributedModel.Behaviors;

    public class RuntimeRequiredAnnotationInfo : RuntimeAnnotationInfoBase<RequiredAttribute>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeRequiredAnnotationInfo"/> class.
        /// </summary>
        /// <param name="runtimeElement">The runtime element.</param>
        public RuntimeRequiredAnnotationInfo(RequiredAttribute runtimeElement)
            : base(runtimeElement)
        {
        }
    }
}