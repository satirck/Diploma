namespace Devices.PPU.PpuRegisters
{
    public struct Control
    {
        public byte reg;

        public bool NametableX
        {
            get => (reg & 0x01) != 0;
            set => reg = (byte)((reg & 0xFE) | (value ? 0x01 : 0x00));
        }

        public bool NametableY
        {
            get => (reg & 0x02) != 0;
            set => reg = (byte)((reg & 0xFD) | (value ? 0x02 : 0x00));
        }

        public bool IncrementMode
        {
            get => (reg & 0x04) != 0;
            set => reg = (byte)((reg & 0xFB) | (value ? 0x04 : 0x00));
        }

        public bool PatternSprite
        {
            get => (reg & 0x08) != 0;
            set => reg = (byte)((reg & 0xF7) | (value ? 0x08 : 0x00));
        }

        public bool PatternBackground
        {
            get => (reg & 0x10) != 0;
            set => reg = (byte)((reg & 0xEF) | (value ? 0x10 : 0x00));
        }

        public bool SpriteSize
        {
            get => (reg & 0x20) != 0;
            set => reg = (byte)((reg & 0xDF) | (value ? 0x20 : 0x00));
        }

        public bool SlaveMode
        {
            get => (reg & 0x40) != 0;
            set => reg = (byte)((reg & 0xBF) | (value ? 0x40 : 0x00));
        }

        public bool EnableNmi
        {
            get => (reg & 0x80) != 0;
            set => reg = (byte)((reg & 0x7F) | (value ? 0x80 : 0x00));
        }
    }
}