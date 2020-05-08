#region Copyright notice and license

// Copyright 2019 The gRPC Authors
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

namespace Grpc.Core
{
    /// <summary>
    /// Verification context for ServerCertificateConfigContext.
    /// </summary>
    public class ServerCertificateConfigContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Grpc.Core.ServerCertificateConfigContext"/> class.
        /// </summary>
        /// <param name="userData">ASDF.</param>
        /// <param name="pointerToPointerToServerCertConfig">Asdf.</param>
        internal ServerCertificateConfigContext(IntPtr userData, IntPtr pointerToPointerToServerCertConfig)
        {
            this.UserData = userData;
            this.PointerToPointerToServerCertConfig = pointerToPointerToServerCertConfig;
        }

        /// <summary>
        /// I dont know what this user data is.
        /// verify_peer_callback also declares a "user_data/userdata" in C++,
        /// but it is not present in VerifyPeerContext,
        /// so maybe it should not be present here as well?
        /// </summary>
        public IntPtr UserData { get; }

        /// <summary>
        /// ASDF.
        /// </summary>
        public IntPtr PointerToPointerToServerCertConfig { get; }
    }
}
