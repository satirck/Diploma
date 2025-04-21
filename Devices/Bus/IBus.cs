using Devices.CPU;

namespace Devices.Bus;

public interface IBus
{
    public Cpu6502 Cpu {get; set; }

    // public int Ticks {get; set; }

    public void CpuWrite(ushort addr, byte data);
    public byte CpuRead(ushort addr, bool bReadOnly = false);

    public void Clock();
}