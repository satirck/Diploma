using System.Runtime.InteropServices;

namespace Devices.PPU.Registers;

[StructLayout(LayoutKind.Sequential)]
public struct Status
{
    private byte reg;

    public byte Reg
    {
        get => reg;
        set => reg = value;
    }

    public bool VerticalBlank
    {
        get => (reg & 0b1000_0000) != 0;
        set
        {
            if (value)
                reg |= 0b1000_0000;
            else
                reg &= 0b0111_1111;
        }
    }

    public bool SpriteZeroHit
    {
        get => (reg & 0b0100_0000) != 0;
        set
        {
            if (value)
                reg |= 0b0100_0000;
            else
                reg &= 0b1011_1111;
        }
    }

    public bool SpriteOverflow
    {
        get => (reg & 0b0010_0000) != 0;
        set
        {
            if (value)
                reg |= 0b0010_0000;
            else
                reg &= 0b1101_1111;
        }
    }

    public byte Unused
    {
        get => (byte)(reg & 0b0001_1111);
        set => reg = (byte)((reg & 0b1110_0000) | (value & 0b0001_1111));
    }
}
