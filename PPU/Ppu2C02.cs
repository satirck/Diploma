using System;

public class Ppu2C02
{
    public bool FrameComplete { get; set; }

    // Добавляем методы для доступа к состоянию PPU
    public byte GetScanline() => _scanline;
    public byte GetCycle() => _cycle;
    public PpuStatus GetStatus() => _status;
    public PpuControl GetControl() => _control;

    public void Clock()
    {
        if (_scanline == -1 && _cycle == -1)
        {
            Console.WriteLine($"PPU: Reset VBlank at scanline {_scanline}, cycle {_cycle}");
            _status.VerticalBlank = false;
        }

        if (_scanline == 241 && _cycle == 1)
        {
            Console.WriteLine($"PPU: Set VBlank at scanline {_scanline}, cycle {_cycle}, NMI enabled: {_control.enableNmi}");
            _status.VerticalBlank = true;
            if (_control.enableNmi)
            {
                Console.WriteLine("PPU: Triggering NMI");
                Nmi = true;
            }
        }

        // ... existing code ...

        // Advance renderer - it never stops, it's relentless
        _cycle++;
        if (_cycle >= 341)
        {
            _cycle = 0;
            _scanline++;
            if (_scanline >= 261)
            {
                _scanline = -1;
                FrameComplete = true;
                Console.WriteLine($"PPU: Frame complete, VBlank: {_status.VerticalBlank}, NMI enabled: {_control.enableNmi}");
            }
        }
    }

    public void CpuWrite(ushort addr, byte data)
    {
        switch (addr)
        {
            case 0x0000: // Control
                Console.WriteLine($"PPU: Writing to control register: 0x{data:X2}, NMI enabled: {(data & 0x80) != 0}");
                _control.reg = data;
                break;
            case 0x0001: // Mask
                Console.WriteLine($"PPU: Writing to mask register: 0x{data:X2}, render background: {(data & 0x08) != 0}, render sprites: {(data & 0x10) != 0}");
                _mask.reg = data;
                break;
            case 0x0002: // Status
                Console.WriteLine($"PPU Status Write: 0x{data:X2} (Read-only register)");
                break;
            case 0x0003: // OAM Address
                break;
            case 0x0004: // OAM Data
                break;
            case 0x0005: // Scroll
                break;
            case 0x0006: // PPU Address
                if (_addressLatch == 0)
                {
                    _ppuAddr = (ushort)((_ppuAddr & 0x00FF) | (data << 8));
                    _addressLatch = 1;
                }
                else
                {
                    _ppuAddr = (ushort)((_ppuAddr & 0xFF00) | data);
                    _addressLatch = 0;
                }
                break;
            case 0x0007: // PPU Data
                PpuWrite(_ppuAddr, data);
                _ppuAddr++;
                break;
        }
    }

    public byte CpuRead(ushort addr, bool bReadOnly = false)
    {
        byte data = 0x00;

        switch (addr)
        {
            case 0x0000: // Control
                break;
            case 0x0001: // Mask
                break;
            case 0x0002: // Status
                data = (byte)((_status.reg & 0xE0) | (_ppuDataBuffer & 0x1F));
                Console.WriteLine($"PPU: Reading status register, VBlank: {_status.VerticalBlank}, data: 0x{data:X2}");
                _status.VerticalBlank = false;
                _addressLatch = 0;
                break;
            case 0x0003: // OAM Address
                break;
            case 0x0004: // OAM Data
                break;
            case 0x0005: // Scroll
                break;
            case 0x0006: // PPU Address
                break;
            case 0x0007: // PPU Data
                data = _ppuDataBuffer;
                _ppuDataBuffer = PpuRead(_ppuAddr);

                if (_ppuAddr > 0x3F00) data = _ppuDataBuffer;
                _ppuAddr++;
                break;
        }

        return data;
    }
} 