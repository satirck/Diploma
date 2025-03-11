namespace Devices.Bus;

using CPU;
using PPU;
using Cartridge;

public class Bus: IBus
{
    private byte[] _cpuRam = new byte[2048];
    private uint _nSystemClockCounter;

    private Cpu6502 _cpu;
    private Ppu2C02 _ppu;
    private Cartridge _cart;
    
    public Bus()
    {
        Array.Fill(_cpuRam, (byte)0x00);
        
        // Connecting the bus, later maybe change it
        _cpu = new ();
        _cpu.ConnectBus(this);
    }

    public void InsertCartridge(Cartridge cartridge)
    {
        _cart = cartridge;
        _ppu.ConnectCart(cartridge);
    }

    public void Reset()
    {
        _cpu.Reset();
        _nSystemClockCounter = 0;
    }

    public void Clock()
    {
        
    }
    
    public void CpuWrite(ushort addr, byte data)
    {
        if (addr is >= 0x0000 and <= 0x1FFF)
        {
            _cpuRam[addr & 0x07FF] = data;       
        } 
        else if (addr is >= 0x2000 and <= 0x3FFF)
        {
            _ppu.CpuWrite((ushort)(addr & 0x0007), data);
        }
    }

    public byte CpuRead(ushort addr, bool bReadOnly = false)
    {
        byte data = 0x00;
        
        if (addr is >= 0x0000 and <= 0x1FFF)
        {
            data = _cpuRam[addr & 0x07FF];
        }
        else if (addr is >= 0x2000 and <= 0x3FFF)
        {
            data = _ppu.CpuRead((ushort)(addr & 0x0007), bReadOnly);
        }
        
        return data;
    }
}