using System.Runtime.InteropServices;

namespace Devices.PPU.PpuRegisters;

[StructLayout(LayoutKind.Explicit)]
public struct Status
{
    [FieldOffset(0)]
    public byte reg;

    [FieldOffset(0)]
    private byte _bits;

    public bool SpriteOverflow
    {
        get => (_bits & (1 << 5)) != 0;
        set => _bits = (byte)(value ? (_bits | (1 << 5)) : (_bits & ~(1 << 5)));
    }

    public bool SpriteZeroHit
    {
        get => (_bits & (1 << 6)) != 0;
        set => _bits = (byte)(value ? (_bits | (1 << 6)) : (_bits & ~(1 << 6)));
    }

    public bool VerticalBlank
    {
        get => (_bits & (1 << 7)) != 0;
        set => _bits = (byte)(value ? (_bits | (1 << 7)) : (_bits & ~(1 << 7)));
    }
}