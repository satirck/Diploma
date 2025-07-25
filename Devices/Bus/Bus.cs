using Devices.CPU;
using Devices.PPU;
using System.IO;

namespace Devices.Bus;

public class Bus
{
    public Cpu6502 Cpu;

    private byte[] _cpuRam = new byte[2048];
    private uint _nSystemClockCounter;

    public Ppu2C02 Ppu;
    private Cartridge.Cartridge _cart;

    public byte[] Controller = [0, 0];
    private byte[] _controllerState = [0, 0];

    // DMA SETUP
    private byte _dmaPage = 0x00;
    private byte _dmaAddr = 0x00;
    private byte _dmaData = 0x00;
    private bool _dmaTransfer = false;
    private bool _dmaDummy =  true;
    
    public Bus()
    {
        Array.Fill(_cpuRam, (byte)0x00);
        
        Cpu = new Cpu6502();
        Cpu.ConnectBus(this);
        
        Ppu = new Ppu2C02();
    }

    public void InsertCartridge(Cartridge.Cartridge cartridge)
    {
        _cart = cartridge;
        Ppu.ConnectCart(cartridge);
        
        Cpu.Reset();
    }

    public void Reset()
    {
        _cart.Reset();
        Cpu.Reset();
        Ppu.Reset();
        
        _nSystemClockCounter = 0;
    }

    // Методы для сохранения/загрузки состояния
    public void SaveState(string filename)
    {
        using var writer = new BinaryWriter(File.Open(filename, FileMode.Create));
        
        // Сохраняем версию формата сохранения
        writer.Write("NES_SAVE_v1.0");
        
        // Сохраняем состояние Bus
        writer.Write(_nSystemClockCounter);
        writer.Write(_cpuRam);
        writer.Write(Controller);
        writer.Write(_controllerState);
        writer.Write(_dmaPage);
        writer.Write(_dmaAddr);
        writer.Write(_dmaData);
        writer.Write(_dmaTransfer);
        writer.Write(_dmaDummy);
        
        // Сохраняем состояние CPU
        Cpu.SaveState(writer);
        
        // Сохраняем состояние PPU
        Ppu.SaveState(writer);
        
        // Сохраняем состояние картриджа
        _cart.SaveState(writer);
    }

    public void LoadState(string filename)
    {
        using var reader = new BinaryReader(File.Open(filename, FileMode.Open));
        
        // Проверяем версию формата
        string version = reader.ReadString();
        if (version != "NES_SAVE_v1.0")
        {
            throw new InvalidOperationException($"Unsupported save format: {version}");
        }
        
        // Загружаем состояние Bus
        _nSystemClockCounter = reader.ReadUInt32();
        _cpuRam = reader.ReadBytes(2048);
        Controller = reader.ReadBytes(2);
        _controllerState = reader.ReadBytes(2);
        _dmaPage = reader.ReadByte();
        _dmaAddr = reader.ReadByte();
        _dmaData = reader.ReadByte();
        _dmaTransfer = reader.ReadBoolean();
        _dmaDummy = reader.ReadBoolean();
        
        // Загружаем состояние CPU
        Cpu.LoadState(reader);
        
        // Загружаем состояние PPU
        Ppu.LoadState(reader);
        
        // Загружаем состояние картриджа
        _cart.LoadState(reader);
    }

    public void Clock()
    {
        Ppu.Clock();
        if (_nSystemClockCounter % 3 == 0)
        {
            if (_dmaTransfer)
            {
                if (_dmaDummy)
                {
                    if (_nSystemClockCounter % 2 == 1)
                    {
                        _dmaDummy = false;
                    }
                }
                else
                {
                    if (_nSystemClockCounter % 2 == 0)
                    {
                        ushort addr = (ushort)((_dmaPage << 8) | _dmaAddr);
                        _dmaData = CpuRead(addr);
                    }
                    else
                    {
                        Ppu.OAMAsBytes[_dmaAddr] = _dmaData;
                        _dmaAddr++;

                        if (_dmaAddr == 0x00)
                        {
                            _dmaTransfer = false;
                            _dmaDummy = true;
                        }
                    }
                }
            }
            else
            {
                Cpu.Clock();   
            }
        }

        if (Ppu.Nmi)
        {
            Ppu.Nmi = false;
            Cpu.Nmi();
        }

        _nSystemClockCounter++;
    }
    
    public void CpuWrite(ushort addr, byte data)
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
        else if (addr == 0x4014)
        {
            _dmaPage = data;
            _dmaAddr = 0x00;
            _dmaTransfer = true;
        }
        else if (addr is >= 0x4016 and <= 0x4017)
        {
            _controllerState[addr & 0x0001] = Controller[addr & 0x0001];
        }
    }

    public byte CpuRead(ushort addr, bool bReadOnly = false)
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
        else if (addr is >= 0x4016 and <= 0x4017)
        {
            data = (_controllerState[addr & 0x0001] & 0x80) > 0 ? (byte)1 : (byte)0;
            _controllerState[addr & 0x0001] <<= 1;
        }
        
        return data;
    }
}