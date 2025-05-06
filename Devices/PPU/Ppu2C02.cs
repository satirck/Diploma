using System.Numerics;

namespace Devices.PPU;

using Cartridge;

public partial class Ppu2C02
{
    private Cartridge _cart;

    public void ConnectCart(Cartridge cartridge)
    {
        _cart = cartridge;
    }

    public void Clock()
    {
        if (_scanline >= -1 && _scanline < 240)
        {
            if (_scanline == 0 && _cycle == 0)
            {
                _cycle = 1;
            }

            if (_scanline == -1 && _cycle == -1)
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
                        _bgNextTileAttrb = PpuRead((ushort)(
                            0x23C0 |
                            (_vramAddr.NametableY ? 1 << 11 : 0) |
                            (_vramAddr.NametableX ? 1 << 10 : 0) |
                            ((_vramAddr.CoarseY >> 2) << 3) |
                            (_vramAddr.CoarseX >> 2)
                        ));
                        if ((_vramAddr.CoarseY & 0x02) != 0) _bgNextTileAttrb >>= 4;
                        if ((_vramAddr.CoarseX & 0x02) != 0) _bgNextTileAttrb >>= 2;
                        _bgNextTileAttrb &= 0x03;
                        break;
                    case 4:
                        _bgNextTileLsb = PpuRead(
                            (ushort)(((_control.patternBackground ? 1 : 0) << 12)
                                     + (_bgNextTileId << 4)
                                     + _vramAddr.FineY));
                        break;
                    case 6:
                        _bgNextTileMsb = PpuRead(
                            (ushort)(((_control.patternBackground ? 1 : 0) << 12)
                                     + (_bgNextTileId << 4)
                                     + _vramAddr.FineY
                                     + 8));
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

            // Superfluous reads of tile id at end of scanline
            if (_cycle == 338 || _cycle == 340)
            {
                _bgNextTileId = PpuRead((ushort)(0x2000 | (_vramAddr.Reg & 0x0FFF)));
            }

            if (_scanline == -1 && _cycle >= 280 && _cycle < 305)
            {
                TransferAddressY();
            }
        }

        if (_scanline == 240)
        {
            // Post Render Scanline
        }

        if (_scanline >= 241 && _scanline < 261)
        {
            if (_scanline == 241 && _cycle == 1)
            {
                _status.VerticalBlank = true;
                if (_control.enableNmi)
                {
                    Nmi = true;
                }
            }
        }

        byte bgPixel = 0x00;
        byte bgPalette = 0x00;

        if (_mask.renderBackground)
        {
            ushort bitMux = (ushort)(0x8000 >> _fineX);

            byte p0Pixel = (byte)((_bgShifterPatternLo & bitMux) > 0 ? 1 : 0);
            byte p1Pixel = (byte)((_bgShifterPatternHi & bitMux) > 0 ? 1 : 0);
            bgPixel = (byte)((p1Pixel << 1) | p0Pixel);

            byte pal0 = (byte)((_bgShifterAttribLo & bitMux) > 0 ? 1 : 0);
            byte pal1 = (byte)((_bgShifterAttribHi & bitMux) > 0 ? 1 : 0);
            bgPalette = (byte)((pal1 << 1) | pal0);
        }

        _sprScreen.SetPixel(_cycle - 1, _scanline, GetColorFromPalette(bgPalette, bgPixel));

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
                            GetColorFromPalette(palette, pixel)
                        );
                    }
                }
            }
        }

        return _sprPatternTable[i];
    }

    public Vector4 GetColorFromPalette(byte palette, byte pixel)
    {
        return _palScreen[PpuRead((ushort)(0x3F00 + (palette << 2) + pixel)) & 0x3F];
    }

    public byte CpuRead(ushort addr, bool bReadOnly = false)
    {
        byte data = 0x00;

        if (bReadOnly)
        {
            switch ((PpuAddrStates)addr)
            {
                case PpuAddrStates.Control:
                    data = _control.reg;
                    break;
                case PpuAddrStates.Mask:
                    data = _mask.reg;
                    break;
                case PpuAddrStates.Status:
                    data = _status.reg;
                    break;
                case PpuAddrStates.OamAddress:
                    break;
                case PpuAddrStates.OamData:
                    break;
                case PpuAddrStates.Scroll:
                    break;
                case PpuAddrStates.PpuAddress:
                    break;
                case PpuAddrStates.PpuData:
                    break;
            }
        }
        else
        {
            switch ((PpuAddrStates)addr)
            {
                case PpuAddrStates.Control:
                    break;
                case PpuAddrStates.Mask:
                    break;
                case PpuAddrStates.Status:
                    data = (byte)((_status.reg & 0xE0) | (_ppuDataBuffer & 0x1F));
                    _status.VerticalBlank = false;
                    _addrLatch = 0;
                    break;
                case PpuAddrStates.OamAddress:
                    break;
                case PpuAddrStates.OamData:
                    break;
                case PpuAddrStates.Scroll:
                    break;
                case PpuAddrStates.PpuAddress:
                    break;
                case PpuAddrStates.PpuData:
                    data = _ppuDataBuffer;
                    _ppuDataBuffer = PpuRead(_vramAddr.Reg);

                    if (_vramAddr.Reg > 0x3F00) data = _ppuDataBuffer;
                    _vramAddr.Reg += (ushort)(_control.incrementMode ? 32 : 1);
                    break;
            }
        }

        return data;
    }

    public void CpuWrite(ushort addr, byte data)
    {
        switch ((PpuAddrStates)addr)
        {
            case PpuAddrStates.Control:
                _control.reg = data;
                _tramAddr.NametableX = _control.nametableX;
                _tramAddr.NametableY = _control.nametableY;
                break;
            case PpuAddrStates.Mask:
                _mask.reg = data;
                break;
            case PpuAddrStates.Status:
                break;
            case PpuAddrStates.OamAddress:
                break;
            case PpuAddrStates.OamData:
                break;
            case PpuAddrStates.Scroll:
                if (_addrLatch == 0)
                {
                    _fineX = (byte)(data & 0x07);
                    _tramAddr.CoarseX = (byte)(data >> 3);
                    _addrLatch = 1;
                }
                else
                {
                    _tramAddr.FineY = (byte)(data & 0x07);
                    _tramAddr.CoarseY = (byte)(data >> 3);
                    _addrLatch = 0;
                }

                break;
            case PpuAddrStates.PpuAddress:
                if (_addrLatch == 0)
                {
                    _tramAddr.Reg = (ushort)((ushort)((data & 0x3F) << 8) | (_tramAddr.Reg & 0x00FF));
                    _addrLatch = 1;
                }
                else
                {
                    _tramAddr.Reg = (ushort)((_tramAddr.Reg & 0xFF00) | data);
                    _vramAddr = _tramAddr;
                    _addrLatch = 0;
                }

                break;
            case PpuAddrStates.PpuData:
                PpuWrite(_vramAddr.Reg, data);
                _vramAddr.Reg += (ushort)(_control.incrementMode ? 32 : 1);
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
        else if (addr is >= 0x0000 and <= 0x1FFF)
        {
            data = _tblPattern[(ushort)((addr & 0x1000) >> 12)][(ushort)(addr & 0x0FFF)];
        }
        else if (addr is >= 0x2000 and <= 0x3EFF)
        {
            addr &= 0x0FFF;

            if (_cart.Mirror == Mirror.Vertical)
            {
                if (addr <= 0x03FF)
                    data = _tblName[0][addr & 0x03FF];
                else if (addr <= 0x07FF)
                    data = _tblName[1][addr & 0x03FF];
                else if (addr <= 0x0BFF)
                    data = _tblName[0][addr & 0x03FF];
                else if (addr <= 0x0FFF)
                    data = _tblName[1][addr & 0x03FF];
            }
            else if (_cart.Mirror == Mirror.Horizontal)
            {
                if (addr <= 0x03FF)
                    data = _tblName[0][addr & 0x03FF];
                else if (addr <= 0x07FF)
                    data = _tblName[0][addr & 0x03FF];
                else if (addr <= 0x0BFF)
                    data = _tblName[1][addr & 0x03FF];
                else if (addr <= 0x0FFF)
                    data = _tblName[1][addr & 0x03FF];
            }
        }
        else if (addr is >= 0x3F00 and <= 0x3FFF)
        {
            addr &= 0x001F;
            if (addr == 0x0010) addr = 0x0000;
            if (addr == 0x0014) addr = 0x0004;
            if (addr == 0x0018) addr = 0x0008;
            if (addr == 0x001C) addr = 0x000C;

            data = (byte)(_tblPalette[addr]);
        }

        return data;
    }

    public void PpuWrite(ushort addr, byte data)
    {
        addr &= 0x3FFF;

        if (_cart.PpuWrite(addr, data))
        {
        }
        else if (addr is >= 0x0000 and <= 0x1FFF)
        {
            _tblPattern[(ushort)((addr & 0x1000) >> 12)][(ushort)(addr & 0x0FFF)] = data;
        }
        else if (addr is >= 0x2000 and <= 0x3EFF)
        {
            addr &= 0x0FFF;

            if (_cart.Mirror == Mirror.Vertical)
            {
                if (addr <= 0x03FF)
                    _tblName[0][addr & 0x03FF] = data;
                else if (addr <= 0x07FF)
                    _tblName[1][addr & 0x03FF] = data;
                else if (addr <= 0x0BFF)
                    _tblName[0][addr & 0x03FF] = data;
                else if (addr <= 0x0FFF)
                    _tblName[1][addr & 0x03FF] = data;
            }
            else if (_cart.Mirror == Mirror.Horizontal)
            {
                if (addr <= 0x03FF)
                    _tblName[0][addr & 0x03FF] = data;
                else if (addr <= 0x07FF)
                    _tblName[0][addr & 0x03FF] = data;
                else if (addr <= 0x0BFF)
                    _tblName[1][addr & 0x03FF] = data;
                else if (addr <= 0x0FFF)
                    _tblName[1][addr & 0x03FF] = data;
            }
        }
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

    public void Reset()
    {
        _fineX = 0x00;
        _addrLatch = 0x00;
        _ppuDataBuffer = 0x00;
        _scanline = 0;
        _cycle = 0;
        _bgNextTileId = 0x00;
        _bgNextTileAttrb = 0x00;
        _bgNextTileLsb = 0x00;
        _bgNextTileMsb = 0x00;
        _bgShifterPatternLo = 0x0000;
        _bgShifterPatternHi = 0x0000;
        _bgShifterAttribLo = 0x0000;
        _bgShifterAttribHi = 0x0000;
        
        _status.reg = 0x00;
        _mask.reg = 0x00;
        _control.reg = 0x00;
        _vramAddr.Reg = 0x0000;
        _tramAddr.Reg = 0x0000;
    }
}