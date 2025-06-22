using System.Runtime.InteropServices;
using Devices.PPU.Registers;
using System.IO;

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
    
    private ObjectAttributeEntry[] OAM = new ObjectAttributeEntry[64];
    public Span<byte> OAMAsBytes => MemoryMarshal.AsBytes<ObjectAttributeEntry>(OAM.AsSpan());
    public byte OamAddr = 0x00;
    
    private ObjectAttributeEntry[] _spriteScanline = new ObjectAttributeEntry[8];
    private byte _spriteCount = 0;
    
    private byte[] _spriteShifterPatternLo = new byte[8];
    private byte[] _spriteShifterPatternHi = new byte[8];

    // Sprite Zero Collision Flags
    private bool _bSpriteZeroHitPossible = false;
    private bool _bSpriteZeroBeingRendered = false;

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

    // Методы для сохранения/загрузки состояния
    public void SaveState(BinaryWriter writer)
    {
        // Сохраняем основные переменные состояния
        writer.Write(_scanline);
        writer.Write(_cycle);
        writer.Write(_status.Reg);
        writer.Write(_mask.Reg);
        writer.Write(_control.Reg);
        writer.Write(_vramAddr.Reg);
        writer.Write(_tramAddr.Reg);
        writer.Write(OamAddr);
        writer.Write(_spriteCount);
        writer.Write(_fineX);
        writer.Write(_addressLatch);
        writer.Write(_ppuDataBuffer);
        writer.Write(_bSpriteZeroHitPossible);
        writer.Write(_bSpriteZeroBeingRendered);
        writer.Write(FrameComplete);
        writer.Write(Nmi);

        // Сохраняем массивы данных
        writer.Write(_spriteShifterPatternLo);
        writer.Write(_spriteShifterPatternHi);
        writer.Write(_tblPalette);
        
        // Сохраняем двумерные массивы
        for (int i = 0; i < 2; i++)
        {
            writer.Write(_tblName[i]);
        }
        
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 4096; j++)
            {
                writer.Write(_tblPattern[i, j]);
            }
        }

        // Сохраняем OAM
        for (int i = 0; i < 64; i++)
        {
            writer.Write(OAM[i].Y);
            writer.Write(OAM[i].ID);
            writer.Write(OAM[i].Attribute);
            writer.Write(OAM[i].X);
        }

        // Сохраняем sprite scanline
        for (int i = 0; i < 8; i++)
        {
            writer.Write(_spriteScanline[i].Y);
            writer.Write(_spriteScanline[i].ID);
            writer.Write(_spriteScanline[i].Attribute);
            writer.Write(_spriteScanline[i].X);
        }

        // Сохраняем background rendering данные
        writer.Write(_bgNextTileId);
        writer.Write(_bgNextTileAttrib);
        writer.Write(_bgNextTileLsb);
        writer.Write(_bgNextTileMsb);
        writer.Write(_bgShifterPatternLo);
        writer.Write(_bgShifterPatternHi);
        writer.Write(_bgShifterAttribLo);
        writer.Write(_bgShifterAttribHi);
    }

    public void LoadState(BinaryReader reader)
    {
        // Загружаем основные переменные состояния
        _scanline = reader.ReadInt16();
        _cycle = reader.ReadInt16();
        _status.Reg = reader.ReadByte();
        _mask.Reg = reader.ReadByte();
        _control.Reg = reader.ReadByte();
        _vramAddr.Reg = reader.ReadUInt16();
        _tramAddr.Reg = reader.ReadUInt16();
        OamAddr = reader.ReadByte();
        _spriteCount = reader.ReadByte();
        _fineX = reader.ReadByte();
        _addressLatch = reader.ReadByte();
        _ppuDataBuffer = reader.ReadByte();
        _bSpriteZeroHitPossible = reader.ReadBoolean();
        _bSpriteZeroBeingRendered = reader.ReadBoolean();
        FrameComplete = reader.ReadBoolean();
        Nmi = reader.ReadBoolean();

        // Загружаем массивы данных
        _spriteShifterPatternLo = reader.ReadBytes(8);
        _spriteShifterPatternHi = reader.ReadBytes(8);
        _tblPalette = reader.ReadBytes(32);
        
        // Загружаем двумерные массивы
        for (int i = 0; i < 2; i++)
        {
            _tblName[i] = reader.ReadBytes(1024);
        }
        
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 4096; j++)
            {
                _tblPattern[i, j] = reader.ReadByte();
            }
        }

        // Загружаем OAM
        for (int i = 0; i < 64; i++)
        {
            OAM[i].Y = reader.ReadByte();
            OAM[i].ID = reader.ReadByte();
            OAM[i].Attribute = reader.ReadByte();
            OAM[i].X = reader.ReadByte();
        }

        // Загружаем sprite scanline
        for (int i = 0; i < 8; i++)
        {
            _spriteScanline[i].Y = reader.ReadByte();
            _spriteScanline[i].ID = reader.ReadByte();
            _spriteScanline[i].Attribute = reader.ReadByte();
            _spriteScanline[i].X = reader.ReadByte();
        }

        // Загружаем background rendering данные
        _bgNextTileId = reader.ReadByte();
        _bgNextTileAttrib = reader.ReadByte();
        _bgNextTileLsb = reader.ReadByte();
        _bgNextTileMsb = reader.ReadByte();
        _bgShifterPatternLo = reader.ReadUInt16();
        _bgShifterPatternHi = reader.ReadUInt16();
        _bgShifterAttribLo = reader.ReadUInt16();
        _bgShifterAttribHi = reader.ReadUInt16();
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
        if (_mask.RenderBackground || _mask.RenderSprites)
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
        if (_mask.RenderBackground || _mask.RenderSprites)
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
        if (_mask.RenderBackground || _mask.RenderSprites)
        {
            _vramAddr.Reg = (ushort)((_vramAddr.Reg & 0xFBE0) | (_tramAddr.Reg & 0x041F));
        }
    }

    private void TransferAddressY()
    {
        if (_mask.RenderBackground || _mask.RenderSprites)
        {
            _vramAddr.FineY      = _tramAddr.FineY;
            _vramAddr.NametableY = _tramAddr.NametableY;
            _vramAddr.CoarseY    = _tramAddr.CoarseY;
        }
    }

    private void LoadBackgroundShifters()
    {	
        _bgShifterPatternLo = (ushort)((_bgShifterPatternLo & 0xFF00) | _bgNextTileLsb);
        _bgShifterPatternHi = (ushort)((_bgShifterPatternHi & 0xFF00) | _bgNextTileMsb);

        _bgShifterAttribLo  = (ushort)((_bgShifterAttribLo & 0xFF00) | ((_bgNextTileAttrib & 0b01) != 0 ? 0xFF : 0x00));
        _bgShifterAttribHi  = (ushort)((_bgShifterAttribHi & 0xFF00) | ((_bgNextTileAttrib & 0b10) != 0 ? 0xFF : 0x00));
    }

    private void UpdateShifters()
    {
        if (_mask.RenderBackground)
        {
            _bgShifterPatternLo <<= 1;
            _bgShifterPatternHi <<= 1;

            _bgShifterAttribLo <<= 1;
            _bgShifterAttribHi <<= 1;
        }
        
        if (_mask.RenderSprites && _cycle >= 1 && _cycle < 258)
        {
            for (int i = 0; i < _spriteCount; i++)
			{
                if (_spriteScanline[i].X > 0)
                {
                    _spriteScanline[i].X--;
				}
				else
				{
                    _spriteShifterPatternLo[i] <<= 1;
                    _spriteShifterPatternHi[i] <<= 1;
				}
			}
		}
    }
}