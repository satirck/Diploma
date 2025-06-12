namespace Devices.PPU.Registers;

public struct Control
{
    private byte _reg;

    public byte Reg
    {
        get => _reg;
        set => _reg = value;
    }

    // Bit 0
    public bool NametableX
    {
        get => (_reg & 0b0000_0001) != 0;
        set
        {
            if (value)
                _reg |= 0b0000_0001;
            else
                _reg &= 0b1111_1110;
        }
    }

    // Bit 1
    public bool NametableY
    {
        get => (_reg & 0b0000_0010) != 0;
        set
        {
            if (value)
                _reg |= 0b0000_0010;
            else
                _reg &= 0b1111_1101;
        }
    }

    // Bit 2
    public bool IncrementMode
    {
        get => (_reg & 0b0000_0100) != 0;
        set
        {
            if (value)
                _reg |= 0b0000_0100;
            else
                _reg &= 0b1111_1011;
        }
    }

    // Bit 3
    public bool PatternSprite
    {
        get => (_reg & 0b0000_1000) != 0;
        set
        {
            if (value)
                _reg |= 0b0000_1000;
            else
                _reg &= 0b1111_0111;
        }
    }

    // Bit 4
    public bool PatternBackground
    {
        get => (_reg & 0b0001_0000) != 0;
        set
        {
            if (value)
                _reg |= 0b0001_0000;
            else
                _reg &= 0b1110_1111;
        }
    }

    // Bit 5
    public bool SpriteSize
    {
        get => (_reg & 0b0010_0000) != 0;
        set
        {
            if (value)
                _reg |= 0b0010_0000;
            else
                _reg &= 0b1101_1111;
        }
    }

    // Bit 6 (unused)
    public bool SlaveMode
    {
        get => (_reg & 0b0100_0000) != 0;
        set
        {
            if (value)
                _reg |= 0b0100_0000;
            else
                _reg &= 0b1011_1111;
        }
    }

    // Bit 7
    public bool EnableNmi
    {
        get => (_reg & 0b1000_0000) != 0;
        set
        {
            if (value)
                _reg |= 0b1000_0000;
            else
                _reg &= 0b0111_1111;
        }
    }
}
