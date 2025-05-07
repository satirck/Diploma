namespace Devices.PPU;

using System;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Explicit)]
public struct Pixel
{
    [FieldOffset(0)]
    public uint n;
    
    [FieldOffset(0)]
    public byte r;

    [FieldOffset(1)]
    public byte g;

    [FieldOffset(2)]
    public byte b;

    [FieldOffset(3)]
    public byte a;
    
    public const uint NDefaultPixel = 0xFF000000;
    
    public static Pixel Default => new() { n = NDefaultPixel };
    
    public Pixel(byte r, byte g, byte b, byte a = 255)
    {
        n = 0;
        this.r = r;
        this.g = g;
        this.b = b;
        this.a = a;
    }
    
    public uint ToUInt32()
    {
        return (uint)((a << 24) | (b << 16) | (g << 8) | r);
    }
    
    public uint ToBgra8888()
    {
        return (uint)(b | (g << 8) | (r << 16) | (a << 24));
    }

}
