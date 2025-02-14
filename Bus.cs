namespace NES;

public class Bus: IDisposable
{
    private const int RamSize = 1024 * 64;
    
    private byte[] _ram = new byte[RamSize];

    public Olc6502 Cpu;
    
    public Bus()
    {
        Array.Fill(_ram, (byte)0x00);
        
        // Connecting the bus, later maybe change it
        Cpu = new Olc6502();
        Cpu.ConnectBus(this);
    }

    public void Dispose()
    {
        // TODO: Later
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