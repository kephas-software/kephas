// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BehaviorValue.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Default implementation of a generic behavior value.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Behavior
{
    using Kephas.Dynamic;

    /// <summary>
    /// Class providing convenience methods for behavior values.
    /// </summary>
    public static class BehaviorValue
    {
        /// <summary>
        /// A behavior value representing the boolean value <c>false</c>.
        /// </summary>
        public static readonly BehaviorValue<bool> False = new BehaviorValue<bool>(false);

        /// <summary>
        /// A behavior value representing the boolean value <c>true</c>.
        /// </summary>
        public static readonly BehaviorValue<bool> True = new BehaviorValue<bool>(true);

        /// <summary>
        /// The key for the dynamic "Reason" property.
        /// </summary>
        private static readonly string ReasonKey = "Reason";

        /// <summary>
        /// A TValue extension method that converts a value to a behavior value.
        /// </summary>
        /// <typeparam name="TValue">Type of the value.</typeparam>
        /// <param name="value">The value to act on.</param>
        /// <returns>
        /// A BehaviorValue{TValue} representing the provided value.
        /// </returns>
        public static IBehaviorValue<TValue> AsBehaviorValue<TValue>(this TValue value)
        {
            return new BehaviorValue<TValue>(value);
        }

        /// <summary>
        /// Sets a reason for the given BehaviorValue{TValue}.
        /// </summary>
        /// <typeparam name="TValue">Type of the value.</typeparam>
        /// <param name="behavior">The value to act on.</param>
        /// <param name="reason">  The reason.</param>
        /// <returns>
        /// The provided BehaviorValue{TValue}.
        /// </returns>
        public static IBehaviorValue<TValue> WithReason<TValue>(this IBehaviorValue<TValue> behavior, string reason)
        {
            if (behavior != null)
            {
                behavior[ReasonKey] = reason;
            }

            return behavior;
        }

        /// <summary>
        /// Sets a reason for the given BehaviorValue{TValue}.
        /// </summary>
        /// <param name="behavior">The value to act on.</param>
        /// <returns>
        /// The reason, if one was provided, otherwise <c>null</c>.
        /// </returns>
        public static string GetReason(this IBehaviorValue behavior)
        {
            if (behavior == null)
            {
                return null;
            }

            var value = behavior[ReasonKey];
            var reason = value as string;
            if (reason != null)
            {
                return reason;
            }

            return value == Undefined.Value ? null : value.ToString();
        }
    }

    /// <summary>
    /// Default implementation of a generic behavior value.
    /// </summary>
    /// <typeparam name="TValue">Type of the value.</typeparam>
    public class BehaviorValue<TValue> : Expando, IBehaviorValue<TValue>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BehaviorValue{TValue}"/> class.
        /// </summary>
        /// <param name="value">The behavior value.</param>
        public BehaviorValue(TValue value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets the behavior value.
        /// </summary>
        /// <value>
        /// The behavior value.
        /// </value>
        object IBehaviorValue.Value => this.Value;

        /// <summary>
        /// Gets or sets the behavior value.
        /// </summary>
        /// <value>
        /// The behavior value.
        /// </value>
        public TValue Value { get; protected set; }
    }
}