using System.Runtime.InteropServices;

namespace Devices.PPU.Registers;

[StructLayout(LayoutKind.Sequential)]
public struct ObjectAttributeEntry
{
    public byte Y;          // Y-позиция спрайта
    public byte ID;         // ID тайла
    public byte Attribute;  // Атрибуты (флипы, приоритет, палитра)
    public byte X;          // X-позиция спрайта

    // Удобные свойства для чтения атрибутов:
    public bool FlipVertical        => (Attribute & 0b1000_0000) != 0;
    public bool FlipHorizontal      => (Attribute & 0b0100_0000) != 0;
    public bool BehindBackground    => (Attribute & 0b0010_0000) != 0;
    public byte Palette             => (byte)(Attribute & 0b0000_0011);
}
