namespace Devices.PPU;

public enum PpuAddrStates: ushort
{
    Control = 0x0000,
    Mask = 0x0001,
    Status = 0x0002,
    OamAddress = 0x0003,
    OamData = 0x0004,
    Scroll = 0x0005,
    PpuAddress = 0x0006,
    PpuData = 0x0007,
}