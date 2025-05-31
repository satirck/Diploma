using Devices.PPU;

namespace Devices.Mapper;

using static Mirror;

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

    public virtual void Reset()
    {
    }

    public virtual Mirror Mirror()
    {
        return Horizontal;
    }

    public virtual bool IrqState()
    {
        return false;
    }

    public virtual void IrqClear()
    {
    }

    public virtual void Scanline()
    {
    }
}