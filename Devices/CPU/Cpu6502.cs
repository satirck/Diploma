namespace Devices.CPU;

using Bus;

public partial class Cpu6502
{
    private IBus? _bus;

    public readonly List<Instruction> Lookup = null!;

    public byte A { get; private set; } = 0x00;
    public byte X { get; private set; } = 0x00;
    public byte Y { get; private set; } = 0x00;
    public byte Stkp { get; private set; } = 0x00;
    public ushort Pc { get; set; } = 0x0000;
    public byte Status { get; private set; } = 0x00;
    public byte Fetched { get; private set; } = 0x00;
    public ushort Temp { get; private set; } = 0x0000;
    public ushort AddrAbs { get; private set; } = 0x0000;
    public ushort AddRel { get; private set; } = 0x0000;
    public byte Opcode { get; private set; } = 0x00;
    public byte Cycles { get; private set; } = 0;

    public void ConnectBus(IBus bus)
    {
        _bus = bus;
    }

    public bool Complete()
    {
        return Cycles == 0;
    }

    public byte Fetch()
    {
        if (Lookup[Opcode].AddrMode != Imp)
        {
            Fetched = Read(AddrAbs);
        }

        return Fetched;
    }

    private void Write(ushort addr, byte value)
    {
        _bus!.CpuWrite(addr, value);
    }

    private byte Read(ushort addr)
    {
        return _bus!.CpuRead(addr);
    }

    private byte GetFlag(Flags6502 f)
    {
        return (byte)(((Status & (byte)f) > 0) ? 1 : 0);
    }

    private void SetFlag(Flags6502 f, bool v)
    {
        if (v)
        {
            Status = (byte)(Status | (byte)f);
        }
        else
        {
            Status = (byte)(Status & ~(byte)f);
        }
    }
}