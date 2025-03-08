namespace Devices.Bus;

using CPU;

public class Bus: IBus
{
    private const int RamSize = 1024 * 64;
    
    private byte[] _ram = new byte[RamSize];

    public Cpu6502 Cpu;
    
    public Bus()
    {
        Array.Fill(_ram, (byte)0x00);
        
        // Connecting the bus, later maybe change it
        Cpu = new ();
        Cpu.ConnectBus(this);
    }

    public void Dispose()
    {
        //TODO: Later
    }
    
    public void Write(ushort addr, byte value)
    {
        if (addr >= 0x0000 && addr <= 0xFFFF)
        {
            _ram[addr] = value;       
        }
    }

    public byte Read(ushort addr, bool bReadOnly = false)
    {
        if (addr >= 0x0000 && addr <= 0xFFFF)
        {
            return _ram[addr];
        }
        
        return 0x00;
    }
}