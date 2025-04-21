using System.Numerics;

namespace Devices.PPU;

using Cartridge;

public partial class Ppu2C02
{
    private Cartridge _cart;
    
    private Status _status;
    private Mask _mask;
    private PpuCtrl _control;
    private LoopyRegister _vramAddr;
    private LoopyRegister _tramAddr;

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
    
    private byte _addrLatch = 0x00;
    private byte _ppuDataBuffer = 0x00;
    private ushort _ppuAddr = 0x0000;
    
    private Random _random = new Random();
    
    public bool FrameComplete = false;
    public bool Nmi = false;
        
    public void ConnectCart(Cartridge cartridge)
    {
        _cart = cartridge;
    }

    public void Clock()
    {
        if (_scanline == -1 && _cycle == -1)
        {
            _status.VerticalBlank = false;
        }
        
        if (_scanline == 241 && _cycle == 1)
        {
            _status.VerticalBlank = true;
            if (_control.enableNmi) { Nmi = true; }
        }
        
        _sprScreen.SetPixel(_cycle - 1, _scanline, _palScreen[_random.Next() % 2 == 0 ? 0x3F : 0x30]);
        
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
        return _palScreen[PpuRead((ushort)(0x3F00 + (palette << 2) + pixel))];
    }

    public byte CpuRead(ushort addr, bool bReadOnly = false)
    {
        byte data = 0x00;

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
                _ppuDataBuffer = PpuRead(_ppuAddr);

                if (_ppuAddr > 0x3F00) data = _ppuDataBuffer;
                _ppuAddr++;
                break;
        }

        return data;
    }

    public void CpuWrite(ushort addr, byte data)
    {
        switch ((PpuAddrStates)addr)
        {
            case PpuAddrStates.Control:
                _control.reg = data;
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
                break;
            case PpuAddrStates.PpuAddress:
                if (_addrLatch == 0)
                {
                    _ppuAddr = (ushort)((_ppuAddr & 0x00FF) | (data << 8));
                    _addrLatch = 1;
                }
                else
                {
                    _ppuAddr = (ushort)((_ppuAddr & 0xFF00) | data);
                    _addrLatch = 0;
                }
                break;
            case PpuAddrStates.PpuData:
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
        else if (addr is >= 0x0000 and <= 0x1FFF)
        {
            data = _tblPattern[(ushort)((addr & 0x1000) >> 12)][(ushort)(addr & 0x0FFF)];
            
        }
        else if (addr is >= 0x2000 and <= 0x3EFF)
        {
            
        }    
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
        else if (addr is >= 0x0000 and <= 0x1FFF)
        {
            _tblPattern[(ushort)((addr & 0x1000) >> 12)][(ushort)(addr & 0x0FFF)] = data;
        }
        else if (addr is >= 0x2000 and <= 0x3EFF)
        {
            
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
}