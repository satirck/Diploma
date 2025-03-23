namespace Devices.Bus.v1;

public class SimpleBus: AbstractBus, IBus
{
    public byte[] CpuRam = new byte[64 * 1024];
    
    public SimpleBus()
    {
        Array.Fill(CpuRam, (byte)0x00);
    }

    public void Reset()
    {
        Cpu.Reset();
    }

    public override void Clock()
    {
       Cpu.Clock();
    }

    public override void CpuWrite(ushort addr, byte data)
    {
        CpuRam[addr] = data;
    }

    public override byte CpuRead(ushort addr, bool bReadOnly = false)
    {
        return CpuRam[addr];
    }
}