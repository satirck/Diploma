using System.Runtime.InteropServices;
using Devices.Mapper.Impl;

namespace Devices.Cartridge;

using Mapper;

public class Cartridge
{
    private List<byte> _vPrgMemory = new();
    private List<byte> _vChrMemory = new();

    private byte _nMapperId = 0;
    private byte _nPrgBanks = 0;
    private byte _nChrBanks = 0;
    
    private Mapper _mapper;
    
    private bool _bImageValid = false;

    public Cartridge(String cartridgePath)
    {
        using (var fs = new FileStream(cartridgePath, FileMode.Open, FileAccess.Read))
        using (var reader = new BinaryReader(fs))
        {
            Header header = new Header();
            header.Name = reader.ReadChars(4);
            header.PrgRomChunks = reader.ReadByte();
            header.ChrRomChunks = reader.ReadByte();
            header.Mapper1 = reader.ReadByte();
            header.Mapper2 = reader.ReadByte();
            header.PrgRamSize = reader.ReadByte();
            header.TvSystem1 = reader.ReadByte();
            header.TvSystem2 = reader.ReadByte();
            header.Unused = reader.ReadBytes(5);
            
            if ((header.Mapper1 & 0x04) != 0)
                reader.BaseStream.Seek(512, SeekOrigin.Current);
            
            _nMapperId = (byte)(((header.Mapper2 >> 4) << 4) | (header.Mapper1 >> 4));
            
            byte nFileType = 1;

            if (nFileType == 0)
            {}
            else if (nFileType == 1)
            {
                _nPrgBanks = header.PrgRamSize;
                _vPrgMemory = new List<byte>(_nPrgBanks * 16384);
                _vPrgMemory.AddRange(reader.ReadBytes(_vPrgMemory.Capacity));

                _nChrBanks = header.ChrRomChunks;
                _vChrMemory = new List<byte>(_nChrBanks * 8192);
                _vChrMemory.AddRange(reader.ReadBytes(_vChrMemory.Capacity));
            }
            else if (nFileType == 2) 
            {}
        }
        
        switch (_nMapperId)
        {
            case 0: _mapper = new Mapper000(_nPrgBanks, _nChrBanks); break;
            default: throw new Exception($"Mapper with id=[{_nMapperId}] not implemented yet");
        }

        _bImageValid = true;
    }
    
    public bool CpuRead(ushort addr, ref byte data, bool bReadOnly = false)
    {
        uint mappedAddr = 0;
        if (_mapper.CpuMapRead(addr, ref mappedAddr))
        {
            data = _vPrgMemory[(int)mappedAddr];    
            return true;
        }
        
        return false;
    }
    
    public bool CpuWrite(ushort addr, byte data)
    {
        uint mappedAddr = 0;
        if (_mapper.CpuMapWrite(addr, ref mappedAddr))
        {
            _vPrgMemory[(int)mappedAddr] = data;    
            return true;
        }
        
        return false;
    }

    public bool PpuRead(ushort addr, ref byte data, bool bReadOnly = false)
    {
        uint mappedAddr = 0;
        if (_mapper.PpuMapRead(addr, ref mappedAddr))
        {
            data = _vChrMemory[(int)mappedAddr];    
            return true;
        }
        
        return false;
    }
    
    public bool PpuWrite(ushort addr, byte data)
    {
        uint mappedAddr = 0;
        if (_mapper.PpuMapWrite(addr, ref mappedAddr))
        {
            _vChrMemory[(int)mappedAddr] = data;    
            return true;
        }
        
        return false;
    }
    
    private struct Header
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public char[] Name;
        public byte PrgRomChunks;
        public byte ChrRomChunks;
        public byte Mapper1;
        public byte Mapper2;
        public byte PrgRamSize;
        public byte TvSystem1;
        public byte TvSystem2;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public byte[] Unused;       
    }
}
