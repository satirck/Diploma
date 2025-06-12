namespace Devices.PPU.Registers;

public struct LoopyRegister
{
    private ushort _reg;

    public ushort Reg
    {
        get => _reg;
        set => _reg = (ushort)(value & 0x7FFF);
    }

    private const ushort CoarseXMask    = 0x001F; // 0000 0000 0001 1111
    private const ushort CoarseYMask    = 0x03E0; // 0000 0011 1110 0000
    private const ushort NametableXMask = 0x0400; // 0000 0100 0000 0000
    private const ushort NametableYMask = 0x0800; // 0000 1000 0000 0000
    private const ushort FineYMask      = 0x7000; // 0111 0000 0000 0000
    private const ushort UnusedMask     = 0x8000; // 1000 0000 0000 0000

    // Bits 0–4: coarse X
    public byte CoarseX
    {
        get => (byte)(_reg & 0b0001_1111);
        set => _reg = (ushort)((_reg & ~CoarseXMask) | (value & 0b0001_1111));
    }

    // Bits 5–9: coarse Y
    public byte CoarseY
    {
        get => (byte)((_reg >> 5) & 0b0001_1111);
        set => _reg = (ushort)((_reg & ~CoarseYMask) | ((value & 0b0001_1111) << 5));
    }

    // Bit 10: nametable X
    public bool NametableX
    {
        get => (_reg & NametableXMask) != 0;
        set
        {
            if (value)
                _reg |= NametableXMask;
            else
                _reg = (ushort)(_reg & ~NametableXMask);
        }
    }
    // Bit 11: nametable Y
    public bool NametableY
    {
        get => (_reg & NametableYMask) != 0;
        set
        {
            if (value)
                _reg |= NametableYMask;
            else
                _reg = (ushort)(_reg &  ~NametableYMask);
        }
    }

    // Bits 12–14: fine Y
    public byte FineY
    {
        get => (byte)((_reg >> 12) & 0b0000_0111);
        set => _reg = (ushort)((_reg & ~FineYMask) | ((value & 0b0000_0111) << 12));
    }

    // Bit 15: unused (обычно всегда 0)
    public bool Unused
    {
        get => (_reg & UnusedMask) != 0;
        set
        {
            if (value)
                _reg |= UnusedMask;
            else
                _reg = (ushort)(_reg & ~UnusedMask);
        }
    }
}
