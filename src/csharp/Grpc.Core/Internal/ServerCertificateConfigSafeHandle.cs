#region Copyright notice and license
// Copyright 2015 gRPC authors.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core.Utils;

namespace Grpc.Core.Internal
{
    /// <summary>
    /// grpc_ssl_server_certificate_config from <c>grpc/grpc_security.h</c>
    /// </summary>
    internal class ServerCertificateConfigSafeHandle : SafeHandleZeroIsInvalid
    {
        static readonly NativeMethods Native = NativeMethods.Get();

        private ServerCertificateConfigSafeHandle()
        {
        }

        public static ServerCertificateConfigSafeHandle CreateSslServerCertificateConfig(string pemRootCerts, string[] keyCertPairCertChainArray, string[] keyCertPairPrivateKeyArray)
        {
            GrpcPreconditions.CheckArgument(keyCertPairCertChainArray.Length == keyCertPairPrivateKeyArray.Length);
            /* Creates a grpc_ssl_server_certificate_config object.
             * - pem_roots_cert is the NULL-terminated string containing the PEM encoding of
             *   the client root certificates. This parameter may be NULL if the server does
             *   not want the client to be authenticated with SSL.
             * - pem_key_cert_pairs is an array private key / certificate chains of the
             *   server. This parameter cannot be NULL.
             * - num_key_cert_pairs indicates the number of items in the private_key_files
             *   and cert_chain_files parameters. It must be at least 1.
             * - It is the caller's responsibility to free this object via
             *   grpc_ssl_server_certificate_config_destroy().
             */
            return Native.grpcsharp_ssl_server_certificate_config_create(
                pemRootCerts,
                keyCertPairCertChainArray,
                keyCertPairPrivateKeyArray,
                new UIntPtr((ulong)keyCertPairCertChainArray.Length));
        }

        protected override bool ReleaseHandle()
        {
            /* Should we call destroy here since the user of this object, which is
             *      ServerCredentialsOptionsSafeHandle.CreateSslServerCredentialsOptionsUsingConfig()
             * takes the ownership of this object?
             * Probably not because it throws:
             *     System.AccessViolationException: 'Attempted to read or write protected memory. This is often an indication that other memory is corrupt.'
             */
            // Native.grpcsharp_ssl_server_certificate_config_destroy(handle);
            return true;
        }
    }
}
