using System.Runtime.InteropServices;

namespace Devices.PPU.PpuRegisters;

public struct LoopyRegister
{
    private ushort _reg;

    public ushort reg
    {
        get => _reg;
        set => _reg = value;
    }

    // bits 0-4: coarse X
    public byte coarseX
    {
        get => (byte)(_reg & 0x1F);
        set => _reg = (ushort)((_reg & ~0x1F) | (value & 0x1F));
    }

    // bits 5-9: coarse Y
    public byte coarseY
    {
        get => (byte)((_reg >> 5) & 0x1F);
        set => _reg = (ushort)((_reg & ~(0x1F << 5)) | ((value & 0x1F) << 5));
    }

    // bit 10: nametable X
    public bool nametableX
    {
        get => (_reg & (1 << 10)) != 0;
        set => _reg = (ushort)(value ? (_reg | (1 << 10)) : (_reg & ~(1 << 10)));
    }

    // bit 11: nametable Y
    public bool nametableY
    {
        get => (_reg & (1 << 11)) != 0;
        set => _reg = (ushort)(value ? (_reg | (1 << 11)) : (_reg & ~(1 << 11)));
    }

    // bits 12-14: fine Y
    public byte fineY
    {
        get => (byte)((_reg >> 12) & 0x07);
        set => _reg = (ushort)((_reg & ~(0x07 << 12)) | ((value & 0x07) << 12));
    }

    // bit 15: unused
    public bool unused
    {
        get => (_reg & (1 << 15)) != 0;
        set => _reg = (ushort)(value ? (_reg | (1 << 15)) : (_reg & ~(1 << 15)));
    }

    public bool GetNametableX() => nametableX;
    public void SetNametableX(bool value) => nametableX = value;
    public bool GetNametableY() => nametableY;
    public void SetNametableY(bool value) => nametableY = value;

    public ushort GetNametableXShift() => (ushort)((nametableX ? 1 : 0) << 10);
    public ushort GetNametableYShift() => (ushort)((nametableY ? 1 : 0) << 11);
}
