using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace h3magic
{
    class SpriteBlockHeader
    {
        public int Unknown1 { get; private set; }
        public int SpritesCount { get; private set; }
        public long Unknown2 { get; private set; }
        public string[] Names { get; private set; }
        public int[] Offsets { get; private set; }
        public int HeaderLength { get { return 16 + 17 * SpritesCount; } }

        public List<SpriteHeader> spriteHeaders;

        public SpriteBlockHeader(byte[] bytes, int offset)
        {
            Unknown1 = BitConverter.ToInt32(bytes, offset);
            SpritesCount = BitConverter.ToInt32(bytes, offset + 4);
            Unknown2 = BitConverter.ToInt64(bytes, offset + 8);

            int off = offset + 16;
            Names = new string[SpritesCount];
            Offsets = new int[SpritesCount];
            for (int i = 0; i < SpritesCount; i++)
                Names[i] = Encoding.ASCII.GetString(bytes, off + i * 13, Array.IndexOf<byte>(bytes, 0, off + i * 13) - (off + i * 13));
            off += 13 * SpritesCount;
            for (int i = 0; i < SpritesCount; i++)
                Offsets[i] = BitConverter.ToInt32(bytes, off + i * 4);

            spriteHeaders = new List<SpriteHeader>(SpritesCount);
            for (int i = 0; i < SpritesCount; i++)
                spriteHeaders.Add(new SpriteHeader(bytes, Offsets[i]));
        }

       

    }
}
