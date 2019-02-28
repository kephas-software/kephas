// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICalendar.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the ICalendar interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.Quartz.JobStore.Model
{
    using Kephas.Data.Model.AttributedModel;

    /// <summary>
    /// Interface for calendar.
    /// </summary>
    [NaturalKey(new[] { nameof(InstanceName), nameof(CalendarName) })]
    public interface ICalendar : IEntityBase
    {
        /// <summary>
        /// Gets or sets the name of the calendar.
        /// </summary>
        /// <value>
        /// The name of the calendar.
        /// </value>
        string CalendarName { get; set; }

        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>
        /// The content.
        /// </value>
        byte[] Content { get; set; }
    }

    /// <summary>
    /// A calendar extensions.
    /// </summary>
    public static class CalendarExtensions
    {
        /// <summary>
        /// Gets the Quartz calendar out of the <see cref="ICalendar"/> information.
        /// </summary>
        /// <param name="calendar">The calendar to act on.</param>
        /// <returns>
        /// The Quartz calendar.
        /// </returns>
        public static global::Quartz.ICalendar GetCalendar(this ICalendar calendar)
        {
            return DataContextExtensions.ObjectSerializer.DeSerialize<global::Quartz.ICalendar>(calendar.Content);
        }
    }
}