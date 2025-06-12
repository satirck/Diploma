namespace Devices.PPU;

public partial class Ppu2C02
{
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

    public Pixel GetColorFromPaletteRam(byte palette, byte pixel)
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
                data = (byte)((_status.Reg & 0xE0) | (_ppuDataBuffer & 0x1F));
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
                _ppuDataBuffer = PpuRead(_vramAddr.Reg);

                if (_vramAddr.Reg > 0x3F00) data = _ppuDataBuffer;
                _vramAddr.Reg += (ushort)(_control.IncrementMode ? 32 : 1);
                break;
        }

        return data;
    }

    public void CpuWrite(ushort addr, byte data)
    {
        switch (addr)
        {
            case 0x0000: // Control
                _control.Reg = data;
                _tramAddr.NametableX = _control.NametableX;
                _tramAddr.NametableY = _control.NametableY;
                break;
            case 0x0001: // Mask
                _mask.Reg = data;
                break;
            case 0x0002: // Status
                break;
            case 0x0003: // OAM Address
                break;
            case 0x0004: // OAM Data
                break;
            case 0x0005: // Scroll
                if (_addressLatch == 0)
                {
                    _fineX = (byte)(data & 0x07);
                    _tramAddr.CoarseX = (byte)(data >> 3);
                    _addressLatch = 1;
                }
                else
                {
                    _tramAddr.FineY = (byte)(data & 0x07);
                    _tramAddr.CoarseY = (byte)(data >> 3);
                    _addressLatch = 0;
                }

                break;
            case 0x0006: // PPU Address
                if (_addressLatch == 0)
                {
                    _tramAddr.Reg = (ushort)((_tramAddr.Reg & 0x00FF) | (data << 8));
                    _addressLatch = 1;
                }
                else
                {
                    _tramAddr.Reg = (ushort)((_tramAddr.Reg & 0xFF00) | data);
                    _vramAddr = _tramAddr;
                    _addressLatch = 0;
                }

                break;
            case 0x0007: // PPU Data
                PpuWrite(_vramAddr.Reg, data);
                _vramAddr.Reg += (ushort)(_control.IncrementMode ? 32 : 1);
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
        // Pattern Memory Request
        else if (addr <= 0x1FFF)
        {
            data = _tblPattern[(ushort)((addr & 0x1000) >> 12), addr & 0x0FFF];
        }
        // Name Table Memory
        else if (addr is >= 0x2000 and <= 0x3EFF)
        {
            addr &= 0x0FFF;

            if (_cart.GetMirror() == Mirror.Vertical)
            {
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
        // Pallet Memory
        else if (addr is >= 0x3F00 and <= 0x3FFF)
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
        // Pattern Memory Request
        else if (addr <= 0x1FFF)
        {
            _tblPattern[(ushort)((addr & 0x1000) >> 12), addr & 0x0FFF] = data;
        }
        // Name Table Memory
        else if (addr is >= 0x2000 and <= 0x3EFF)
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
        // Pallet Memory
        else if (addr is >= 0x3F00 and <= 0x3FFF)
        {
            addr &= 0x001F;

            if (addr == 0x0010) addr = 0x0000;
            if (addr == 0x0014) addr = 0x0004;
            if (addr == 0x0018) addr = 0x0008;
            if (addr == 0x001C) addr = 0x000C;

            _tblPalette[addr] = data;
        }
    }

    public void Clock()
    {
        if (_scanline >= -1 && _scanline < 240)
        {
            if (-1 == _scanline && 1 == _cycle)
            {
                _status.VerticalBlank = false;
            }

            if ((_cycle >= 2 && _cycle < 258) || (_cycle >= 321 && _cycle < 338))
            {
                UpdateShifters();
                
                switch ((_cycle - 1) % 8)
                {
                    case 0:
                        LoadBackgroundShifters();
                        _bgNextTileId = PpuRead((ushort)(0x2000 | (_vramAddr.Reg & 0x0FFF)));
                        break;
                    case 2:
                        _bgNextTileAttrib = PpuRead((ushort)(0x23C0
                                                             | ((_vramAddr.NametableY ? 1 : 0) << 11)
                                                             | ((_vramAddr.NametableX ? 1 : 0) << 10)
                                                             | ((_vramAddr.CoarseY >> 2) << 3)
                                                             | (_vramAddr.CoarseX >> 2)));

                        if ((_vramAddr.CoarseY & 0x02) != 0) _bgNextTileAttrib >>= 4;
                        if ((_vramAddr.CoarseX & 0x02) != 0) _bgNextTileAttrib >>= 2;
                        _bgNextTileAttrib &= 0x03;
                        break;
                    case 4:
                        _bgNextTileLsb = PpuRead((ushort)
                            (
                                ((_control.PatternBackground ? 1 : 0) << 12)
                                + (_bgNextTileId << 4)
                                + (_vramAddr.FineY) + 0
                            )
                        );
                        break;
                    case 6:
                        _bgNextTileMsb = PpuRead((ushort)
                            (
                                ((_control.PatternBackground ? 1 : 0) << 12)
                                + (_bgNextTileId << 4)
                                + (_vramAddr.FineY) + 8
                            )
                        );
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
                TransferAddressX();
            }

            if (_scanline == -1 && _cycle is >= 280 && _cycle < 305)
            {
                TransferAddressY();
            }
        }

        if (_scanline == 240)
        {
            // Post render
        }

        if (241 == _scanline && 1 == _cycle)
        {
            _status.VerticalBlank = true;
            if (_control.EnableNmi) Nmi = true;
        }

        byte bg_pixel = 0x00;   // The 2-bit pixel to be rendered
        byte bg_palette = 0x00; // The 3-bit index of the palette the pixel indexes

        if (_mask.RenderBackground)
        {
            ushort bit_mux = (ushort)(0x8000 >> _fineX);

            byte p0_pixel = (_bgShifterPatternLo & bit_mux) > 0 ? (byte)1 : (byte)0;
            byte p1_pixel = (_bgShifterPatternHi & bit_mux) > 0 ? (byte)1 : (byte)0;

            bg_pixel = (byte)((p1_pixel << 1) | p0_pixel);

            byte bg_pal0 = (_bgShifterAttribLo & bit_mux) > 0 ? (byte)1 : (byte)0;
            byte bg_pal1 = (_bgShifterAttribHi & bit_mux) > 0 ? (byte)1 : (byte)0;
            bg_palette = (byte)((bg_pal1 << 1) | bg_pal0);
        }

        _sprScreen.SetPixel(_cycle - 1, _scanline, GetColorFromPaletteRam(bg_palette, bg_pixel));

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

    // ВРЕМЕННЫЙ ДЕБАГ: рисует индексы тайлов из NameTable цветом
    public void DebugDrawNametableToScreen()
    {
        // 32 x 30 тайлов, каждый тайл 8x8 пикселей
        for (int y = 0; y < 30; y++)
        {
            for (int x = 0; x < 32; x++)
            {
                // Получаем индекс тайла из NameTable
                byte id = _tblName[0][y * 32 + x];
                // Цвет зависит от id (разные оттенки)
                Pixel color = new Pixel((byte)(id * 8), (byte)(id * 4), (byte)(id * 16));
                // Рисуем квадрат 8x8 пикселей этим цветом
                for (int py = 0; py < 8; py++)
                for (int px = 0; px < 8; px++)
                    _sprScreen.SetPixel(x * 8 + px, y * 8 + py, color);
            }
        }
    }
}