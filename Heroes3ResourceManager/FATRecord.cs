using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;

namespace h3magic
{
    class FATRecord
    {


        public string FileName { get; private set; }
        public string Extension { get; private set; }
        public int Offset { get; private set; }
        public int Size { get; private set; }
        public int RealSize { get; private set; }

        public int Unknown1 { get; private set; }
        public int Unknown2 { get; private set; }

        private byte[] fname;
        private byte[] newVal;

        public FATRecord(byte[] record)
        {
            if (record.Length != 32)
                throw new ArgumentException("not a record");

            FileName = Encoding.ASCII.GetString(record, 0, Array.IndexOf<byte>(record, 0));

            fname = new byte[12];
            Buffer.BlockCopy(record, 0, fname, 0, 12);
            Extension = FileName.Substring(FileName.IndexOf('.') + 1).ToUpper();
            Unknown1 = BitConverter.ToInt32(record, 12);
            Offset = BitConverter.ToInt32(record, 16);
            RealSize = BitConverter.ToInt32(record, 20);
            Unknown2 = BitConverter.ToInt32(record, 24);
            Size = BitConverter.ToInt32(record, 28);
        }

        public override string ToString()
        {
            return FileName + " off: " + Offset + " zSize: " + Size + " realSize: " + RealSize;
        }

        public Bitmap GetBitmap(Stream stream)
        {
            if (Extension == "PCX")
            {
                byte[] array;
                if (newVal == null)
                {
                    byte[] bts = new byte[Size];
                    stream.Position = Offset;
                    stream.Read(bts, 0, Size);
                    array = ZlibWrapper.UnZlib(bts);
                }
                else
                {
                    array = newVal;
                }
                PCXFile file = new PCXFile(array);
                try
                {
                    return file.GetBitmap();
                }
                catch
                {
                    return null;
                }
            }
            else return null;
        }

        public byte[] GetRawData(Stream stream)
        {
            if (Size != 0)
            {
                byte[] bts = new byte[Size];
                stream.Position = Offset;
                stream.Read(bts, 0, Size);
                return ZlibWrapper.UnZlib(bts);
            }
            else
            {
                byte[] bts = new byte[RealSize];
                stream.Position = Offset;
                stream.Read(bts, 0, RealSize);
                return bts;
            }
        }



        public DEFFile GetDEFFile(Stream stream)
        {
            byte[] bts;
            if (Size != 0)
            {
                bts = new byte[Size];
                stream.Position = Offset;
                stream.Read(bts, 0, Size);
                return new DEFFile(ZlibWrapper.UnZlib(bts));
            }
            bts = new byte[RealSize];
            stream.Position = Offset;
            stream.Read(bts, 0, RealSize);
            return new DEFFile(bts);
        }

        public static string ToggleCase(string val)
        {
            return val.ToUpper();
        }

        public void SaveToDisk(Stream stream, string fileName)
        {

            if (Extension == "PCX")
            {
                Bitmap bmp = GetBitmap(stream);
                if (bmp != null)
                {
                    if (fileName != FileName)
                        bmp.Save(fileName);
                    else
                        bmp.Save(fileName.Substring(0, fileName.IndexOf('.')) + ".bmp");
                    bmp.Dispose();
                }
            }
            else
            {
                File.WriteAllBytes(fileName, GetRawData(stream));
            }
        }

        public void SaveToDisk(Stream stream)
        {

            if (Extension == "PCX")
            {
                Bitmap bmp = GetBitmap(stream);
                if (bmp != null)
                {
                    bmp.Save(FileName.Substring(0, FileName.Length - 4) + ".bmp");
                    bmp.Dispose();
                }
            }
            else
            {
                File.WriteAllBytes(Path.Combine(Path.GetDirectoryName((stream as FileStream).Name), FileName), GetRawData(stream));
                //File.WriteAllBytes(FileName, GetRawData(stream));
            }
        }


        public unsafe byte[] GetHeader()
        {
            byte[] buffer = new byte[32];

            Buffer.BlockCopy(Encoding.ASCII.GetBytes(FileName), 0, buffer, 0, Encoding.ASCII.GetByteCount(FileName));
            fixed (byte* bts = buffer)
            {
                int* ip = (int*)(void*)bts;
                *(ip + 3) = Unknown1;
                *(ip + 4) = Offset;
                *(ip + 5) = RealSize;
                *(ip + 6) = Unknown2;
                *(ip + 7) = Size;
            }
            return buffer;
        }

        public void SaveToStream(Stream input, Stream stream)
        {
            if (newVal == null)
            {
                int len = Size == 0 ? RealSize : Size;
                byte[] bytes = new byte[len];
                input.Position = Offset;
                input.Read(bytes, 0, len);
                Offset = (int)stream.Position;
                stream.Write(bytes, 0, len);
            }
            else
            {
                Offset = (int)stream.Position;
                stream.Write(newVal, 0, newVal.Length);
            }
        }

        public void AppendToStream(byte[] bts, Stream stream)
        {
            Offset = (int)stream.Position;
            byte[] comp = ZlibWrapper.Zlib(bts);
            Size = comp.Length;
            RealSize = bts.Length;
            stream.Write(comp, 0, Size);
        }



        public void ApplyChanges(byte[] newValue)
        {
            newVal = ZlibWrapper.Zlib(newValue);
            Size = newVal.Length;
            RealSize = newValue.Length;
        }





    }
}
