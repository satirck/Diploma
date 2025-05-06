namespace Devices.PPU.PpuRegisters;

public struct Status
{
    public byte Reg; 
    
    public byte Unused
    {
        get => (byte)((Reg >> 3) & 0x1F); 
        set => Reg = (byte)((Reg & 0x07) | ((value & 0x1F) << 3)); 
    }

    public bool SpriteOverflow
    {
        get => (Reg & 0x04) != 0; 
        set => Reg = (byte)((Reg & 0xFB) | (value ? 0x04 : 0x00));
    }

    public bool SpriteZeroHit
    {
        get => (Reg & 0x02) != 0;
        set => Reg = (byte)((Reg & 0xFD) | (value ? 0x02 : 0x00));
    }

    public bool VerticalBlank
    {
        get => (Reg & 0x01) != 0;
        set => Reg = (byte)((Reg & 0xFE) | (value ? 0x01 : 0x00));
    }
}
