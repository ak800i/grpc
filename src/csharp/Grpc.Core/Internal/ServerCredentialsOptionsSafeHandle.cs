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
    /// grpc_ssl_server_credentials_options from <c>grpc/grpc_security.h</c>
    /// </summary>
    internal class ServerCredentialsOptionsSafeHandle : SafeHandleZeroIsInvalid
    {
        static readonly NativeMethods Native = NativeMethods.Get();

        private ServerCredentialsOptionsSafeHandle()
        {
        }

        public static ServerCredentialsOptionsSafeHandle CreateSslServerCredentialsOptionsUsingConfig(SslClientCertificateRequestType sslClientCertificateRequest, ServerCertificateConfigSafeHandle serverCertificateConfig)
        {
            /* Creates an options object using a certificate config. Use this method when
             * the certificates and keys of the SSL server will not change during the
             * server's lifetime.
             * - Takes ownership of the serverCertificateConfig parameter.
             */
            return Native.grpcsharp_ssl_server_credentials_create_options_using_config(sslClientCertificateRequest, serverCertificateConfig);
        }

        public static ServerCredentialsOptionsSafeHandle CreateSslServerCredentialsOptionsUsingConfigFetcher(SslClientCertificateRequestType sslClientCertificateRequest, IntPtr serverCertificateConfigCallbackTag, IntPtr userData)
        {
            /* Creates an options object using a certificate config fetcher. Use this
             * method to reload the certificates and keys of the SSL server without
             * interrupting the operation of the server. Initial certificate config will be
             * fetched during server initialization.
             * - user_data parameter, if not NULL, contains opaque data which will be passed
             *   to the fetcher (see definition of
             *   grpc_ssl_server_certificate_config_callback).
             */
            return Native.grpcsharp_ssl_server_credentials_create_options_using_config_fetcher(sslClientCertificateRequest, serverCertificateConfigCallbackTag, userData);
        }

        protected override bool ReleaseHandle()
        {
            /* Should we call destroy here since the user of this object, which is
             *      ServerCredentialsSafeHandle CreateSslCredentials()
             * takes the ownership of this object?
             * Probably not because it throws:
             *     System.AccessViolationException: 'Attempted to read or write protected memory. This is often an indication that other memory is corrupt.'
             */
            // Native.grpcsharp_ssl_server_credentials_options_destroy(handle);
            return true;
        }
    }
}
