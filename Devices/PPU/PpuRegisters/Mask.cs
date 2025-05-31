using System.Runtime.InteropServices;

namespace Devices.PPU.PpuRegisters;

public struct Mask
{
    private byte _reg;

    public byte reg
    {
        get => _reg;
        set => _reg = value;
    }

    // bit 0: grayscale
    public bool grayscale
    {
        get => (_reg & (1 << 0)) != 0;
        set => _reg = (byte)(value ? (_reg | (1 << 0)) : (_reg & ~(1 << 0)));
    }

    // bit 1: render background left
    public bool renderBackgroundLeft
    {
        get => (_reg & (1 << 1)) != 0;
        set => _reg = (byte)(value ? (_reg | (1 << 1)) : (_reg & ~(1 << 1)));
    }

    // bit 2: render sprites left
    public bool renderSpritesLeft
    {
        get => (_reg & (1 << 2)) != 0;
        set => _reg = (byte)(value ? (_reg | (1 << 2)) : (_reg & ~(1 << 2)));
    }

    // bit 3: render background
    public bool renderBackground
    {
        get => (_reg & (1 << 3)) != 0;
        set => _reg = (byte)(value ? (_reg | (1 << 3)) : (_reg & ~(1 << 3)));
    }

    // bit 4: render sprites
    public bool renderSprites
    {
        get => (_reg & (1 << 4)) != 0;
        set => _reg = (byte)(value ? (_reg | (1 << 4)) : (_reg & ~(1 << 4)));
    }

    // bit 5: enhance red
    public bool enhanceRed
    {
        get => (_reg & (1 << 5)) != 0;
        set => _reg = (byte)(value ? (_reg | (1 << 5)) : (_reg & ~(1 << 5)));
    }

    // bit 6: enhance green
    public bool enhanceGreen
    {
        get => (_reg & (1 << 6)) != 0;
        set => _reg = (byte)(value ? (_reg | (1 << 6)) : (_reg & ~(1 << 6)));
    }

    // bit 7: enhance blue
    public bool enhanceBlue
    {
        get => (_reg & (1 << 7)) != 0;
        set => _reg = (byte)(value ? (_reg | (1 << 7)) : (_reg & ~(1 << 7)));
    }
}