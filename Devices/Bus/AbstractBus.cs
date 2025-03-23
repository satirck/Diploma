using Devices.CPU;

namespace Devices.Bus;

public abstract class AbstractBus
{
    private Cpu6502 _cpu;

    public Cpu6502 Cpu { get => _cpu; set => _cpu = value; }
    
    public AbstractBus()
    {
        _cpu = new ();
        _cpu.ConnectBus((IBus)this);
    }

    public abstract void CpuWrite(ushort addr, byte data);

    public abstract byte CpuRead(ushort addr, bool bReadOnly = false);
    
    public abstract void Clock();
}