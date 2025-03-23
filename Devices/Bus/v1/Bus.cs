using Devices.PPU;

namespace Devices.Bus.v1;

public class Bus: AbstractBus, IBus
{
    private byte[] _cpuRam = new byte[2048];
    private uint _nSystemClockCounter;

    private Ppu2C02 _ppu;
    private Cartridge.Cartridge _cart;
    
    public Bus()
    {
        Array.Fill(_cpuRam, (byte)0x00);
    }

    public void InsertCartridge(Cartridge.Cartridge cartridge)
    {
        _cart = cartridge;
        _ppu.ConnectCart(cartridge);
    }

    public void Reset()
    {
        Cpu.Reset();
        _nSystemClockCounter = 0;
    }

    public override void Clock()
    {
        _ppu.Clock();
        if (_nSystemClockCounter % 3 == 0)
        {
            Cpu.Clock();
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
            _ppu.CpuWrite((ushort)(addr & 0x0007), data);
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
            data = _ppu.CpuRead((ushort)(addr & 0x0007), bReadOnly);
        }
        
        return data;
    }
}