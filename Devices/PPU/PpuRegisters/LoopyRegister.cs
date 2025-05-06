namespace Devices.PPU.PpuRegisters;

public struct LoopyRegister
{
    public ushort Reg;

    public byte CoarseX
    {
        get => (byte)(Reg & 0x1F);
        set => Reg = (ushort)((Reg & 0xFFE0) | (value & 0x1F));
    }

    public byte CoarseY
    {
        get => (byte)((Reg >> 5) & 0x1F);
        set => Reg = (ushort)((Reg & 0xFC1F) | ((value & 0x1F) << 5));
    }

    public bool NametableX
    {
        get => (Reg & 0x400) != 0;
        set => Reg = (ushort)((Reg & 0xFBFF) | (value ? 0x400 : 0x00));
    }

    public bool NametableY
    {
        get => (Reg & 0x800) != 0;
        set => Reg = (ushort)((Reg & 0xF7FF) | (value ? 0x800 : 0x00));
    }

    public byte FineY
    {
        get => (byte)((Reg >> 12) & 0x07);
        set => Reg = (ushort)((Reg & 0x8FFF) | ((value & 0x07) << 12));
    }
}
