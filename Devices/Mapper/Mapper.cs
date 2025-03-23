namespace Devices.Mapper;

public abstract class Mapper
{
    protected byte NPrgBanks = 0;
    protected byte NChrBanks = 0;
    
    public Mapper(byte prgBanks, byte chrBanks)
    {
        NPrgBanks = prgBanks;
        NChrBanks = chrBanks;
    }
    
    public abstract bool CpuMapRead(ushort addr, ref uint mappedAddr);
    public abstract bool CpuMapWrite(ushort addr, ref uint mappedAddr);
    public abstract bool PpuMapRead(ushort addr, ref uint mappedAddr);
    public abstract bool PpuMapWrite(ushort addr, ref uint mappedAddr);
}