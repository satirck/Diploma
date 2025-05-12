using System.Runtime.InteropServices;

namespace Devices.PPU.PpuRegisters;

[StructLayout(LayoutKind.Explicit)]
public struct PpuCtrl
{
    [FieldOffset(0)] public byte reg;

    [FieldOffset(0)] public bool nametableX;
    [FieldOffset(1)] public bool nametableY;
    [FieldOffset(2)] public bool incrementMode;
    [FieldOffset(3)] public bool patternSprite;
    [FieldOffset(4)] public bool patternBackground;
    [FieldOffset(5)] public bool spriteSize;
    [FieldOffset(6)] public bool slaveMode;
    [FieldOffset(7)] public bool enableNmi;

    public void SetFlag(int bit, bool value)
    {
        if (value)
            reg |= (byte)(1 << bit);
        else
            reg &= (byte)~(1 << bit);
    }

    public bool GetFlag(int bit) => (reg & (1 << bit)) != 0;
}