using System.Runtime.InteropServices;

namespace Devices.PPU.PpuRegisters;

public struct PpuCtrl
{
    private byte _reg;

    public byte reg
    {
        get => _reg;
        set => _reg = value;
    }

    // bit 0: nametable X
    public bool nametableX
    {
        get => (_reg & (1 << 0)) != 0;
        set => _reg = (byte)(value ? (_reg | (1 << 0)) : (_reg & ~(1 << 0)));
    }

    // bit 1: nametable Y
    public bool nametableY
    {
        get => (_reg & (1 << 1)) != 0;
        set => _reg = (byte)(value ? (_reg | (1 << 1)) : (_reg & ~(1 << 1)));
    }

    // bit 2: increment mode
    public bool incrementMode
    {
        get => (_reg & (1 << 2)) != 0;
        set => _reg = (byte)(value ? (_reg | (1 << 2)) : (_reg & ~(1 << 2)));
    }

    // bit 3: pattern sprite
    public bool patternSprite
    {
        get => (_reg & (1 << 3)) != 0;
        set => _reg = (byte)(value ? (_reg | (1 << 3)) : (_reg & ~(1 << 3)));
    }

    // bit 4: pattern background
    public bool patternBackground
    {
        get => (_reg & (1 << 4)) != 0;
        set => _reg = (byte)(value ? (_reg | (1 << 4)) : (_reg & ~(1 << 4)));
    }

    // bit 5: sprite size
    public bool spriteSize
    {
        get => (_reg & (1 << 5)) != 0;
        set => _reg = (byte)(value ? (_reg | (1 << 5)) : (_reg & ~(1 << 5)));
    }

    // bit 6: slave mode (unused)
    public bool slaveMode
    {
        get => (_reg & (1 << 6)) != 0;
        set => _reg = (byte)(value ? (_reg | (1 << 6)) : (_reg & ~(1 << 6)));
    }

    // bit 7: enable NMI
    public bool enableNmi
    {
        get => (_reg & (1 << 7)) != 0;
        set => _reg = (byte)(value ? (_reg | (1 << 7)) : (_reg & ~(1 << 7)));
    }

    public void SetFlag(int bit, bool value)
    {
        if (value)
            _reg |= (byte)(1 << bit);
        else
            _reg &= (byte)~(1 << bit);
    }

    public bool GetFlag(int bit) => (_reg & (1 << bit)) != 0;
}