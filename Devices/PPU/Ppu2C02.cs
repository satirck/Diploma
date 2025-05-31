namespace Devices.PPU;

public partial class Ppu2C02
{
    //Utils
    private readonly Random _random = new Random();

    public void Clock()
    {
        if (_scanline == -1 && _cycle == -1)
        {
            _status.VerticalBlank = false;
        }

        if (_scanline == 241 && _cycle == 1)
        {
            _status.VerticalBlank = true;
            if (_control.enableNmi)
            {
                Console.WriteLine("PPU: Triggering NMI");
                Nmi = true;
            }
        }

        if (_scanline >= 0 && _scanline < 240 && _cycle >= 1 && _cycle <= 256)
        {
            byte bgPixel = 0x00;
            byte bgPalette = 0x00;

            if (_mask.renderBackground)
            {
                ushort bitMux = (ushort)(0x8000 >> _fineX);
                byte p0Pixel = (byte)((_bgShifterPatternLo & bitMux) > 0 ? 1 : 0);
                byte p1Pixel = (byte)((_bgShifterPatternHi & bitMux) > 0 ? 1 : 0);
                bgPixel = (byte)((p1Pixel << 1) | p0Pixel);

                byte bgPal0 = (byte)((_bgShifterAttribLo & bitMux) > 0 ? 1 : 0);
                byte bgPal1 = (byte)((_bgShifterAttribHi & bitMux) > 0 ? 1 : 0);
                bgPalette = (byte)((bgPal1 << 1) | bgPal0);
            }

            _sprScreen.SetPixel(_cycle - 1, _scanline, GetColorFromPaletteRam(bgPalette, bgPixel));
        }

        if (_scanline >= -1 && _scanline < 240)
        {
            if (_scanline == 0 && _cycle == 0)
            {
                _cycle = 1;
            }

            if (_scanline == -1 && _cycle == 1)
            {
                _status.VerticalBlank = false;
            }

            if ((_cycle >= 2 && _cycle < 258) || (_cycle >= 321 && _cycle < 338))
            {
                switch ((_cycle - 1) & 0x07)
                {
                    case 0:
                        LoadBackgroundShifters();
                        _bgNextTileId = PpuRead((ushort)(0x2000 | (_vramAddr.reg & 0x0FFF)));
                        break;
                    case 2:
                        _bgNextTileAttrib = PpuRead((ushort)(0x23C0 | _vramAddr.GetNametableYShift() | _vramAddr.GetNametableXShift() | ((_vramAddr.coarseY >> 2) << 3) | (_vramAddr.coarseX >> 2)));
                        if ((_vramAddr.coarseY & 0x02) != 0) _bgNextTileAttrib >>= 4;
                        if ((_vramAddr.coarseX & 0x02) != 0) _bgNextTileAttrib >>= 2;
                        _bgNextTileAttrib &= 0x03;
                        break;
                    case 4:
                        _bgNextTileLsb = PpuRead((ushort)(((_control.patternBackground ? 1 : 0) << 12) + ((ushort)_bgNextTileId << 4) + (_vramAddr.fineY) + 0));
                        break;
                    case 6:
                        _bgNextTileMsb = PpuRead((ushort)((_control.patternBackground ? 1 : 0) + ((ushort)_bgNextTileId << 4) + (_vramAddr.fineY) + 8));
                        break;
                    case 7:
                        IncrementScrollX();
                        break;
                }
            }

            if (_cycle == 256)
            {
                IncrementScrollY();
            }

            if (_cycle == 257)
            {
                LoadBackgroundShifters();
                TransferAddressX();
            }

            if (_scanline == -1 && _cycle >= 280 && _cycle < 305)
            {
                TransferAddressY();
            }
        }

        // Advance renderer - it never stops, it's relentless
        _cycle++;
        if (_cycle >= 341)
        {
            _cycle = 0;
            _scanline++;
            if (_scanline >= 261)
            {
                _scanline = -1;
                FrameComplete = true;
                }
        }
    }

    public Sprite GetScreen()
    {
        return _sprScreen;
    }

    public Sprite GetNameTable(int i)
    {
        return _sprNameTable[i];
    }

