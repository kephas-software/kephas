// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GroupMatcherExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the group matcher extensions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.Quartz.Linq
{
    using System;
    using System.Linq.Expressions;

    using global::Quartz.Impl.Matchers;
    using global::Quartz.Util;

    using Kephas.Scheduling.Quartz.JobStore.Model;

    internal static class GroupMatcherExtensions
    {
        public static Expression<Func<T, bool>> ToFilterExpression<T, TMatcher>(this GroupMatcher<TMatcher> matcher, string instanceName)
            where TMatcher : Key<TMatcher>
            where T : IGroupedEntityBase
        {
            if (StringOperator.Equality.Equals(matcher.CompareWithOperator))
            {
                return e => e.InstanceName == instanceName && e.Group == matcher.CompareToValue;
            }

            if (StringOperator.Contains.Equals(matcher.CompareWithOperator))
            {
                return e => e.InstanceName == instanceName && e.Group.Contains(matcher.CompareToValue);
            }

            if (StringOperator.EndsWith.Equals(matcher.CompareWithOperator))
            {
                return e => e.InstanceName == instanceName && e.Group.EndsWith(matcher.CompareToValue);
            }

            if (StringOperator.StartsWith.Equals(matcher.CompareWithOperator))
            {
                return e => e.InstanceName == instanceName && e.Group.StartsWith(matcher.CompareToValue);
            }

            if (StringOperator.Anything.Equals(matcher.CompareWithOperator))
            {
                return e => e.InstanceName == instanceName;
            }

            throw new ArgumentOutOfRangeException("Don't know how to translate " + matcher.CompareWithOperator +
                                                  " into filter expression");
        }
    }
}