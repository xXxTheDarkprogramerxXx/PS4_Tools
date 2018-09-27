/**********************************************
 *
 * Credits to IDC
 * 
 **********************************************/

namespace PS4_Tools
{
    internal struct Entry
    {
        public uint Flags;
        public long Offset;
        public long CompressedSize;
        public long UncompressedSize;

        public uint Id
        {
            get { return this.Flags >> 20; }
        }

        public bool IsCompressed
        {
            get { return (this.Flags & 8) != 0; }
        }

        public bool IsBlocked
        {
            get { return (this.Flags & 0x800) != 0; }
        }

        public override string ToString()
        {
            return string.Format("{1} {0} @ {2:X}", this.Id, this.IsBlocked == true ? "+" : "-", this.Offset);
        }
    }
}
