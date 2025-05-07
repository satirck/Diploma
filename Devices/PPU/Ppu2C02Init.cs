using Devices.PPU.PpuRegisters;

namespace Devices.PPU;

public partial class Ppu2C02
{
    private Cartridge.Cartridge _cart;
    
    private short _scanline;
    private short _cycle;

    private Status _status;
    private Mask _mask;
    private PpuCtrl _control;
    private LoopyRegister _vramAddr;
    private LoopyRegister _tramAddr;

    private byte _fineX = 0x00;
    private byte _addressLatch = 0x00;
    private byte _ppuDataBuffer = 0x00;
    private ushort _ppuAddr = 0x0000;
    
    private byte[][] _tblName = new byte[2][]
    {
        new byte[1024],
        new byte[1024]
    };

    private byte[] _tblPalette = new byte[32];
    private byte[,] _tblPattern = new byte[2, 4096];
    private Pixel[] _palScreen = new Pixel[0x40];
    private Sprite _sprScreen = new Sprite(256, 240);
    private Sprite[] _sprNameTable = [new Sprite(256, 240), new Sprite(256, 240)];
    private Sprite[] _sprPatternTable = [ new Sprite(128, 128), new Sprite(128, 128) ];

    public bool FrameComplete;
    public bool Nmi;

    public Ppu2C02()
    {
        _palScreen[0x00] = new Pixel(84, 84, 84);
        _palScreen[0x01] = new Pixel(0, 30, 116);
        _palScreen[0x02] = new Pixel(8, 16, 144);
        _palScreen[0x03] = new Pixel(48, 0, 136);
        _palScreen[0x04] = new Pixel(68, 0, 100);
        _palScreen[0x05] = new Pixel(92, 0, 48);
        _palScreen[0x06] = new Pixel(84, 4, 0);
        _palScreen[0x07] = new Pixel(60, 24, 0);
        _palScreen[0x08] = new Pixel(32, 42, 0);
        _palScreen[0x09] = new Pixel(8, 58, 0);
        _palScreen[0x0A] = new Pixel(0, 64, 0);
        _palScreen[0x0B] = new Pixel(0, 60, 0);
        _palScreen[0x0C] = new Pixel(0, 50, 60);
        _palScreen[0x0D] = new Pixel(0, 0, 0);
        _palScreen[0x0E] = new Pixel(0, 0, 0);
        _palScreen[0x0F] = new Pixel(0, 0, 0);

        _palScreen[0x10] = new Pixel(152, 150, 152);
        _palScreen[0x11] = new Pixel(8, 76, 196);
        _palScreen[0x12] = new Pixel(48, 50, 236);
        _palScreen[0x13] = new Pixel(92, 30, 228);
        _palScreen[0x14] = new Pixel(136, 20, 176);
        _palScreen[0x15] = new Pixel(160, 20, 100);
        _palScreen[0x16] = new Pixel(152, 34, 32);
        _palScreen[0x17] = new Pixel(120, 60, 0);
        _palScreen[0x18] = new Pixel(84, 90, 0);
        _palScreen[0x19] = new Pixel(40, 114, 0);
        _palScreen[0x1A] = new Pixel(8, 124, 0);
        _palScreen[0x1B] = new Pixel(0, 118, 40);
        _palScreen[0x1C] = new Pixel(0, 102, 120);
        _palScreen[0x1D] = new Pixel(0, 0, 0);
        _palScreen[0x1E] = new Pixel(0, 0, 0);
        _palScreen[0x1F] = new Pixel(0, 0, 0);

        _palScreen[0x20] = new Pixel(236, 238, 236);
        _palScreen[0x21] = new Pixel(76, 154, 236);
        _palScreen[0x22] = new Pixel(120, 124, 236);
        _palScreen[0x23] = new Pixel(176, 98, 236);
        _palScreen[0x24] = new Pixel(228, 84, 236);
        _palScreen[0x25] = new Pixel(236, 88, 180);
        _palScreen[0x26] = new Pixel(236, 106, 100);
        _palScreen[0x27] = new Pixel(212, 136, 32);
        _palScreen[0x28] = new Pixel(160, 170, 0);
        _palScreen[0x29] = new Pixel(116, 196, 0);
        _palScreen[0x2A] = new Pixel(76, 208, 32);
        _palScreen[0x2B] = new Pixel(56, 204, 108);
        _palScreen[0x2C] = new Pixel(56, 180, 204);
        _palScreen[0x2D] = new Pixel(60, 60, 60);
        _palScreen[0x2E] = new Pixel(0, 0, 0);
        _palScreen[0x2F] = new Pixel(0, 0, 0);

        _palScreen[0x30] = new Pixel(236, 238, 236);
        _palScreen[0x31] = new Pixel(168, 204, 236);
        _palScreen[0x32] = new Pixel(188, 188, 236);
        _palScreen[0x33] = new Pixel(212, 178, 236);
        _palScreen[0x34] = new Pixel(236, 174, 236);
        _palScreen[0x35] = new Pixel(236, 174, 212);
        _palScreen[0x36] = new Pixel(236, 180, 176);
        _palScreen[0x37] = new Pixel(228, 196, 144);
        _palScreen[0x38] = new Pixel(204, 210, 120);
        _palScreen[0x39] = new Pixel(180, 222, 120);
        _palScreen[0x3A] = new Pixel(168, 226, 144);
        _palScreen[0x3B] = new Pixel(152, 226, 180);
        _palScreen[0x3C] = new Pixel(160, 214, 228);
        _palScreen[0x3D] = new Pixel(160, 162, 160);
        _palScreen[0x3E] = new Pixel(0, 0, 0);
        _palScreen[0x3F] = new Pixel(0, 0, 0);
    }

    public void ConnectCart(Cartridge.Cartridge cartridge)
    {
        _cart = cartridge;
    }
}