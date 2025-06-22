using Devices.PPU;
using System.IO;

namespace Devices.Mapper.Impl;

using static Mirror;

public class Mapper001(byte prgBanks, byte chrBanks) : Mapper(prgBanks, chrBanks)
{
    private byte _loadRegister = 0x00;
    private byte _loadRegisterCount = 0x00;
    private byte _control = 0x1C;
    private byte _prgBank = 0x00;
    private byte _chrBank0 = 0x00;
    private byte _chrBank1 = 0x00;
    private byte[] _prgRam = new byte[0x2000];

    public override bool CpuMapRead(ushort addr, ref uint mappedAddr)
    {
        if (addr >= 0x6000 && addr <= 0x7FFF)
        {
            mappedAddr = (uint)(addr & 0x1FFF);
            return true;
        }

        if (addr >= 0x8000 && addr <= 0xFFFF)
        {
            if ((_control & 0x08) != 0)
            {
                // 16KB Mode
                if (addr >= 0x8000 && addr <= 0xBFFF)
                {
                    mappedAddr = (uint)((_prgBank & 0x0E) * 0x4000 + (addr & 0x3FFF));
                }
                else
                {
                    mappedAddr = (uint)(0x0F * 0x4000 + (addr & 0x3FFF));
                }
            }
            else
            {
                // 32KB Mode
                mappedAddr = (uint)((_prgBank & 0x0F) * 0x8000 + (addr & 0x7FFF));
            }
            return true;
        }

        return false;
    }

    public override bool CpuMapWrite(ushort addr, ref uint mappedAddr)
    {
        if (addr >= 0x6000 && addr <= 0x7FFF)
        {
            mappedAddr = (uint)(addr & 0x1FFF);
            return true;
        }

        if (addr >= 0x8000 && addr <= 0xFFFF)
        {
            byte data = (byte)(addr & 0xFF);
            if ((data & 0x80) != 0)
            {
                _loadRegister = 0x00;
                _loadRegisterCount = 0x00;
                _control |= 0x0C;
            }
            else
            {
                _loadRegister >>= 1;
                _loadRegister |= (byte)((data & 0x01) << 4);
                _loadRegisterCount++;

                if (_loadRegisterCount == 5)
                {
                    byte targetRegister = (byte)((addr >> 13) & 0x03);

                    switch (targetRegister)
                    {
                        case 0: // Control
                            _control = _loadRegister;
                            break;
                        case 1: // CHR Bank 0
                            _chrBank0 = _loadRegister;
                            break;
                        case 2: // CHR Bank 1
                            _chrBank1 = _loadRegister;
                            break;
                        case 3: // PRG Bank
                            _prgBank = _loadRegister;
                            break;
                    }

                    _loadRegister = 0x00;
                    _loadRegisterCount = 0x00;
                }
            }
            return true;
        }

        return false;
    }

    public override bool PpuMapRead(ushort addr, ref uint mappedAddr)
    {
        if (addr >= 0x0000 && addr <= 0x1FFF)
        {
            if ((_control & 0x10) != 0)
            {
                // 4KB CHR Bank Mode
                if (addr <= 0x0FFF)
                {
                    mappedAddr = (uint)(_chrBank0 * 0x1000 + (addr & 0x0FFF));
                }
                else
                {
                    mappedAddr = (uint)(_chrBank1 * 0x1000 + (addr & 0x0FFF));
                }
            }
            else
            {
                // 8KB CHR Bank Mode
                mappedAddr = (uint)((_chrBank0 & 0x1E) * 0x1000 + (addr & 0x1FFF));
            }
            return true;
        }

        return false;
    }

    public override bool PpuMapWrite(ushort addr, ref uint mappedAddr)
    {
        if (addr >= 0x0000 && addr <= 0x1FFF)
        {
            if (NChrBanks == 0)
            {
                mappedAddr = addr;
                return true;
            }
        }

        return false;
    }

    public override Mirror Mirror()
    {
        switch (_control & 0x03)
        {
            case 0: return OneScreenLo;
            case 1: return OneScreenHi;
            case 2: return Vertical;
            case 3: return Horizontal;
            default: return Horizontal;
            // default: return Hardware;
        }
    }

    public override void Reset()
    {
        _loadRegister = 0x00;
        _loadRegisterCount = 0x00;
        _control = 0x1C;
        _prgBank = 0x00;
        _chrBank0 = 0x00;
        _chrBank1 = 0x00;
    }

    public override void SaveState(BinaryWriter writer)
    {
        base.SaveState(writer);
        
        // Сохраняем состояние Mapper001
        writer.Write(_loadRegister);
        writer.Write(_loadRegisterCount);
        writer.Write(_control);
        writer.Write(_prgBank);
        writer.Write(_chrBank0);
        writer.Write(_chrBank1);
        writer.Write(_prgRam);
    }

    public override void LoadState(BinaryReader reader)
    {
        base.LoadState(reader);
        
        // Загружаем состояние Mapper001
        _loadRegister = reader.ReadByte();
        _loadRegisterCount = reader.ReadByte();
        _control = reader.ReadByte();
        _prgBank = reader.ReadByte();
        _chrBank0 = reader.ReadByte();
        _chrBank1 = reader.ReadByte();
        _prgRam = reader.ReadBytes(0x2000);
    }
} 