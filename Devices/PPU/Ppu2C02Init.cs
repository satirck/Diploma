using System.Numerics;

namespace Devices.PPU;

public partial class Ppu2C02
{
    private ushort _bgShifterPatternLo = 0x0000;
    private ushort _bgShifterPatternHi = 0x0000;
    private ushort _bgShifterAttribLo  = 0x0000;
    private ushort _bgShifterAttribHi  = 0x0000;
    
    private Status _status;
    private Mask _mask;
    private PpuCtrl _control;
    
    private LoopyRegister _vramAddr;
    private LoopyRegister _tramAddr;

    private byte _fineX = 0x00;

    private byte[][] _tblName = new byte[2][]
    {
        new byte[1024],
        new byte[1024]
    };

    private byte[] _tblPalette = new byte[32];
    
    private byte[][] _tblPattern = new byte[2][]
    {
        new byte[4096],
        new byte[4096]
    };

    private Vector4[] _palScreen = new Vector4[0x40];
    private Sprite _sprScreen = new Sprite(256, 240);
    private Sprite[] _sprNameTable = [new Sprite(256, 240), new Sprite(256, 240)];
    private Sprite[] _sprPatternTable = [ new Sprite(128, 128), new Sprite(128, 128) ];

    private short _scanline = 0;
    private short _cycle = 0;

    private byte _bgNextTileId = 0x00;
    private byte _bgNextTileAttrb = 0x00;
    private byte _bgNextTileLsb = 0x00;
    private byte _bgNextTileMsb = 0x00;
    
    private byte _addrLatch = 0x00;
    private byte _ppuDataBuffer = 0x00;
    
    private readonly Random _random = new Random();

    public bool FrameComplete;
    public bool Nmi;
    
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

    private void IncrementScrollX()
    {
        if (_mask.renderBackground || _mask.renderSprites)
        {
            if (_vramAddr.CoarseX == 31)
            {
                _vramAddr.CoarseX = 0;
                _vramAddr.NametableX = !_vramAddr.NametableX;
            }
            else
            {
                _vramAddr.CoarseX++;
            }
        }
    }

    private void IncrementScrollY()
    {
        if (_mask.renderBackground || _mask.renderSprites)
        {
            if (_vramAddr.FineY < 7)
            {
                _vramAddr.FineY++;
            }
            else
            {
                _vramAddr.FineY = 0;

                if (_vramAddr.CoarseY == 29)
                {
                    _vramAddr.CoarseY = 0;
                    _vramAddr.NametableY = !_vramAddr.NametableY;
                }
                else if (_vramAddr.CoarseY == 31)
                {
                    _vramAddr.CoarseY = 0;
                }
                else
                {
                    _vramAddr.CoarseY++;
                }
            }
        }
    }

    private void TransferAddressX()
    {
        if (_mask.renderBackground || _mask.renderSprites)
        {
            _vramAddr.NametableX = _tramAddr.NametableX;
            _vramAddr.CoarseX = _tramAddr.CoarseX;
        }
    }

    private void TransferAddressY()
    {
        if (_mask.renderBackground || _mask.renderSprites)
        {
            _vramAddr.FineY = _tramAddr.FineY;
            _vramAddr.NametableY = _tramAddr.NametableY;
            _vramAddr.CoarseY = _tramAddr.CoarseY;
        }
    }

    private void LoadBackgroundShifters()
    {
        _bgShifterPatternLo = (ushort)((_bgShifterPatternLo & 0xFF00) | _bgNextTileLsb);
        _bgShifterPatternHi = (ushort)((_bgShifterPatternHi & 0xFF00) | _bgNextTileMsb);

        _bgShifterAttribLo = (ushort)((_bgShifterAttribLo & 0xFF00) | ((_bgNextTileAttrb & 0b01) != 0 ? 0xFF : 0x00));
        _bgShifterAttribHi = (ushort)((_bgShifterAttribHi & 0xFF00) | ((_bgNextTileAttrb & 0b10) != 0 ? 0xFF : 0x00));
    }

    private void UpdateShifters()
    {
        if (_mask.renderBackground)
        {
            _bgShifterPatternLo <<= 1;
            _bgShifterPatternHi <<= 1;
            _bgShifterAttribLo <<= 1;
            _bgShifterAttribHi <<= 1;
        }
    }
}