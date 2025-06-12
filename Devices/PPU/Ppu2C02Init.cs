using Devices.PPU.Registers;

namespace Devices.PPU;

public partial class Ppu2C02
{
    private Cartridge.Cartridge _cart;
    
    private short _scanline;
    private short _cycle;
    //
    private Status _status;
    private Mask _mask;
    private Control _control;
    
    private LoopyRegister _vramAddr;
    private LoopyRegister _tramAddr;

    private byte _fineX = 0x00;
    private byte _addressLatch = 0x00;
    private byte _ppuDataBuffer = 0x00;

    private byte[][] _tblName = new byte[2][]
    {
        new byte[1024],
        new byte[1024]
    };

    public byte[] _tblPalette = new byte[32];
    private byte[,] _tblPattern = new byte[2, 4096];
    private Pixel[] _palScreen = new Pixel[0x40];
    private Sprite _sprScreen = new Sprite(256, 240);
    private Sprite[] _sprNameTable = [new Sprite(256, 240), new Sprite(256, 240)];
    private Sprite[] _sprPatternTable = [ new Sprite(128, 128), new Sprite(128, 128) ];

    // Background rendering
    private byte _bgNextTileId = 0x00;
    private byte _bgNextTileAttrib = 0x00;
    private byte _bgNextTileLsb = 0x00;
    private byte _bgNextTileMsb = 0x00;
    private ushort _bgShifterPatternLo = 0x0000;
    private ushort _bgShifterPatternHi = 0x0000;
    private ushort _bgShifterAttribLo = 0x0000;
    private ushort _bgShifterAttribHi = 0x0000;

    public bool FrameComplete = false;
    public bool Nmi = false;

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
    
    public void Reset()
    {
        _fineX = 0x00;
        _addressLatch = 0x00;
        _ppuDataBuffer = 0x00;
        _scanline = 0;
        _cycle = 0;
        _bgNextTileId = 0x00;
        _bgNextTileAttrib = 0x00;
        _bgNextTileLsb = 0x00;
        _bgNextTileMsb = 0x00;
        _bgShifterPatternLo = 0x0000;
        _bgShifterPatternHi = 0x0000;
        _bgShifterAttribLo = 0x0000;
        _bgShifterAttribHi = 0x0000;
        _status.Reg = 0x00;
        _mask.Reg = 0x00;
        _control.Reg = 0x00;
        _vramAddr.Reg = 0x0000;
        _tramAddr.Reg = 0x0000;
    }
    
    public Sprite GetScreen()
    {
        return _sprScreen;
    }

    public Sprite GetNameTable(int i)
    {
        return _sprNameTable[i];
    }

    private void IncrementScrollX()
    {
        // Только если включён рендеринг фона или спрайтов
        if (_mask.RenderBackground || _mask.RenderSprites)
        {
            if (_vramAddr.CoarseX == 31)
            {
                // Дошли до конца строки тайлов — переходим в другой nametable по X
                _vramAddr.CoarseX = 0;
                _vramAddr.NametableX = !_vramAddr.NametableX;
            }
            else
            {
                // Просто двигаемся вправо по тайлам
                _vramAddr.CoarseX++;
            }
        }
    }

