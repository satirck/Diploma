namespace Devices.PPU.Registers;

public struct Mask
{
    private byte reg;

    public byte Reg
    {
        get => reg;
        set => reg = value;
    }

    // Bit 0
    public bool Grayscale
    {
        get => (reg & 0b0000_0001) != 0;
        set
        {
            if (value)
                reg |= 0b0000_0001;
            else
                reg &= 0b1111_1110;
        }
    }

    // Bit 1
    public bool RenderBackgroundLeft
    {
        get => (reg & 0b0000_0010) != 0;
        set
        {
            if (value)
                reg |= 0b0000_0010;
            else
                reg &= 0b1111_1101;
        }
    }

    // Bit 2
    public bool RenderSpritesLeft
    {
        get => (reg & 0b0000_0100) != 0;
        set
        {
            if (value)
                reg |= 0b0000_0100;
            else
                reg &= 0b1111_1011;
        }
    }

    // Bit 3
    public bool RenderBackground
    {
        get => (reg & 0b0000_1000) != 0;
        set
        {
            if (value)
                reg |= 0b0000_1000;
            else
                reg &= 0b1111_0111;
        }
    }

    // Bit 4
    public bool RenderSprites
    {
        get => (reg & 0b0001_0000) != 0;
        set
        {
            if (value)
                reg |= 0b0001_0000;
            else
                reg &= 0b1110_1111;
        }
    }

    // Bit 5
    public bool EnhanceRed
    {
        get => (reg & 0b0010_0000) != 0;
        set
        {
            if (value)
                reg |= 0b0010_0000;
            else
                reg &= 0b1101_1111;
        }
    }

    // Bit 6
    public bool EnhanceGreen
    {
        get => (reg & 0b0100_0000) != 0;
        set
        {
            if (value)
                reg |= 0b0100_0000;
            else
                reg &= 0b1011_1111;
        }
    }

    // Bit 7
    public bool EnhanceBlue
    {
        get => (reg & 0b1000_0000) != 0;
        set
        {
            if (value)
                reg |= 0b1000_0000;
            else
                reg &= 0b0111_1111;
        }
    }
}
