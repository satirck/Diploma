namespace Devices.Mapper.Impl;

public class Mapper004(byte prgBanks, byte chrBanks) : Mapper(prgBanks, chrBanks)
{
    private byte _register = 0;
    private byte[] _registers = new byte[8];
    private byte _prgMode = 0;
    private byte _chrMode = 0;
    private bool _irqActive = false;
    private byte _irqCounter = 0;
    private byte _irqLatch = 0;
    private bool _irqReload = false;
    private byte[] _prgRam = new byte[0x2000];

    public override bool CpuMapRead(ushort addr, ref uint mappedAddr)
    {
        if (addr >= 0x6000 && addr <= 0x7FFF)
        {
            mappedAddr = (uint)(addr & 0x1FFF);
            return true;
        }

        if (addr >= 0x8000 && addr <= 0xFFFF)
        {
            if (_prgMode == 0)
            {
                if (addr >= 0x8000 && addr <= 0x9FFF)
                {
                    mappedAddr = (uint)(_registers[6] * 0x2000 + (addr & 0x1FFF));
                }
                else if (addr >= 0xA000 && addr <= 0xBFFF)
                {
                    mappedAddr = (uint)(_registers[7] * 0x2000 + (addr & 0x1FFF));
                }
                else if (addr >= 0xC000 && addr <= 0xDFFF)
                {
                    mappedAddr = (uint)((NPrgBanks - 2) * 0x2000 + (addr & 0x1FFF));
                }
                else
                {
                    mappedAddr = (uint)((NPrgBanks - 1) * 0x2000 + (addr & 0x1FFF));
                }
            }
            else
            {
                if (addr >= 0x8000 && addr <= 0x9FFF)
                {
                    mappedAddr = (uint)((NPrgBanks - 2) * 0x2000 + (addr & 0x1FFF));
                }
                else if (addr >= 0xA000 && addr <= 0xBFFF)
                {
                    mappedAddr = (uint)(_registers[6] * 0x2000 + (addr & 0x1FFF));
                }
                else if (addr >= 0xC000 && addr <= 0xDFFF)
                {
                    mappedAddr = (uint)(_registers[7] * 0x2000 + (addr & 0x1FFF));
                }
                else
                {
                    mappedAddr = (uint)((NPrgBanks - 1) * 0x2000 + (addr & 0x1FFF));
                }
            }
            return true;
        }

        return false;
    }

    public override bool CpuMapWrite(ushort addr, ref uint mappedAddr)
    {
        if (addr >= 0x6000 && addr <= 0x7FFF)
        {
            mappedAddr = (uint)(addr & 0x1FFF);
            return true;
        }

        if (addr >= 0x8000 && addr <= 0x9FFF)
        {
            if ((addr & 0x01) == 0)
            {
                _register = (byte)(addr & 0x07);
                _prgMode = (byte)((addr >> 6) & 0x01);
                _chrMode = (byte)((addr >> 7) & 0x01);
            }
            else
            {
                _registers[_register] = (byte)(addr & 0xFF);
            }
            return false;
        }

        if (addr >= 0xA000 && addr <= 0xBFFF)
        {
            if ((addr & 0x01) == 0)
            {
                // Mirroring
                if ((addr & 0x01) == 0)
                {
                    // TODO: Implement mirroring
                }
            }
            else
            {
                // PRG RAM protect
                // TODO: Implement PRG RAM protection
            }
            return false;
        }

        if (addr >= 0xC000 && addr <= 0xDFFF)
        {
            if ((addr & 0x01) == 0)
            {
                _irqLatch = (byte)(addr & 0xFF);
            }
            else
            {
                _irqCounter = 0;
                _irqReload = true;
            }
            return false;
        }

        if (addr >= 0xE000 && addr <= 0xFFFF)
        {
            if ((addr & 0x01) == 0)
            {
                _irqActive = false;
            }
            else
            {
                _irqActive = true;
            }
            return false;
        }

        return false;
    }

    public override bool PpuMapRead(ushort addr, ref uint mappedAddr)
    {
        if (addr >= 0x0000 && addr <= 0x1FFF)
        {
            if (_chrMode == 0)
            {
                if (addr <= 0x07FF)
                {
                    mappedAddr = (uint)(_registers[0] * 0x0400 + (addr & 0x03FF));
                }
                else if (addr <= 0x0FFF)
                {
                    mappedAddr = (uint)(_registers[1] * 0x0400 + (addr & 0x03FF));
                }
                else if (addr <= 0x13FF)
                {
                    mappedAddr = (uint)(_registers[2] * 0x0400 + (addr & 0x03FF));
                }
                else if (addr <= 0x17FF)
                {
                    mappedAddr = (uint)(_registers[3] * 0x0400 + (addr & 0x03FF));
                }
                else if (addr <= 0x1BFF)
                {
                    mappedAddr = (uint)(_registers[4] * 0x0400 + (addr & 0x03FF));
                }
                else
                {
                    mappedAddr = (uint)(_registers[5] * 0x0400 + (addr & 0x03FF));
                }
            }
            else
            {
                if (addr <= 0x0FFF)
                {
                    mappedAddr = (uint)((_registers[0] & 0xFE) * 0x0400 + (addr & 0x07FF));
                }
                else
                {
                    mappedAddr = (uint)((_registers[2] & 0xFE) * 0x0400 + (addr & 0x07FF));
                }
            }
            return true;
        }

        return false;
    }

    public override bool PpuMapWrite(ushort addr, ref uint mappedAddr)
    {
        if (addr >= 0x0000 && addr <= 0x1FFF)
        {
            if (NChrBanks == 0)
            {
                mappedAddr = addr;
                return true;
            }
        }

        return false;
    }

    public override bool IrqState()
    {
        return _irqActive;
    }

    public override void IrqClear()
    {
        _irqActive = false;
    }

    public override void Scanline()
    {
        if (_irqCounter == 0 || _irqReload)
        {
            _irqCounter = _irqLatch;
            _irqReload = false;
        }
        else
        {
            _irqCounter--;
        }

        if (_irqCounter == 0 && _irqActive)
        {
            _irqActive = true;
        }
    }

    public override void Reset()
    {
        _register = 0;
        _prgMode = 0;
        _chrMode = 0;
        _irqActive = false;
        _irqCounter = 0;
        _irqLatch = 0;
        _irqReload = false;
    }
} 