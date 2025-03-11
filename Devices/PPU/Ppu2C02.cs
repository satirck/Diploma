namespace Devices.PPU;

using Cartridge;

public class Ppu2C02
{
    private byte[][] _tblName = new byte[2][]
    {
        new byte[1024],
        new byte[1024]
    };
    
    // private byte[][] _tblPattern = new byte[2][]
    // {
    //     new byte[4096],
    //     new byte[4096]
    // };
    
    private byte[] _tblPalette = new byte[32];
    
    
    private Cartridge _cart;

    public void ConnectCart(Cartridge cartridge)
    {
        _cart = cartridge;
    }

    public void Clock()
    {
        
    }
    
    public byte CpuRead(ushort addr, bool bReadOnly = false)
    {
        byte data = 0x00;

        switch ((PpuAddrStates)addr)
        {
            case PpuAddrStates.Control:
                break;
            case PpuAddrStates.Mask:
                break;
            case PpuAddrStates.Status:
                break;
            case PpuAddrStates.OamAddress:
                break;
            case PpuAddrStates.OamData:
                break;
            case PpuAddrStates.Scroll:
                break;
            case PpuAddrStates.PpuAddress:
                break;
            case PpuAddrStates.PpuData:
                break;
        }

        return data;
    }
    
    public void CpuWrite(ushort addr, byte data)
    {
        switch ((PpuAddrStates)addr)
        {
            case PpuAddrStates.Control:
                break;
            case PpuAddrStates.Mask:
                break;
            case PpuAddrStates.Status:
                break;
            case PpuAddrStates.OamAddress:
                break;
            case PpuAddrStates.OamData:
                break;
            case PpuAddrStates.Scroll:
                break;
            case PpuAddrStates.PpuAddress:
                break;
            case PpuAddrStates.PpuData:
                break;
        }
    }

    public byte PpuRead(ushort addr, bool bReadOnly = false)
    {
        byte data = 0x00;
        addr &= 0x3FFF;
        
        return data;
    }
    
    public void PpuWrite(ushort addr, byte data)
    {
        addr &= 0x3FFF;
    }
}