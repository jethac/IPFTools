using System;
using System.IO;
using System.Runtime.InteropServices;

namespace IPF
{
    class iph2bmp
    {
        static void Main(string[] args)
        {
            IPF.Wrapper.Initialize();

            // @todo make this less shitty
            foreach (string arg in args)
            {
                string path_source = Path.GetFullPath(arg);
                DoIPHToBMP(path_source);
            }

            IPF.Wrapper.Teardown();
        }

        // @todo allow setting heap size
        static int HEAP_SIZE = 2000000;

        private static void DoIPHToBMP (string source)
        {
            int bytecount = 0;
            byte[] workingheap = new byte[HEAP_SIZE];

            // First, figure out how big the file is.
            try
            {
                byte[] foo = File.ReadAllBytes(source);
                bytecount = foo.Length;
            }
            catch (IOException e)
            {
                Console.WriteLine(e.StackTrace);
                throw;
            }

            // Let's roll!
            // @todo actually do something with the status flags we get back
            if (bytecount > 0)
            {
                // Read the file.
                IntPtr unmanagedPointer = Marshal.AllocHGlobal(bytecount);
                int b = IPF.Wrapper.ReadIPF(
                    source,
                    unmanagedPointer.ToInt32()
                );
                // Write the file.
                // @todo make this less ugly
                string outputpath = String.Format("{0}.bmp", source.Substring(0, source.Length - 4));
                int c = IPF.Wrapper.WriteBMP(
                    outputpath,
                    unmanagedPointer.ToInt32()
                );
                Marshal.FreeHGlobal(unmanagedPointer);
            }

        }
    }
}
