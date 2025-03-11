using System.Runtime.InteropServices;

namespace Devices.Cartridge;

public class Cartridge
{
    private List<byte> _vPrgMemory = new List<byte>();
    private List<byte> _vChrMemory = new List<byte>();

    private byte _nMapperId = 0;
    private byte _nPrgBanks = 0;
    private byte _nChrBanks = 0;

    public Cartridge(String cartridgePath)
    {
        
    }
    
    public void CpuWrite(ushort addr, byte data)
    {
        
    }

    public byte CpuRead(ushort addr, bool bReadOnly = false)
    {
        byte data = 0x00;
        
        return data;
    }

    public void PpuWrite(ushort addr, byte data)
    {
        
    }

    public byte PpuRead(ushort addr, bool bReadOnly = false)
    {
        return 0x00;
    }
    
    protected struct Header
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public char[] name;

        public byte prg_rom_chunks;
        public byte chr_rom_chunks;
        public byte mapper1;
        public byte mapper2;
        public byte prg_ram_size;
        public byte tv_system1;
        public byte tv_system2;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public byte[] unused;       
    }
}
