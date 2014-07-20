using System;
using System.Runtime.InteropServices;

namespace IPF
{
    public class Wrapper
    {
        [DllImport("kernel32.dll", EntryPoint = "LoadLibrary")]
        static extern int LoadLibrary(
            [MarshalAs(UnmanagedType.LPStr)] string lpLibFileName);
        [DllImport("kernel32.dll", EntryPoint = "GetProcAddress")]
        static extern IntPtr GetProcAddress(int hModule,
            [MarshalAs(UnmanagedType.LPStr)] string lpProcName);

        [DllImport("kernel32.dll", EntryPoint = "FreeLibrary")]
        static extern bool FreeLibrary(int hModule);

        /**
         * Read an IPF (.iph) file into a byte array.
         * 
         * @param   string  lpFilename  The path to the file to be loaded.
         * @param   int     ptr         A pointer to the byte array to load the file into, as an int32.
         * @return  int                 0 on failure, 1 on success.    
         */
        public delegate int DReadIPF(string lpFileName, int ptr);

        /**
         * Write a 24-bit BMP to disk, given an IPF (.iph) file in memory.
         * 
         * @param   string  lpFilename  The path to the file to create.
         * @param   int     ptr         A pointer to the byte array that holds the IPF file, as an int32.
         * @return  int                 0 on failure, 1 on success.
         */
        public delegate int DWriteBMP(string lpFileName, int ptr);

        public static DReadIPF ReadIPF;
        public static DWriteBMP WriteBMP;

        static int hModule = 0;

        /**
         *  Load the library.
         */
        public static bool Initialize()
        {
            hModule = LoadLibrary(@"ipf33.dll");
            if (hModule == 0) return false;

            ReadIPF = (DReadIPF)Marshal.GetDelegateForFunctionPointer(
                GetProcAddress(hModule, "IPFDLL_ReadIPF"),
                typeof(DReadIPF)
            );
            WriteBMP = (DWriteBMP)Marshal.GetDelegateForFunctionPointer(
                GetProcAddress(hModule, "IPFDLL_WriteBMP24"),
                typeof(DWriteBMP)
            );

            return true;
        }

        /**
         *  Free the library.
         */ 
        public static bool Teardown()
        {
            return FreeLibrary(hModule);
        }
    }
}
