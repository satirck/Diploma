namespace Devices.Mapper.Impl;

public class Mapper000 : Mapper
{
    public Mapper000(byte prgBanks, byte chrBanks) : base(prgBanks, chrBanks) {}
    
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
            mappedAddr = (uint)(addr & (NPrgBanks > 1 ? 0x7FFF : 0x3FFF));
            return true;
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
        // if (addr >= 0x0000 && addr <= 0x1FFF)
        // {
        //     if (NChrBanks == 0)
        //     {
        //         mappedAddr = addr;
        //         return true;
        //     }
        // }
        return false;
    }
}