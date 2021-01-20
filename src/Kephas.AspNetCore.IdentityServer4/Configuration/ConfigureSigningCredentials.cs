// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigureSigningCredentials.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.IdentityServer4.Configuration
{
    using System;
    using System.IO;
    using System.Security.Cryptography;

    using Kephas.AspNetCore.IdentityServer4.Options;
    using Kephas.Cryptography.X509Certificates;
    using Kephas.Serialization;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Tokens;

    internal class ConfigureSigningCredentials : IConfigureOptions<ApiAuthorizationOptions>
    {
        private const string DefaultTempKeyRelativePath = "obj/tempkey.json";
        private readonly ICertificateProvider certificateProvider;
        private readonly ISerializationService serializationService;
        private readonly IdentityServerSettings configuration;
        private readonly ILogger<ConfigureSigningCredentials> logger;

        public ConfigureSigningCredentials(
            IdentityServerSettings configuration,
            ICertificateProvider certificateProvider,
            ISerializationService serializationService,
            ILogger<ConfigureSigningCredentials> logger)
        {
            this.certificateProvider = certificateProvider;
            this.serializationService = serializationService;
            this.configuration = configuration;
            this.logger = logger;
        }

        public void Configure(ApiAuthorizationOptions options)
        {
            var key = this.LoadKey();
            if (key != null)
            {
                options.SigningCredential = key;
            }
        }

        public SigningCredentials? LoadKey()
        {
            // We can't know for sure if there was a configuration section explicitly defined.
            // Check if the current configuration has any children and avoid failing if that's the case.
            // This will avoid failing when no configuration has been specified but will still fail if partial data
            // was defined.
            if (this.configuration.Key == null)
            {
                return null;
            }

            var key = this.configuration.Key;
            var keyType = key.Type ?? KeySettings.ProductionType;

            switch (keyType)
            {
                case KeySettings.DevelopmentType:
                    var developmentKeyPath = Path.Combine(Directory.GetCurrentDirectory(), key.FilePath ?? DefaultTempKeyRelativePath);
                    var createIfMissing = key.Persisted ?? true;
                    this.logger.LogInformation($"Loading development key at '{developmentKeyPath}'.");
                    var developmentKey = new RsaSecurityKey(LoadDevelopment(this.serializationService, developmentKeyPath, createIfMissing))
                    {
                        KeyId = "Development",
                    };
                    return new SigningCredentials(developmentKey, "RS256");
                case KeySettings.ProductionType:
                    if (key.Certificate == null)
                    {
                        throw new InvalidOperationException($"No certificate provided for production mode.");
                    }

                    var cert = this.certificateProvider.TryGetCertificate(key.Certificate);
                    if (cert == null)
                    {
                        throw new InvalidOperationException($"The provider could not find certificate '{key.Certificate ?? "(null)"}'. Check the spelling and the certificate settings store.");
                    }

                    this.logger.LogInformation($"Loading production key from certificate '{key.Certificate}'.");
                    return new SigningCredentials(new X509SecurityKey(cert), "RS256");
                default:
                    throw new InvalidOperationException($"Invalid key type '{key.Type ?? "(null)"}'.");
            }
        }

        // for testing purposes only
        internal virtual DateTimeOffset GetCurrentTime() => DateTimeOffset.UtcNow;

        public static RSA LoadDevelopment(ISerializationService serializationService, string path, bool createIfMissing)
        {
            var fileExists = File.Exists(path);
            if (!fileExists && !createIfMissing)
            {
                throw new InvalidOperationException($"Couldn't find the file '{path}' and creation of a development key was not requested.");
            }

            if (fileExists)
            {
                var rsa = serializationService.JsonDeserialize<RSAKeyParameters>(File.ReadAllText(path));
                return rsa.GetRSA();
            }
            else
            {
                var parameters = RSAKeyParameters.Create();
                var directory = Path.GetDirectoryName(path);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                File.WriteAllText(path, serializationService.JsonSerialize(parameters, c => c.IncludeTypeInfo(false)));
                return parameters.GetRSA();
            }
        }

        private class RSAKeyParameters
        {
            public string D { get; set; }
            public string DP { get; set; }
            public string DQ { get; set; }
            public string E { get; set; }
            public string IQ { get; set; }
            public string M { get; set; }
            public string P { get; set; }
            public string Q { get; set; }

            public static RSAKeyParameters Create()
            {
                using (var rsa = RSA.Create())
                {
                    if (rsa is RSACryptoServiceProvider rSACryptoServiceProvider && rsa.KeySize < 2048)
                    {
                        rsa.KeySize = 2048;
                        if (rsa.KeySize < 2048)
                        {
                            throw new InvalidOperationException("We can't generate an RSA key with at least 2048 bits. Key generation is not supported in this system.");
                        }
                    }

                    return GetParameters(rsa);
                }
            }

            public static RSAKeyParameters GetParameters(RSA key)
            {
                var result = new RSAKeyParameters();
                var rawParameters = key.ExportParameters(includePrivateParameters: true);

                if (rawParameters.D != null)
                {
                    result.D = Convert.ToBase64String(rawParameters.D);
                }

                if (rawParameters.DP != null)
                {
                    result.DP = Convert.ToBase64String(rawParameters.DP);
                }

                if (rawParameters.DQ != null)
                {
                    result.DQ = Convert.ToBase64String(rawParameters.DQ);
                }

                if (rawParameters.Exponent != null)
                {
                    result.E = Convert.ToBase64String(rawParameters.Exponent);
                }

                if (rawParameters.InverseQ != null)
                {
                    result.IQ = Convert.ToBase64String(rawParameters.InverseQ);
                }

                if (rawParameters.Modulus != null)
                {
                    result.M = Convert.ToBase64String(rawParameters.Modulus);
                }

                if (rawParameters.P != null)
                {
                    result.P = Convert.ToBase64String(rawParameters.P);
                }

                if (rawParameters.Q != null)
                {
                    result.Q = Convert.ToBase64String(rawParameters.Q);
                }

                return result;
            }

            public RSA GetRSA()
            {
                var parameters = new RSAParameters();
                if (D != null)
                {
                    parameters.D = Convert.FromBase64String(D);
                }

                if (DP != null)
                {
                    parameters.DP = Convert.FromBase64String(DP);
                }

                if (DQ != null)
                {
                    parameters.DQ = Convert.FromBase64String(DQ);
                }

                if (E != null)
                {
                    parameters.Exponent = Convert.FromBase64String(E);
                }

                if (IQ != null)
                {
                    parameters.InverseQ = Convert.FromBase64String(IQ);
                }

                if (M != null)
                {
                    parameters.Modulus = Convert.FromBase64String(M);
                }

                if (P != null)
                {
                    parameters.P = Convert.FromBase64String(P);
                }

                if (Q != null)
                {
                    parameters.Q = Convert.FromBase64String(Q);
                }

                var rsa = RSA.Create();
                rsa.ImportParameters(parameters);

                return rsa;
            }
        }
    }
}
