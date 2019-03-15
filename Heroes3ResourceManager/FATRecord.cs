using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Diagnostics;

namespace h3magic
{
    public class FatRecord
    {
        public string FileName { get; private set; }
        public string Extension { get; private set; }
        public int Offset { get; private set; }
        public int Size { get; private set; }
        public int RealSize { get; private set; }

        public int Unknown1 { get; private set; }
        public int SpriteType { get; private set; }

        public bool Created { get; set; }

        public bool HasChanged { get; set; }

        private byte[] fname;
        private byte[] newVal;

        public LodFile Parent { get; set; }

        public FatRecord(LodFile parent, byte[] record)
        {
            if (record.Length != 32)
                throw new ArgumentException("not a record");

            Parent = parent;
            FileName = Encoding.ASCII.GetString(record, 0, Array.IndexOf<byte>(record, 0));

            fname = new byte[12];
            Buffer.BlockCopy(record, 0, fname, 0, 12);
            Extension = FileName.Substring(FileName.IndexOf('.') + 1).ToUpper();
            Unknown1 = BitConverter.ToInt32(record, 12);
            Offset = BitConverter.ToInt32(record, 16);
            RealSize = BitConverter.ToInt32(record, 20);
            SpriteType = BitConverter.ToInt32(record, 24);
            Size = BitConverter.ToInt32(record, 28);
        }

        public override string ToString()
        {
            return FileName + " parent: " + Parent + " off: " + Offset + " zSize: " + Size + " realSize: " + RealSize;
        }

        public Bitmap GetBitmap()
        {
            if (Parent != null)
                return GetBitmap(Parent.stream);
            return null;
        }

        public Bitmap GetBitmap(Stream stream)
        {
            if (Extension == "PCX")
            {
                byte[] array;
                if (newVal == null)
                {
                    if (Size != 0)
                    {
                        byte[] bts = new byte[Size];
                        stream.Position = Offset;
                        stream.Read(bts, 0, Size);
                        array = ZlibWrapper.UnZlib(bts);
                    }
                    else if (RealSize != 0)
                    {
                        array = new byte[RealSize];
                        stream.Position = Offset;
                        stream.Read(array, 0, RealSize);
                    }
                    else
                    {
                        throw new Exception("Image size unknown");
                    }
                }
                else
                {
                    array = newVal;
                }

                var file = new PcxFile(array);
                try
                {
                    return file.GetBitmap();
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
            return null;
        }

        public byte[] GetBitmap24Data(Stream stream, out int width)
        {
            width = 0;
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
                var file = new PcxFile(array);
                try
                {
                    return file.GetBitmap24Bytes(out width);
                }
                catch
                {
                    return null;
                }
            }
            return null;
        }

        public byte[] GetRawData(Stream stream)
        {
            if (HasChanged && !Created)
            {
                return newVal;
            }


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


        public byte[] GetRawData()
        {
            if (Parent != null)
                return GetRawData(Parent.stream);
            return null;
        }


        public DefFile GetDefFile(Stream stream)
        {
            if (newVal == null)
            {
                stream.Position = Offset;
                if (Size != 0)
                {
                    var bts = new byte[Size];
                    stream.Read(bts, 0, Size);
                    newVal = ZlibWrapper.UnZlib(bts);
                }
                else
                {
                    newVal = new byte[RealSize];
                    stream.Read(newVal, 0, RealSize);
                }
            }
            return new DefFile(this, newVal);
        }

        public DefFile GetDefFile()
        {
            if (Parent != null)
                return GetDefFile(Parent.stream);
            return null;
        }

        public static string ToggleCase(string val)
        {
            return val.ToUpper();
        }

        public void SaveToDisk(Stream stream, string fileName)
        {

            if (Extension == "PCX")
            {
                var bmp = GetBitmap();
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
                File.WriteAllBytes(fileName, GetRawData());
            }
        }

        public void SaveToDisk(Stream stream)
        {

            if (Extension == "PCX")
            {
                var bmp = GetBitmap();
                if (bmp != null)
                {
                    bmp.Save(Path.Combine(Path.GetDirectoryName((stream as FileStream).Name), FileName.Substring(0, FileName.Length - 4) + ".bmp"), System.Drawing.Imaging.ImageFormat.Bmp);
                    bmp.Dispose();
                }
            }
            else
            {
                File.WriteAllBytes(Path.Combine(Path.GetDirectoryName((stream as FileStream).Name), FileName), GetRawData());
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
                *(ip + 6) = SpriteType;
                *(ip + 7) = Size;
            }
            return buffer;
        }

        public void SaveToStream(Stream input, Stream output)
        {
            if (!HasChanged || Created)
            {
                int len = Size == 0 ? RealSize : Size;
                byte[] bytes = new byte[len];
                input.Position = Offset;
                input.Read(bytes, 0, len);
                Offset = (int)output.Position;
                output.Write(bytes, 0, len);
            }
            else
            {
                var temp = ZlibWrapper.Zlib(newVal);
                Size = temp.Length;
                Offset = (int)output.Position;
                output.Write(temp, 0, temp.Length);
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
            HasChanged = true;
            newVal = newValue;
            //newVal = ZlibWrapper.Zlib(newValue);
            //Size = newVal.Length;
            RealSize = newValue.Length;
        }

        public FatRecord Clone(string newName)
        {
            var clone = (FatRecord)MemberwiseClone();
            clone.FileName = newName;
            clone.newVal = null;
            clone.Created = true;
            return clone;
        }

    }
}
