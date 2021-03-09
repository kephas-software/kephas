// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeviceFlowStoreService.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.IdentityServer4.Stores
{
    using System.Linq;
    using System.Threading.Tasks;

    using global::IdentityServer4.Models;
    using Kephas.Logging;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// A repository based service for device flow store.
    /// </summary>
    [OverridePriority(Priority.Lowest)]
    public class DeviceFlowStoreService : Loggable, IDeviceFlowStoreService
    {
        private readonly IIdentityRepository repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceFlowStoreService"/> class.
        /// </summary>
        /// <param name="repository">The identity repository.</param>
        /// <param name="logManager">Optional. The log manager.</param>
        public DeviceFlowStoreService(IIdentityRepository repository, ILogManager? logManager = null)
            : base(logManager)
        {
            this.repository = repository;
        }

        /// <summary>Stores the device authorization request.</summary>
        /// <param name="deviceCode">The device code.</param>
        /// <param name="userCode">The user code.</param>
        /// <param name="data">The data.</param>
        /// <returns>The asynchronous result.</returns>
        public Task StoreDeviceAuthorizationAsync(string deviceCode, string userCode, DeviceCode data)
            => this.repository.CreateAsync(new InMemoryDeviceAuthorization(deviceCode, userCode, data), deviceCode, default);

        public Task<DeviceCode> FindByUserCodeAsync(string userCode)
            => this.repository.QueryAsync<InMemoryDeviceAuthorization, DeviceCode>(
                (r, q, ct) =>
                    Task.FromResult(q.FirstOrDefault(d => d.UserCode == userCode)?.Data),
                default);

        /// <summary>Finds device authorization by device code.</summary>
        /// <param name="deviceCode">The device code.</param>
        /// <returns>The asynchronous result yielding the device information.</returns>
        public async Task<DeviceCode> FindByDeviceCodeAsync(string deviceCode)
            => (await this.repository
                    .FindByIdAsync<InMemoryDeviceAuthorization>(deviceCode, default)
                    .PreserveThreadContext())?.Data;

        /// <summary>Updates device authorization, searching by user code.</summary>
        /// <param name="userCode">The user code.</param>
        /// <param name="data">The data.</param>
        /// <returns>The asynchronous result.</returns>
        public async Task UpdateByUserCodeAsync(string userCode, DeviceCode data)
        {
            var item = await this.repository.QueryAsync<InMemoryDeviceAuthorization, InMemoryDeviceAuthorization>(
                    (r, q, ct) => Task.FromResult(q.FirstOrDefault(d => d.UserCode == userCode)),
                    default).PreserveThreadContext();

            if (item != null)
            {
                await this.repository.UpdateAsync(new InMemoryDeviceAuthorization(item.DeviceCode, userCode, data), item.DeviceCode, default).PreserveThreadContext();
            }
        }

        /// <summary>
        /// Removes the device authorization, searching by device code.
        /// </summary>
        /// <param name="deviceCode">The device code.</param>
        /// <returns>The asynchronous result.</returns>
        public async Task RemoveByDeviceCodeAsync(string deviceCode)
        {
            var item = await this.repository
                .FindByIdAsync<InMemoryDeviceAuthorization>(deviceCode, default)
                .PreserveThreadContext();

            if (item != null)
            {
                await this.repository.DeleteAsync(item, deviceCode, default).PreserveThreadContext();
            }
        }

        private class InMemoryDeviceAuthorization
        {
            public InMemoryDeviceAuthorization(string deviceCode, string userCode, DeviceCode data)
            {
                DeviceCode = deviceCode;
                UserCode = userCode;
                Data = data;
            }

            public string DeviceCode { get; }
            public string UserCode { get; }
            public DeviceCode Data { get; set; }
        }
    }
}