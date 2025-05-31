using System.Runtime.InteropServices;

namespace Devices.PPU.PpuRegisters;

public struct Status
{
    private byte _reg;

    public byte reg
    {
        get => _reg;
        set => _reg = value;
    }

    // bits 0-4: unused
    public byte unused
    {
        get => (byte)(_reg & 0x1F);
        set => _reg = (byte)((_reg & ~0x1F) | (value & 0x1F));
    }

    // bit 5: sprite overflow
    public bool SpriteOverflow
    {
        get => (_reg & (1 << 5)) != 0;
        set => _reg = (byte)(value ? (_reg | (1 << 5)) : (_reg & ~(1 << 5)));
    }

    // bit 6: sprite zero hit
    public bool SpriteZeroHit
    {
        get => (_reg & (1 << 6)) != 0;
        set => _reg = (byte)(value ? (_reg | (1 << 6)) : (_reg & ~(1 << 6)));
    }

    // bit 7: vertical blank
    public bool VerticalBlank
    {
        get => (_reg & (1 << 7)) != 0;
        set => _reg = (byte)(value ? (_reg | (1 << 7)) : (_reg & ~(1 << 7)));
    }
}