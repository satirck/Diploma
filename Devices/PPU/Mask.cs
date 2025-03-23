using System.Runtime.InteropServices;

namespace Devices.PPU;

[StructLayout(LayoutKind.Explicit)]
public struct Mask
{
    [FieldOffset(0)] public byte reg;

    [FieldOffset(0)] public bool grayscale;
    [FieldOffset(0)] public bool renderBackgroundLeft;
    [FieldOffset(0)] public bool renderSpritesLeft;
    [FieldOffset(0)] public bool renderBackground;
    [FieldOffset(0)] public bool renderSprites;
    [FieldOffset(0)] public bool enhanceRed;
    [FieldOffset(0)] public bool enhanceGreen;
    [FieldOffset(0)] public bool enhanceBlue;

    public void SetFlag(int bit, bool value)
    {
        if (value)
            reg |= (byte)(1 << bit);
        else
            reg &= (byte)~(1 << bit);
    }

    public bool GetFlag(int bit) => (reg & (1 << bit)) != 0;
}