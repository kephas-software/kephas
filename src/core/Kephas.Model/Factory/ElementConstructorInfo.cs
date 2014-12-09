using System.Reflection;

namespace Kephas.Model.Factory
{
    /// <summary>
    /// Structure providing information about constructing elements.
    /// </summary>
    public class ElementConstructorInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ElementConstructorInfo"/> class.
        /// </summary>
        /// <param name="nativeElement">The native element.</param>
        public ElementConstructorInfo(MemberInfo nativeElement)
        {
            this.NativeElement = nativeElement;
            this.NativeTypeInfo = nativeElement as TypeInfo;
        }

        /// <summary>
        /// Gets the native element.
        /// </summary>
        /// <value>
        /// The native element.
        /// </value>
        public MemberInfo NativeElement { get; private set; }

        /// <summary>
        /// Gets the native type information.
        /// </summary>
        /// <value>
        /// The native type information.
        /// </value>
        public TypeInfo NativeTypeInfo { get; private set; }
    }
}