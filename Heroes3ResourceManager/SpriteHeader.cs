using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace h3magic
{
    public class SpriteHeader
    {
        public int ContentSize { get; private set; }
        public int Type { get; private set; }
        public int FullWidth { get; private set; }
        public int FullHeight { get; private set; }
        public int SpriteWidth { get; private set; }
        public int SpriteHeight { get; private set; }
        public int LeftMargin { get; private set; }
        public int TopMargin { get; private set; }

        private int offset;

        public SpriteHeader(byte[] block, int offset)
        {
            this.offset = offset;
            ContentSize = BitConverter.ToInt32(block, offset);
            Type = BitConverter.ToInt32(block, offset + 4);
            FullWidth = BitConverter.ToInt32(block, offset + 8);
            FullHeight = BitConverter.ToInt32(block, offset + 12);
            SpriteWidth = BitConverter.ToInt32(block, offset + 16);
            SpriteHeight = BitConverter.ToInt32(block, offset + 20);
            LeftMargin = BitConverter.ToInt32(block, offset + 24);
            TopMargin = BitConverter.ToInt32(block, offset + 28);
        }        
    }
}
