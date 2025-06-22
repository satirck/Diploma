namespace Devices.Mapper.Impl;
using System.IO;

public class Mapper002(byte prgBanks, byte chrBanks) : Mapper(prgBanks, chrBanks)
{
    private byte _prgBank = 0;

    public override bool CpuMapRead(ushort addr, ref uint mappedAddr)
    {
        if (addr >= 0x8000 && addr <= 0xFFFF)
        {
            if (addr >= 0xC000)
            {
                // Fixed to last bank
                mappedAddr = (uint)((NPrgBanks - 1) * 0x4000 + (addr & 0x3FFF));
            }
            else
            {
                // Switchable bank
                mappedAddr = (uint)(_prgBank * 0x4000 + (addr & 0x3FFF));
            }
            return true;
        }
        return false;
    }

    public override bool CpuMapWrite(ushort addr, ref uint mappedAddr)
    {
        if (addr >= 0x8000 && addr <= 0xFFFF)
        {
            _prgBank = (byte)(addr & 0x0F);
            return false;
        }
        return false;
    }

    public override bool PpuMapRead(ushort addr, ref uint mappedAddr)
    {
        if (addr >= 0x0000 && addr <= 0x1FFF)
        {
            mappedAddr = addr;
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

    public override void Reset()
    {
        _prgBank = 0;
    }

    public override void SaveState(BinaryWriter writer)
    {
        base.SaveState(writer);
        writer.Write(_prgBank);
    }

    public override void LoadState(BinaryReader reader)
    {
        base.LoadState(reader);
        _prgBank = reader.ReadByte();
    }
} 