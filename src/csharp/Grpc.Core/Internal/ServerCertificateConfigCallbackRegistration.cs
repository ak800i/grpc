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
            return ServerCertificateConfigCallback(arg0, arg1, arg2 != IntPtr.Zero);
        }

        private int ServerCertificateConfigCallback(IntPtr arg0, IntPtr arg1, bool isDestroy)
        {
            // Do we need destroy?
            if (isDestroy)
            {
                this.callbackRegistration.Dispose();
                return 0;
            }

            try
            {
                ServerCertificateConfig serverCertificateConfig = this.serverCertificateConfigCallback();

                // perhaps the part below should be done in ext.cc (Native runtime)
                ServerCertificateConfigSafeHandle nativeServerCertificateConfig = serverCertificateConfig.ToNative();
                //GCHandle handle = GCHandle.Alloc(nativeServerCertificateConfig);
                //IntPtr pointerToNativeHandle = GCHandle.ToIntPtr(handle);
                //Logger.Debug($"pointerToNativeHandle={pointerToNativeHandle.ToString("X")}");
                //Marshal.WriteIntPtr(arg1, pointerToNativeHandle);
                Native.grpcsharp_write_config_to_pointer(arg1, nativeServerCertificateConfig);
                // a GCHnadle to nativeServerCertificateConfig needs to written into the address that arg1 points to.

                // instead of context we could retrieve the old cert config and compare
                // var context = new ServerCertificateConfigContext(arg0, arg1);

                // Maybe the context should have some user-facing classes and then after the callback
                // what the user set should be re-tumbled back to the pointer. here.
                return (int)SslCertificateConfigReloadStatus.New;
            }
            catch (Exception e)
            {
                // eat the exception, we must not throw when inside callback from native code.
                Logger.Error(e, "Exception occurred while invoking verify peer callback handler.");
                // Return validation failure in case of exception.
                return 1;
            }
        }
    }
}