    public Sprite GetPatternTable(byte i, byte palette)
    {
        for (byte nTileY = 0; nTileY < 16; nTileY++)
        {
            for (byte nTileX = 0; nTileX < 16; nTileX++)
            {
                ushort nOffset = (ushort)(nTileY * 256 + nTileX * 16);

                for (byte row = 0; row < 8; row++)
                {
                    byte tileLsb = PpuRead((ushort)(i * 0x1000 + nOffset + row + 0));
                    byte tileMsb = PpuRead((ushort)(i * 0x1000 + nOffset + row + 8));

                    for (byte col = 0; col < 8; col++)
                    {
                        byte pixel = (byte)((tileLsb & 0x01) + (tileMsb & 0x01));
                        tileLsb >>= 1;
                        tileMsb >>= 1;

                        _sprPatternTable[i].SetPixel(
                            nTileX * 8 + (7 - col),
                            nTileY * 8 + row,
                            GetColorFromPaletteRam(palette, pixel)
                        );
                    }
                }
            }
        }

        return _sprPatternTable[i];
    }

    private Pixel GetColorFromPaletteRam(byte palette, byte pixel)
    {
        var address = 0x3F00 + (palette << 2) + pixel;
        byte palIndex = PpuRead((ushort)address);
        return _palScreen[palIndex & 0x3F];
    }

    public byte CpuRead(ushort addr, bool bReadOnly = false)
    {
        byte data = 0x00;

        switch (addr)
        {
            case 0x0000: // Control
                break;
            case 0x0001: // Mask
                break;
            case 0x0002: // Status
                data = (byte)((_status.reg & 0xE0) | (_ppuDataBuffer & 0x1F));
                _status.VerticalBlank = false;
                _addressLatch = 0;
                break;
            case 0x0003: // OAM Address
                break;
            case 0x0004: // OAM Data
                break;
            case 0x0005: // Scroll
                break;
            case 0x0006: // PPU Address
                break;
            case 0x0007: // PPU Data
                data = _ppuDataBuffer;
                _ppuDataBuffer = PpuRead(_ppuAddr);

                if (_ppuAddr > 0x3F00) data = _ppuDataBuffer;
                _ppuAddr++;
                break;
        }

        return data;
    }

    public void CpuWrite(ushort addr, byte data)
    {
        switch (addr)
        {
            case 0x0000: // Control
                _control.reg = data;
                break;
            case 0x0001: // Mask
                _mask.reg = data;
                break;
            case 0x0002: // Status
                break;
            case 0x0003: // OAM Address
                break;
            case 0x0004: // OAM Data
                break;
            case 0x0005: // Scroll
                break;
            case 0x0006: // PPU Address
                if (_addressLatch == 0)
                {
                    _ppuAddr = (ushort)((_ppuAddr & 0x00FF) | (data << 8));
                    _addressLatch = 1;
                }
                else
                {
                    _ppuAddr = (ushort)((_ppuAddr & 0xFF00) | data);
                    _addressLatch = 0;
                }
                break;
            case 0x0007: // PPU Data
                PpuWrite(_ppuAddr, data);
                _ppuAddr++;
                break;
        }
    }

    public byte PpuRead(ushort addr, bool bReadOnly = false)
    {
        byte data = 0x00;
        addr &= 0x3FFF;

        if (_cart.PpuRead(addr, ref data))
        {
        }
        else if (addr <= 0x1FFF)
        {
            data = _tblPattern[(addr & 0x1000) >> 12, addr & 0x0FFF];
        }
        else if (addr <= 0x3EFF)
        {
            addr &= 0x0FFF;

            if (_cart.GetMirror() == Mirror.Vertical)
            {
                // Vertical
                if (addr >= 0x0000 && addr <= 0x03FF)
                    data = _tblName[0][addr & 0x03FF];
                if (addr >= 0x0400 && addr <= 0x07FF)
                    data = _tblName[1][addr & 0x03FF];
                if (addr >= 0x0800 && addr <= 0x0BFF)
                    data = _tblName[0][addr & 0x03FF];
                if (addr >= 0x0C00 && addr <= 0x0FFF)
                    data = _tblName[1][addr & 0x03FF];
            }
            else if (_cart.GetMirror() == Mirror.Horizontal)
            {
                // Horizontal
                if (addr >= 0x0000 && addr <= 0x03FF)
                    data = _tblName[0][addr & 0x03FF];
                if (addr >= 0x0400 && addr <= 0x07FF)
                    data = _tblName[0][addr & 0x03FF];
                if (addr >= 0x0800 && addr <= 0x0BFF)
                    data = _tblName[1][addr & 0x03FF];
                if (addr >= 0x0C00 && addr <= 0x0FFF)
                    data = _tblName[1][addr & 0x03FF];
            }
        }
        else if (addr <= 0x3FFF)
        {
            addr &= 0x001F;
            if (addr == 0x0010) addr = 0x0000;
            if (addr == 0x0014) addr = 0x0004;
            if (addr == 0x0018) addr = 0x0008;
            if (addr == 0x001C) addr = 0x000C;
            data = _tblPalette[addr];
        }

        return data;
    }