    // ==============================================================================
    // Increment the background tile "pointer" one scanline vertically
    private void IncrementScrollY()
    {
        // Incrementing vertically is more complicated. The visible nametable
        // is 32x30 tiles, but in memory there is enough room for 32x32 tiles.
        // The bottom two rows of tiles are in fact not tiles at all, they
        // contain the "attribute" information for the entire table. This is
        // information that describes which palettes are used for different 
        // regions of the nametable.
        
        // In addition, the NES doesnt scroll vertically in chunks of 8 pixels
        // i.e. the height of a tile, it can perform fine scrolling by using
        // the fine_y component of the register. This means an increment in Y
        // first adjusts the fine offset, but may need to adjust the whole
        // row offset, since fine_y is a value 0 to 7, and a row is 8 pixels high

        // Ony if rendering is enabled
        if (_mask.RenderBackground || _mask.RenderSprites)
        {
            // If possible, just increment the fine y offset
            if (_vramAddr.FineY < 7)
            {
                _vramAddr.FineY++;
            }
            else
            {
                // If we have gone beyond the height of a row, we need to
                // increment the row, potentially wrapping into neighbouring
                // vertical nametables. Dont forget however, the bottom two rows
                // do not contain tile information. The coarse y offset is used
                // to identify which row of the nametable we want, and the fine
                // y offset is the specific "scanline"

                // Reset fine y offset
                _vramAddr.FineY = 0;

                // Check if we need to swap vertical nametable targets
                if (_vramAddr.CoarseY == 29)
                {
                    // We do, so reset coarse y offset
                    _vramAddr.CoarseY = 0;
                    // And flip the target nametable bit
                    _vramAddr.NametableY = !_vramAddr.NametableY;
                }
                else if (_vramAddr.CoarseY == 31)
                {
                    // In case the pointer is in the attribute memory, we
                    // just wrap around the current nametable
                    _vramAddr.CoarseY = 0;
                }
                else
                {
                    // None of the above boundary/wrapping conditions apply
                    // so just increment the coarse y offset
                    _vramAddr.CoarseY++;
                }
            }
        }
    }

    // ==============================================================================
    // Transfer the temporarily stored horizontal nametable access information
    // into the "pointer". Note that fine x scrolling is not part of the "pointer"
    // addressing mechanism
    private void TransferAddressX()
    {
        // Ony if rendering is enabled
        if (_mask.RenderBackground || _mask.RenderSprites)
        {
            _vramAddr.NametableX = _tramAddr.NametableX;
            _vramAddr.CoarseX    = _tramAddr.CoarseX;
        }
    }

    // ==============================================================================
    // Transfer the temporarily stored vertical nametable access information
    // into the "pointer". Note that fine y scrolling is part of the "pointer"
    // addressing mechanism
    private void TransferAddressY()
    {
        // Ony if rendering is enabled
        if (_mask.RenderBackground || _mask.RenderSprites)
        {
            _vramAddr.FineY      = _tramAddr.FineY;
            _vramAddr.NametableY = _tramAddr.NametableY;
            _vramAddr.CoarseY    = _tramAddr.CoarseY;
        }
    }


    // ==============================================================================
    // Prime the "in-effect" background tile shifters ready for outputting next
    // 8 pixels in scanline.
    private void LoadBackgroundShifters()
    {	
        // Each PPU update we calculate one pixel. These shifters shift 1 bit along
        // feeding the pixel compositor with the binary information it needs. Its
        // 16 bits wide, because the top 8 bits are the current 8 pixels being drawn
        // and the bottom 8 bits are the next 8 pixels to be drawn. Naturally this means
        // the required bit is always the MSB of the shifter. However, "fine x" scrolling
        // plays a part in this too, whcih is seen later, so in fact we can choose
        // any one of the top 8 bits.
        _bgShifterPatternLo = (ushort)((_bgShifterPatternLo & 0xFF00) | _bgNextTileLsb);
        _bgShifterPatternHi = (ushort)((_bgShifterPatternHi & 0xFF00) | _bgNextTileMsb);

        // Attribute bits do not change per pixel, rather they change every 8 pixels
        // but are synchronised with the pattern shifters for convenience, so here
        // we take the bottom 2 bits of the attribute word which represent which 
        // palette is being used for the current 8 pixels and the next 8 pixels, and 
        // "inflate" them to 8 bit words.
        _bgShifterAttribLo  = (ushort)((_bgShifterAttribLo & 0xFF00) | ((_bgNextTileAttrib & 0b01) != 0 ? 0xFF : 0x00));
        _bgShifterAttribHi  = (ushort)((_bgShifterAttribHi & 0xFF00) | ((_bgNextTileAttrib & 0b10) != 0 ? 0xFF : 0x00));
    }

    private void UpdateShifters()
    {
        if (_mask.RenderBackground)
        {
            // Shifting background tile pattern row
            _bgShifterPatternLo <<= 1;
            _bgShifterPatternHi <<= 1;

            // Shifting palette attributes by 1
            _bgShifterAttribLo <<= 1;
            _bgShifterAttribHi <<= 1;
        }
    }
}