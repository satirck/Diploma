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
                Nmi = true;
            }
        }

        // Fake some noise for now
        _sprScreen.SetPixel(_cycle - 1, _scanline, _palScreen[(_random.Next() % 2 != 0) ? 0x3F : 0x30]);

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
        return _palScreen[PpuRead((ushort)address)];
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
    }
}