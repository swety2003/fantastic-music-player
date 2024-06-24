using Microsoft.Extensions.Configuration;
using System;
using System.Runtime.InteropServices;

namespace FantasticMusicPlayer
{
    public static unsafe class FFTConvolver
    {
        public delegate bool ConvolverInitCall(int blockSize, float* ir, int irLen);
        public delegate void ConvolverProcess(float* input,float* output, int len);
        public delegate void ConvolverReset();

        [DllImport("kernel32.dll")]
        private extern static IntPtr LoadLibrary(String path);

        [DllImport("kernel32.dll")]
        private extern static IntPtr GetProcAddress(IntPtr lib, String funcName);

        [DllImport("kernel32.dll")]
        private extern static bool FreeLibrary(IntPtr lib);

        private static IntPtr _dllHandle = IntPtr.Zero;

        public static void Init()
        {
            var cfg = App.CurrentApp.GetService<IConfiguration>();
            
            var FFTConvolverModule = cfg.GetValue<string>("FFTConvolverModule");
            if (_dllHandle == IntPtr.Zero)
            {
                IntPtr dllHandle = LoadLibrary(FFTConvolverModule);
                if(dllHandle == IntPtr.Zero)
                {
                    System.Windows.Forms.MessageBox.Show("加载DLL失败："+FFTConvolverModule+"\r\n"+Marshal.GetLastWin32Error());
                    throw new Exception("加载DLL失败");
                }
                IntPtr exportAddress = IntPtr.Zero;

                exportAddress = GetProcAddress(dllHandle, "_con01_init@12");
                con01_init = Marshal.GetDelegateForFunctionPointer<ConvolverInitCall>(exportAddress);
                exportAddress = GetProcAddress(dllHandle, "_con01_process@12");
                con01_process = Marshal.GetDelegateForFunctionPointer<ConvolverProcess>(exportAddress);
                exportAddress = GetProcAddress(dllHandle, "_con01_reset@0");
                con01_reset = Marshal.GetDelegateForFunctionPointer<ConvolverReset>(exportAddress);
                exportAddress = GetProcAddress(dllHandle, "_con02_init@12");
                con02_init = Marshal.GetDelegateForFunctionPointer<ConvolverInitCall>(exportAddress);
                exportAddress = GetProcAddress(dllHandle, "_con02_process@12");
                con02_process = Marshal.GetDelegateForFunctionPointer<ConvolverProcess>(exportAddress);
                exportAddress = GetProcAddress(dllHandle, "_con02_reset@0");
                con02_reset = Marshal.GetDelegateForFunctionPointer<ConvolverReset>(exportAddress);
                exportAddress = GetProcAddress(dllHandle, "_con03_init@12");
                con03_init = Marshal.GetDelegateForFunctionPointer<ConvolverInitCall>(exportAddress);
                exportAddress = GetProcAddress(dllHandle, "_con03_process@12");
                con03_process = Marshal.GetDelegateForFunctionPointer<ConvolverProcess>(exportAddress);
                exportAddress = GetProcAddress(dllHandle, "_con03_reset@0");
                con03_reset = Marshal.GetDelegateForFunctionPointer<ConvolverReset>(exportAddress);
                exportAddress = GetProcAddress(dllHandle, "_con04_init@12");
                con04_init = Marshal.GetDelegateForFunctionPointer<ConvolverInitCall>(exportAddress);
                exportAddress = GetProcAddress(dllHandle, "_con04_process@12");
                con04_process = Marshal.GetDelegateForFunctionPointer<ConvolverProcess>(exportAddress);
                exportAddress = GetProcAddress(dllHandle, "_con04_reset@0");
                con04_reset = Marshal.GetDelegateForFunctionPointer<ConvolverReset>(exportAddress);
                exportAddress = GetProcAddress(dllHandle, "_con05_init@12");
                con05_init = Marshal.GetDelegateForFunctionPointer<ConvolverInitCall>(exportAddress);
                exportAddress = GetProcAddress(dllHandle, "_con05_process@12");
                con05_process = Marshal.GetDelegateForFunctionPointer<ConvolverProcess>(exportAddress);
                exportAddress = GetProcAddress(dllHandle, "_con05_reset@0");
                con05_reset = Marshal.GetDelegateForFunctionPointer<ConvolverReset>(exportAddress);
                exportAddress = GetProcAddress(dllHandle, "_con06_init@12");
                con06_init = Marshal.GetDelegateForFunctionPointer<ConvolverInitCall>(exportAddress);
                exportAddress = GetProcAddress(dllHandle, "_con06_process@12");
                con06_process = Marshal.GetDelegateForFunctionPointer<ConvolverProcess>(exportAddress);
                exportAddress = GetProcAddress(dllHandle, "_con06_reset@0");
                con06_reset = Marshal.GetDelegateForFunctionPointer<ConvolverReset>(exportAddress);
                exportAddress = GetProcAddress(dllHandle, "_con07_init@12");
                con07_init = Marshal.GetDelegateForFunctionPointer<ConvolverInitCall>(exportAddress);
                exportAddress = GetProcAddress(dllHandle, "_con07_process@12");
                con07_process = Marshal.GetDelegateForFunctionPointer<ConvolverProcess>(exportAddress);
                exportAddress = GetProcAddress(dllHandle, "_con07_reset@0");
                con07_reset = Marshal.GetDelegateForFunctionPointer<ConvolverReset>(exportAddress);
                exportAddress = GetProcAddress(dllHandle, "_con08_init@12");
                con08_init = Marshal.GetDelegateForFunctionPointer<ConvolverInitCall>(exportAddress);
                exportAddress = GetProcAddress(dllHandle, "_con08_process@12");
                con08_process = Marshal.GetDelegateForFunctionPointer<ConvolverProcess>(exportAddress);
                exportAddress = GetProcAddress(dllHandle, "_con08_reset@0");
                con08_reset = Marshal.GetDelegateForFunctionPointer<ConvolverReset>(exportAddress);
                exportAddress = GetProcAddress(dllHandle, "_con09_init@12");
                con09_init = Marshal.GetDelegateForFunctionPointer<ConvolverInitCall>(exportAddress);
                exportAddress = GetProcAddress(dllHandle, "_con09_process@12");
                con09_process = Marshal.GetDelegateForFunctionPointer<ConvolverProcess>(exportAddress);
                exportAddress = GetProcAddress(dllHandle, "_con09_reset@0");
                con09_reset = Marshal.GetDelegateForFunctionPointer<ConvolverReset>(exportAddress);
                exportAddress = GetProcAddress(dllHandle, "_con10_init@12");
                con10_init = Marshal.GetDelegateForFunctionPointer<ConvolverInitCall>(exportAddress);
                exportAddress = GetProcAddress(dllHandle, "_con10_process@12");
                con10_process = Marshal.GetDelegateForFunctionPointer<ConvolverProcess>(exportAddress);
                exportAddress = GetProcAddress(dllHandle, "_con10_reset@0");
                con10_reset = Marshal.GetDelegateForFunctionPointer<ConvolverReset>(exportAddress);
                exportAddress = GetProcAddress(dllHandle, "_con11_init@12");
                con11_init = Marshal.GetDelegateForFunctionPointer<ConvolverInitCall>(exportAddress);
                exportAddress = GetProcAddress(dllHandle, "_con11_process@12");
                con11_process = Marshal.GetDelegateForFunctionPointer<ConvolverProcess>(exportAddress);
                exportAddress = GetProcAddress(dllHandle, "_con11_reset@0");
                con11_reset = Marshal.GetDelegateForFunctionPointer<ConvolverReset>(exportAddress);
                exportAddress = GetProcAddress(dllHandle, "_con12_init@12");
                con12_init = Marshal.GetDelegateForFunctionPointer<ConvolverInitCall>(exportAddress);
                exportAddress = GetProcAddress(dllHandle, "_con12_process@12");
                con12_process = Marshal.GetDelegateForFunctionPointer<ConvolverProcess>(exportAddress);
                exportAddress = GetProcAddress(dllHandle, "_con12_reset@0");
                con12_reset = Marshal.GetDelegateForFunctionPointer<ConvolverReset>(exportAddress);
                exportAddress = GetProcAddress(dllHandle, "_con13_init@12");
                con13_init = Marshal.GetDelegateForFunctionPointer<ConvolverInitCall>(exportAddress);
                exportAddress = GetProcAddress(dllHandle, "_con13_process@12");
                con13_process = Marshal.GetDelegateForFunctionPointer<ConvolverProcess>(exportAddress);
                exportAddress = GetProcAddress(dllHandle, "_con13_reset@0");
                con13_reset = Marshal.GetDelegateForFunctionPointer<ConvolverReset>(exportAddress);
                exportAddress = GetProcAddress(dllHandle, "_con14_init@12");
                con14_init = Marshal.GetDelegateForFunctionPointer<ConvolverInitCall>(exportAddress);
                exportAddress = GetProcAddress(dllHandle, "_con14_process@12");
                con14_process = Marshal.GetDelegateForFunctionPointer<ConvolverProcess>(exportAddress);
                exportAddress = GetProcAddress(dllHandle, "_con14_reset@0");
                con14_reset = Marshal.GetDelegateForFunctionPointer<ConvolverReset>(exportAddress);
                exportAddress = GetProcAddress(dllHandle, "_con15_init@12");
                con15_init = Marshal.GetDelegateForFunctionPointer<ConvolverInitCall>(exportAddress);
                exportAddress = GetProcAddress(dllHandle, "_con15_process@12");
                con15_process = Marshal.GetDelegateForFunctionPointer<ConvolverProcess>(exportAddress);
                exportAddress = GetProcAddress(dllHandle, "_con15_reset@0");
                con15_reset = Marshal.GetDelegateForFunctionPointer<ConvolverReset>(exportAddress);
                exportAddress = GetProcAddress(dllHandle, "_con16_init@12");
                con16_init = Marshal.GetDelegateForFunctionPointer<ConvolverInitCall>(exportAddress);
                exportAddress = GetProcAddress(dllHandle, "_con16_process@12");
                con16_process = Marshal.GetDelegateForFunctionPointer<ConvolverProcess>(exportAddress);
                exportAddress = GetProcAddress(dllHandle, "_con16_reset@0");
                con16_reset = Marshal.GetDelegateForFunctionPointer<ConvolverReset>(exportAddress);


                _dllHandle = dllHandle;
            }
        }

