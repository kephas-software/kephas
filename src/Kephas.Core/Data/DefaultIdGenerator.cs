// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultIdGenerator.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the default identifier generator class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data
{
    using System;

    using Kephas.Composition.AttributedModel;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Services;

    /// <summary>
    /// The default implementation of an identifier generator.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultIdGenerator : IIdGenerator
    {
        /// <summary>
        /// Options for controlling the operation.
        /// </summary>
        private readonly IdGeneratorSettings settings;

        /// <summary>
        /// The maximum random part = 2 ^ 7 (for the last 7 bits).
        /// </summary>
        private readonly int maxRandom;

        /// <summary>
        /// Bit shift value for the timestamp.
        /// </summary>
        private readonly int timestampShift;

        /// <summary>
        /// The synchronize object.
        /// </summary>
        private readonly object syncObject = new object();

        /// <summary>
        /// The last generated identifier.
        /// </summary>
        private long lastGeneratedId;

        /// <summary>
        /// The counter for discriminating values within one timestamp.
        /// </summary>
        private int discriminatorCounter;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultIdGenerator"/> class.
        /// </summary>
        [CompositionConstructor]
        public DefaultIdGenerator()
            : this(new IdGeneratorSettings())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultIdGenerator"/> class.
        /// </summary>
        /// <param name="settings">Options for controlling the operation.</param>
        public DefaultIdGenerator(IdGeneratorSettings settings)
        {
            Requires.NotNull(settings, nameof(settings));

            this.settings = settings;

            this.maxRandom = (1 << this.settings.DiscriminatorBitLength) - 1;
            this.timestampShift = this.settings.DiscriminatorBitLength + this.settings.NamespaceIdentifierBitLength;
        }

        /// <summary>
        /// Generates the 52-bit identifier.
        /// This is due to the fact that JavaScript can read accurately only integers up to 2^53,
        /// values greater than that will get truncated.
        /// See http://stackoverflow.com/questions/5353388/javascript-parsing-int64 for more info.
        /// </summary>
        /// <returns>
        /// The unique identifier.
        /// </returns>
        public long GenerateId()
        {
            lock (this.syncObject)
            {
                var generatedId = this.GenerateIdCore();
                while (generatedId == this.lastGeneratedId)
                {
                    generatedId = this.GenerateIdCore();
                }

                this.lastGeneratedId = generatedId;
                return generatedId;
            }
        }

        /// <summary>
        ///   Gets timestamp from ID.
        /// </summary>
        /// <param name="id">The ID.</param>
        /// <returns>
        ///   The <see cref="DateTimeOffset"/>.
        /// </returns>
        internal DateTimeOffset GetTimestamp(long id)
        {
            var milliseconds = this.GetTimestampPart(id);

            var date = new DateTimeOffset(this.settings.StartEpoch.DateTime, this.settings.StartEpoch.Offset).AddMilliseconds(milliseconds);
            return date;
        }

        /// <summary>
        ///   Generates the discriminator part for the relative timestamp.
        /// </summary>
        /// <param name="relativeTimestamp">The relative timestamp.</param>
        /// <remarks>The relative timestamp may be adjusted to avoid collisions.</remarks>
        /// <returns>
        ///   The discriminator part of the ID.
        /// </returns>
        protected int GenerateDiscriminator(ref long relativeTimestamp)
        {
            var timestampPartFromLastId = this.GetTimestampPart(this.lastGeneratedId);

            // The rare case when more than <MaxRandom> IDs were generated in one millisecond,
            // than the next generated IDs within the same millisecond will have the timestamp of the next (incremented millisecond)
            if (relativeTimestamp < timestampPartFromLastId)
            {
                relativeTimestamp = timestampPartFromLastId;
            }

            // If the milliseconds for which the number is taken are different from the milliseconds from the last generated ID,
            // reset counter, there is no need to discriminate anymore.
            if (relativeTimestamp != timestampPartFromLastId)
            {
                this.discriminatorCounter = 0;
            }

            var discriminatorPart = this.discriminatorCounter++;

            if (this.discriminatorCounter > this.maxRandom)
            {
                relativeTimestamp++;
                discriminatorPart = 0;
                this.discriminatorCounter = 0;
            }

            return discriminatorPart;
        }

        /// <summary>
        /// Gets the identifier part for the namespace.
        /// </summary>
        /// <returns>
        /// The namespace identifier part.
        /// </returns>
        protected virtual int GenerateNamespace()
        {
            // TODO - when we need to generate unique IDs on different machine,
            // then use a service to get the namespace ID on the current machine
            return 0;
        }

        /// <summary>
        /// Generates the 52-bit identifier.
        /// </summary>
        /// <returns>
        /// The generated ID.
        /// </returns>
        private long GenerateIdCore()
        {
            var timestampPart = this.GenerateTimestamp();

            // 7-bit discriminator value
            var discriminatorPart = this.GenerateDiscriminator(ref timestampPart);

            // 3-bit namespace identifier
            var namespacePart = this.GenerateNamespace();

            // shifting timestamp (milliseconds from start epoch) to the left to get 42-bit timestamp part of the Id
            var shiftedTimestampPart = timestampPart << this.timestampShift;

            var shiftedNamespacePart = namespacePart << this.settings.DiscriminatorBitLength;

            // 52-bit ID consisting of 42-bit timestamp (milliseconds from start epoch), 3-bit machine identifier and 7-bit random value
            var result = shiftedTimestampPart + shiftedNamespacePart + discriminatorPart;

            return result;
        }

        /// <summary>
        ///   Gets the milliseconds part from identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>
        ///   Milliseconds from the start epoch taken from the given ID.
        /// </returns>
        private long GetTimestampPart(long id)
        {
            return id >> this.timestampShift;
        }

        /// <summary>
        ///   Gets the relative timestamp.
        /// </summary>
        /// <returns>
        ///   The timestamp relative to the <see cref="IdGeneratorSettings.StartEpoch"/>.
        /// </returns>
        private long GenerateTimestamp()
        {
            var now = DateTimeOffset.Now.ToUniversalTime();
            return (long)(now - this.settings.StartEpoch).TotalMilliseconds;
        }
    }
}