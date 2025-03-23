using System.Numerics;

namespace Devices.PPU;

public partial class Ppu2C02
{
    public Ppu2C02()
    {
        _palScreen[0x00] = new Vector4(84, 84, 84, 255);
        _palScreen[0x01] = new Vector4(0, 30, 116, 255);
        _palScreen[0x02] = new Vector4(8, 16, 144, 255);
        _palScreen[0x03] = new Vector4(48, 0, 136, 255);
        _palScreen[0x04] = new Vector4(68, 0, 100, 255);
        _palScreen[0x05] = new Vector4(92, 0, 48, 255);
        _palScreen[0x06] = new Vector4(84, 4, 0, 255);
        _palScreen[0x07] = new Vector4(60, 24, 0, 255);
        _palScreen[0x08] = new Vector4(32, 42, 0, 255);
        _palScreen[0x09] = new Vector4(8, 58, 0, 255);
        _palScreen[0x0A] = new Vector4(0, 64, 0, 255);
        _palScreen[0x0B] = new Vector4(0, 60, 0, 255);
        _palScreen[0x0C] = new Vector4(0, 50, 60, 255);
        _palScreen[0x0D] = new Vector4(0, 0, 0, 255);
        _palScreen[0x0E] = new Vector4(0, 0, 0, 255);
        _palScreen[0x0F] = new Vector4(0, 0, 0, 255);

        _palScreen[0x10] = new Vector4(152, 150, 152, 255);
        _palScreen[0x11] = new Vector4(8, 76, 196, 255);
        _palScreen[0x12] = new Vector4(48, 50, 236, 255);
        _palScreen[0x13] = new Vector4(92, 30, 228, 255);
        _palScreen[0x14] = new Vector4(136, 20, 176, 255);
        _palScreen[0x15] = new Vector4(160, 20, 100, 255);
        _palScreen[0x16] = new Vector4(152, 34, 32, 255);
        _palScreen[0x17] = new Vector4(120, 60, 0, 255);
        _palScreen[0x18] = new Vector4(84, 90, 0, 255);
        _palScreen[0x19] = new Vector4(40, 114, 0, 255);
        _palScreen[0x1A] = new Vector4(8, 124, 0, 255);
        _palScreen[0x1B] = new Vector4(0, 118, 40, 255);
        _palScreen[0x1C] = new Vector4(0, 102, 120, 255);
        _palScreen[0x1D] = new Vector4(0, 0, 0, 255);
        _palScreen[0x1E] = new Vector4(0, 0, 0, 255);
        _palScreen[0x1F] = new Vector4(0, 0, 0, 255);

        _palScreen[0x20] = new Vector4(236, 238, 236, 255);
        _palScreen[0x21] = new Vector4(76, 154, 236, 255);
        _palScreen[0x22] = new Vector4(120, 124, 236, 255);
        _palScreen[0x23] = new Vector4(176, 98, 236, 255);
        _palScreen[0x24] = new Vector4(228, 84, 236, 255);
        _palScreen[0x25] = new Vector4(236, 88, 180, 255);
        _palScreen[0x26] = new Vector4(236, 106, 100, 255);
        _palScreen[0x27] = new Vector4(212, 136, 32, 255);
        _palScreen[0x28] = new Vector4(160, 170, 0, 255);
        _palScreen[0x29] = new Vector4(116, 196, 0, 255);
        _palScreen[0x2A] = new Vector4(76, 208, 32, 255);
        _palScreen[0x2B] = new Vector4(56, 204, 108, 255);
        _palScreen[0x2C] = new Vector4(56, 180, 204, 255);
        _palScreen[0x2D] = new Vector4(60, 60, 60, 255);
        _palScreen[0x2E] = new Vector4(0, 0, 0, 255);
        _palScreen[0x2F] = new Vector4(0, 0, 0, 255);

        _palScreen[0x30] = new Vector4(236, 238, 236, 255);
        _palScreen[0x31] = new Vector4(168, 204, 236, 255);
        _palScreen[0x32] = new Vector4(188, 188, 236, 255);
        _palScreen[0x33] = new Vector4(212, 178, 236, 255);
        _palScreen[0x34] = new Vector4(236, 174, 236, 255);
        _palScreen[0x35] = new Vector4(236, 174, 212, 255);
        _palScreen[0x36] = new Vector4(236, 180, 176, 255);
        _palScreen[0x37] = new Vector4(228, 196, 144, 255);
        _palScreen[0x38] = new Vector4(204, 210, 120, 255);
        _palScreen[0x39] = new Vector4(180, 222, 120, 255);
        _palScreen[0x3A] = new Vector4(168, 226, 144, 255);
        _palScreen[0x3B] = new Vector4(152, 226, 180, 255);
        _palScreen[0x3C] = new Vector4(160, 214, 228, 255);
        _palScreen[0x3D] = new Vector4(160, 162, 160, 255);
        _palScreen[0x3E] = new Vector4(0, 0, 0, 255);
        _palScreen[0x3F] = new Vector4(0, 0, 0, 255);
    }
}