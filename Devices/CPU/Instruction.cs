namespace Devices.CPU;

public class Instruction(string name, Func<byte> operate, Func<byte> addrMode, byte cycles)
{
    public string Name { get; set; } = name;
    public Func<byte> Operate { get; } = operate;
    public Func<byte> AddrMode { get; } = addrMode;
    public byte Cycles { get; } = cycles;
}