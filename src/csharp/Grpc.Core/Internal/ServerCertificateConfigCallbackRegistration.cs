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
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Grpc.Core.Logging;

namespace Grpc.Core.Internal
{
    internal class ServerCertificateConfigCallbackRegistration
    {
        static readonly ILogger Logger = GrpcEnvironment.Logger.ForType<ServerCertificateConfigCallbackRegistration>();
        static readonly NativeMethods Native = NativeMethods.Get();

        readonly ServerCertificateConfigCallback serverCertificateConfigCallback;
        readonly NativeCallbackRegistration callbackRegistration;

        public ServerCertificateConfigCallbackRegistration(ServerCertificateConfigCallback serverCertificateConfigCallback)
        {
            this.serverCertificateConfigCallback = serverCertificateConfigCallback;
            this.callbackRegistration = NativeCallbackDispatcher.RegisterCallback(HandleUniversalCallback);
        }

        public NativeCallbackRegistration CallbackRegistration => callbackRegistration;

        private int HandleUniversalCallback(IntPtr arg0, IntPtr arg1, IntPtr arg2, IntPtr arg3, IntPtr arg4, IntPtr arg5)
        {
            return ServerCertificateConfigCallback(arg0);
        }

        private int ServerCertificateConfigCallback(IntPtr arg0)
        {
            try
            {
                // Get the certificate from users callback:
                ServerCertificateConfig serverCertificateConfig = this.serverCertificateConfigCallback();

                // Write the pointer to the certificate to the location where arg1 is pointing to:
                ServerCertificateConfigSafeHandle nativeServerCertificateConfig = serverCertificateConfig.ToNative();
                Native.grpcsharp_write_ssl_server_certificate_config_to_pointer(arg0, nativeServerCertificateConfig);

                // Should we pass .New unconditionally? If we pass .Unchanged
                // then native try_replace_server_handshaker_factory will not be invoked,
                // effectively saving a small number of CPU cycles.
                // However, the cost of obtaining the current certificate from native and inspecting it
                // is probably orders of magnitude higher then unconditionally reloading certs every time.
                return (int)SslCertificateConfigReloadStatus.New;
            }
            catch (Exception e)
            {
                // eat the exception, we must not throw when inside callback from native code.
                Logger.Error(e, "Exception occurred while invoking verify peer callback handler.");
                // Return failure in case of exception.
                return (int)SslCertificateConfigReloadStatus.Fail;
            }
        }
    }
}