        public static ConvolverInitCall con01_init;
        public static ConvolverProcess con01_process;
        public static ConvolverReset con01_reset;
        public static ConvolverInitCall con02_init;
        public static ConvolverProcess con02_process;
        public static ConvolverReset con02_reset;
        public static ConvolverInitCall con03_init;
        public static ConvolverProcess con03_process;
        public static ConvolverReset con03_reset;
        public static ConvolverInitCall con04_init;
        public static ConvolverProcess con04_process;
        public static ConvolverReset con04_reset;
        public static ConvolverInitCall con05_init;
        public static ConvolverProcess con05_process;
        public static ConvolverReset con05_reset;
        public static ConvolverInitCall con06_init;
        public static ConvolverProcess con06_process;
        public static ConvolverReset con06_reset;
        public static ConvolverInitCall con07_init;
        public static ConvolverProcess con07_process;
        public static ConvolverReset con07_reset;
        public static ConvolverInitCall con08_init;
        public static ConvolverProcess con08_process;
        public static ConvolverReset con08_reset;
        public static ConvolverInitCall con09_init;
        public static ConvolverProcess con09_process;
        public static ConvolverReset con09_reset;
        public static ConvolverInitCall con10_init;
        public static ConvolverProcess con10_process;
        public static ConvolverReset con10_reset;
        public static ConvolverInitCall con11_init;
        public static ConvolverProcess con11_process;
        public static ConvolverReset con11_reset;
        public static ConvolverInitCall con12_init;
        public static ConvolverProcess con12_process;
        public static ConvolverReset con12_reset;
        public static ConvolverInitCall con13_init;
        public static ConvolverProcess con13_process;
        public static ConvolverReset con13_reset;
        public static ConvolverInitCall con14_init;
        public static ConvolverProcess con14_process;
        public static ConvolverReset con14_reset;
        public static ConvolverInitCall con15_init;
        public static ConvolverProcess con15_process;
        public static ConvolverReset con15_reset;
        public static ConvolverInitCall con16_init;
        public static ConvolverProcess con16_process;
        public static ConvolverReset con16_reset;

    }

}