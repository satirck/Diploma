using System.Runtime.InteropServices;

namespace Devices.PPU;

[StructLayout(LayoutKind.Explicit)]
public struct LoopyRegister
{
    [FieldOffset(0)]
    private ushort _reg;

    public ushort Reg
    {
        get => _reg;
        set => _reg = value;
    }

    public byte CoarseX
    {
        get => (byte)(_reg & 0b00000_00000_00011111);
        set => _reg = (ushort)((_reg & ~0b00000_00000_00011111) | (value & 0b11111));
    }

    public byte CoarseY
    {
        get => (byte)((_reg >> 5) & 0b11111);
        set => _reg = (ushort)((_reg & ~0b00000_00011_11100000) | ((value & 0b11111) << 5));
    }

    public bool NametableX
    {
        get => (_reg & (1 << 10)) != 0;
        set => _reg = (ushort)(value ? (_reg | (1 << 10)) : (_reg & ~(1 << 10)));
    }

    public bool NametableY
    {
        get => (_reg & (1 << 11)) != 0;
        set => _reg = (ushort)(value ? (_reg | (1 << 11)) : (_reg & ~(1 << 11)));
    }

    public byte FineY
    {
        get => (byte)((_reg >> 12) & 0b111);
        set => _reg = (ushort)((_reg & ~(0b111 << 12)) | ((value & 0b111) << 12));
    }
}
