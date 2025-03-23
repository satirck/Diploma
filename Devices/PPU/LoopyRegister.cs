using System.Runtime.InteropServices;

namespace Devices.PPU;

[StructLayout(LayoutKind.Explicit)]
public struct LoopyRegister
{
    [FieldOffset(0)] public ushort reg;

    [FieldOffset(0)] public ushort coarseX;
    [FieldOffset(5)] public ushort coarseY;
    [FieldOffset(10)] public ushort nametableX;
    [FieldOffset(11)] public ushort nametableY;
    [FieldOffset(12)] public ushort fineY;
    [FieldOffset(15)] public ushort unused;
}
