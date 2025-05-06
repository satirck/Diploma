namespace Devices.PPU.PpuRegisters;

public struct Mask
{
    public byte Reg;

    public bool Grayscale
    {
        get => (Reg & 0x80) != 0;
        set => Reg = (byte)((Reg & 0x7F) | (value ? 0x80 : 0x00));
    }

    public bool RenderBackgroundLeft
    {
        get => (Reg & 0x40) != 0;
        set => Reg = (byte)((Reg & 0xBF) | (value ? 0x40 : 0x00));
    }

    public bool RenderSpritesLeft
    {
        get => (Reg & 0x20) != 0;
        set => Reg = (byte)((Reg & 0xDF) | (value ? 0x20 : 0x00));
    }

    public bool RenderBackground
    {
        get => (Reg & 0x10) != 0;
        set => Reg = (byte)((Reg & 0xEF) | (value ? 0x10 : 0x00));
    }

    public bool RenderSprites
    {
        get => (Reg & 0x08) != 0;
        set => Reg = (byte)((Reg & 0xF7) | (value ? 0x08 : 0x00)); 
    }

    public bool EnhanceRed
    {
        get => (Reg & 0x04) != 0; 
        set => Reg = (byte)((Reg & 0xFB) | (value ? 0x04 : 0x00));
    }

    public bool EnhanceGreen
    {
        get => (Reg & 0x02) != 0; 
        set => Reg = (byte)((Reg & 0xFD) | (value ? 0x02 : 0x00));
    }

    public bool EnhanceBlue
    {
        get => (Reg & 0x01) != 0; 
        set => Reg = (byte)((Reg & 0xFE) | (value ? 0x01 : 0x00));
    }
}