    public void PpuWrite(ushort addr, byte data)
    {
        addr &= 0x3FFF;
        if (_cart.PpuWrite(addr, data))
        {
        }
        else if (addr <= 0x1FFF)
        {
            _tblPattern[(addr & 0x1000) >> 12, addr & 0x0FFF] = data;
        }
        else if (addr <= 0x3EFF)
        {
            addr &= 0x0FFF;
            
            if (_cart.GetMirror() == Mirror.Vertical)
            {
                // Vertical
                if (addr >= 0x0000 && addr <= 0x03FF)
                    _tblName[0][addr & 0x03FF] = data;
                if (addr >= 0x0400 && addr <= 0x07FF)
                    _tblName[1][addr & 0x03FF] = data;
                if (addr >= 0x0800 && addr <= 0x0BFF)
                    _tblName[0][addr & 0x03FF] = data;
                if (addr >= 0x0C00 && addr <= 0x0FFF)
                    _tblName[1][addr & 0x03FF] = data;
            }
            else if (_cart.GetMirror() == Mirror.Horizontal)
            {
                // Horizontal
                if (addr >= 0x0000 && addr <= 0x03FF)
                    _tblName[0][addr & 0x03FF] = data;
                if (addr >= 0x0400 && addr <= 0x07FF)
                    _tblName[0][addr & 0x03FF] = data;
                if (addr >= 0x0800 && addr <= 0x0BFF)
                    _tblName[1][addr & 0x03FF] = data;
                if (addr >= 0x0C00 && addr <= 0x0FFF)
                    _tblName[1][addr & 0x03FF] = data;
            }
        }
        else if (addr <= 0x3FFF)
        {
            addr &= 0x001F;
            if (addr == 0x0010) addr = 0x0000;
            if (addr == 0x0014) addr = 0x0004;
            if (addr == 0x0018) addr = 0x0008;
            if (addr == 0x001C) addr = 0x000C;
            _tblPalette[addr] = data;
        }
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
        _status.reg = 0x00;
        _mask.reg = 0x00;
        _control.reg = 0x00;
        _vramAddr.reg = 0x0000;
        _tramAddr.reg = 0x0000;
    }

    private void LoadBackgroundShifters()
    {
        _bgShifterPatternLo = (ushort)((_bgShifterPatternLo & 0xFF00) | _bgNextTileLsb);
        _bgShifterPatternHi = (ushort)((_bgShifterPatternHi & 0xFF00) | _bgNextTileMsb);
        _bgShifterAttribLo = (ushort)((_bgShifterAttribLo & 0xFF00) | ((_bgNextTileAttrib & 0b01) != 0 ? 0xFF : 0x00));
        _bgShifterAttribHi = (ushort)((_bgShifterAttribHi & 0xFF00) | ((_bgNextTileAttrib & 0b10) != 0 ? 0xFF : 0x00));
    }

    private void IncrementScrollX()
    {
        if (_mask.renderBackground || _mask.renderSprites)
        {
            if (_vramAddr.coarseX == 31)
            {
                _vramAddr.coarseX = 0;
                _vramAddr.SetNametableX(!_vramAddr.GetNametableX());
            }
            else
            {
                _vramAddr.coarseX++;
            }
        }
    }

    private void IncrementScrollY()
    {
        if (_mask.renderBackground || _mask.renderSprites)
        {
            if (_vramAddr.fineY < 7)
            {
                _vramAddr.fineY++;
            }
            else
            {
                _vramAddr.fineY = 0;
                if (_vramAddr.coarseY == 29)
                {
                    _vramAddr.coarseY = 0;
                    _vramAddr.SetNametableY(!_vramAddr.GetNametableY());
                }
                else if (_vramAddr.coarseY == 31)
                {
                    _vramAddr.coarseY = 0;
                }
                else
                {
                    _vramAddr.coarseY++;
                }
            }
        }
    }

    private void TransferAddressX()
    {
        if (_mask.renderBackground || _mask.renderSprites)
        {
            _vramAddr.SetNametableX(_tramAddr.GetNametableX());
            _vramAddr.coarseX = _tramAddr.coarseX;
        }
    }

    private void TransferAddressY()
    {
        if (_mask.renderBackground || _mask.renderSprites)
        {
            _vramAddr.fineY = _tramAddr.fineY;
            _vramAddr.SetNametableY(_tramAddr.GetNametableY());
            _vramAddr.coarseY = _tramAddr.coarseY;
        }
    }
}