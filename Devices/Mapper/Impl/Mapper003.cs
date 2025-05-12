namespace Devices.Mapper.Impl;

public class Mapper003(byte prgBanks, byte chrBanks) : Mapper(prgBanks, chrBanks)
{
    private byte _chrBank = 0;

    public override bool CpuMapRead(ushort addr, ref uint mappedAddr)
    {
        if (addr >= 0x8000 && addr <= 0xFFFF)
        {
            mappedAddr = (uint)(addr & (NPrgBanks > 1 ? 0x7FFF : 0x3FFF));
            return true;
        }
        return false;
    }

    public override bool CpuMapWrite(ushort addr, ref uint mappedAddr)
    {
        if (addr >= 0x8000 && addr <= 0xFFFF)
        {
            _chrBank = (byte)(addr & 0x03);
            return false;
        }
        return false;
    }

    public override bool PpuMapRead(ushort addr, ref uint mappedAddr)
    {
        if (addr >= 0x0000 && addr <= 0x1FFF)
        {
            mappedAddr = (uint)(_chrBank * 0x2000 + addr);
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
        _chrBank = 0;
    }
} 