using Devices.PPU;

namespace Devices.Bus.v1;

public class Bus: AbstractBus, IBus
{
    private byte[] _cpuRam = new byte[2048];
    private uint _nSystemClockCounter;

    public Ppu2C02 Ppu;
    private Cartridge.Cartridge _cart;
    
    public Bus()
    {
        Array.Fill(_cpuRam, (byte)0x00);
    }

    public void InsertCartridge(Cartridge.Cartridge cartridge)
    {
        _cart = cartridge;
        Ppu.ConnectCart(cartridge);
    }

    public void Reset()
    {
        Cpu.Reset();
        _nSystemClockCounter = 0;
    }

    public override void Clock()
    {
        Ppu.Clock();
        if (_nSystemClockCounter % 3 == 0)
        {
            Cpu.Clock();
        }

        if (Ppu.Nmi)
        {
            Ppu.Nmi = false;
            Cpu.Nmi();
        }

        _nSystemClockCounter++;
    }
    
    public override void CpuWrite(ushort addr, byte data)
    {
        if (_cart.CpuWrite(addr, data))
        {
            
        }
        else if (addr is >= 0x0000 and <= 0x1FFF)
        {
            _cpuRam[addr & 0x07FF] = data;       
        } 
        else if (addr is >= 0x2000 and <= 0x3FFF)
        {
            Ppu.CpuWrite((ushort)(addr & 0x0007), data);
        }
    }

    public override byte CpuRead(ushort addr, bool bReadOnly = false)
    {
        byte data = 0x00;
        
        if (_cart.CpuRead(addr, ref data))
        {
            
        }
        else if (addr is >= 0x0000 and <= 0x1FFF)
        {
            data = _cpuRam[addr & 0x07FF];
        }
        else if (addr is >= 0x2000 and <= 0x3FFF)
        {
            data = Ppu.CpuRead((ushort)(addr & 0x0007), bReadOnly);
        }
        
        return data;
    }
}