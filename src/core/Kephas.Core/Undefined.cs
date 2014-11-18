// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Undefined.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Type used for providing undefined values.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas
{
    /// <summary>
    /// Type used for providing undefined values.
    /// </summary>
    public class Undefined
    {
        /// <summary>
        /// The single value of the <see cref="Undefined"/> type.
        /// </summary>
        public static readonly Undefined Value = new Undefined();

        /// <summary>
        /// Prevents a default instance of the <see cref="Undefined"/> class from being created.
        /// </summary>
        private Undefined()
        {
        } 
    }
}