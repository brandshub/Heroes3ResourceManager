using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ComponentAce.Compression.Libs.zlib;

namespace h3magic
{
    static class ZlibWrapper
    {
        const int BUFFER_SIZE = 8192;

        public static byte[] UnZlib(byte[] bytes)
        {
            try
            {
                MemoryStream ms = new MemoryStream(bytes);
                ZInputStream zs = new ZInputStream(ms);
                MemoryStream mz = new MemoryStream();
                byte[] buffer = new byte[BUFFER_SIZE];
                int read;
                do
                {
                    read = zs.read(buffer, 0, BUFFER_SIZE);
                    if (read > 0)
                        mz.Write(buffer, 0, read);
                }
                while (read > 0);
                ms.Close();
                zs.Close();
                byte[] retVal = mz.ToArray();
                mz.Close();
                return retVal;

            }
            catch
            {
                return null;
            }
        }
        public static byte[] Zlib(byte[] bytes)
        {
            try
            {
                MemoryStream ms = new MemoryStream();

                ZOutputStream zs = new ZOutputStream(ms, zlibConst.Z_DEFAULT_COMPRESSION);

                zs.Write(bytes, 0, bytes.Length);
                zs.Flush();
                zs.Close();
                byte[] retVal = ms.ToArray();
                return retVal;
            }
            catch
            {
                return null;
            }
        }

        public static bool Test(byte[] bytes)
        {
            byte[] decom = UnZlib(bytes);
            byte[] comp = Zlib(decom);
            if (comp.Length != bytes.Length) return false;
            for (int i = 0; i < bytes.Length; i++)
                if (bytes[i] != comp[i])
                    return false;
            return true;
        }
    }


}
