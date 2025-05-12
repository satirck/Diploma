using System.Runtime.InteropServices;
using Devices.Mapper.Impl;
using Devices.PPU;

namespace Devices.Cartridge;

using Mapper;
using static Mirror;

public class Cartridge
{
    private List<byte> _vPrgMemory = new();
    private List<byte> _vChrMemory = new();

    private byte _nMapperId = 0;
    private byte _nPrgBanks = 0;
    private byte _nChrBanks = 0;
    private byte _nFileType = 1;
               
    private Mapper? _mapper;
    
    private bool _bImageValid = false;

    private Mirror _mirror;
    
    private Header _header = new();
    
    public Cartridge(String cartridgePath)
    {
        using (var fs = new FileStream(cartridgePath, FileMode.Open, FileAccess.Read))
        using (var reader = new BinaryReader(fs))
        {
            _header.Name = reader.ReadBytes(4);
            _header.PrgRomChunks = reader.ReadByte();
            _header.ChrRomChunks = reader.ReadByte();
            _header.Mapper1 = reader.ReadByte();
            _header.Mapper2 = reader.ReadByte();
            _header.PrgRamSize = reader.ReadByte();
            _header.TvSystem1 = reader.ReadByte();
            _header.TvSystem2 = reader.ReadByte();
            _header.Unused = reader.ReadBytes(5);
            
            if ((_header.Mapper1 & 0x04) != 0)
                reader.BaseStream.Seek(512, SeekOrigin.Current);
            
            _nMapperId = (byte)(((_header.Mapper2 >> 4) << 4) | (_header.Mapper1 >> 4));
            _mirror = (_header.Mapper1 & 0x01) != 0 ? Vertical : Horizontal;
 
            if ((_header.Mapper2 & 0x0C) == 0x08) _nFileType = 2;

            if (_nFileType == 0)
            {}
            else if (_nFileType == 1)
            {
                _nPrgBanks = _header.PrgRomChunks;
                _vPrgMemory = new List<byte>(_nPrgBanks * 16384);
                _vPrgMemory.AddRange(reader.ReadBytes(_vPrgMemory.Capacity));

                _nChrBanks = _header.ChrRomChunks;
                if (_nChrBanks == 0) _vChrMemory = new List<byte>(8192);
                else _vChrMemory.AddRange(reader.ReadBytes(_nChrBanks * 8192));
                _vChrMemory.AddRange(reader.ReadBytes(_vChrMemory.Capacity));
            }
            else if (_nFileType == 2) 
            {
                _nPrgBanks = (byte)(((_header.PrgRamSize & 0x07) << 8) | _header.PrgRomChunks);
                _vPrgMemory = new List<byte>(_nPrgBanks * 16384);
                _vPrgMemory.AddRange(reader.ReadBytes(_vPrgMemory.Capacity));

                _nChrBanks = (byte)(((_header.PrgRamSize & 0x38) << 8) | _header.ChrRomChunks);
                _vChrMemory = new List<byte>(_nChrBanks * 8192);
                _vChrMemory.AddRange(reader.ReadBytes(_vChrMemory.Capacity));
            }
        }
        
        switch (_nMapperId)
        {
            case 0: _mapper = new Mapper000(_nPrgBanks, _nChrBanks); break;
            case 1: _mapper = new Mapper001(_nPrgBanks, _nChrBanks); break;
            case 2: _mapper = new Mapper002(_nPrgBanks, _nChrBanks); break;
            case 3: _mapper = new Mapper003(_nPrgBanks, _nChrBanks); break;
            case 4: _mapper = new Mapper004(_nPrgBanks, _nChrBanks); break;
            case 66: _mapper = new Mapper066(_nPrgBanks, _nChrBanks); break;
        }

        _bImageValid = true;
    }

    public bool ImageValid()
    {
        return _bImageValid;
    }
    
    public bool CpuRead(ushort addr, ref byte data, bool bReadOnly = false)
    {
        uint mappedAddr = 0;
        if (_mapper != null && _mapper.CpuMapRead(addr, ref mappedAddr))
        {
            data = _vPrgMemory[(int)mappedAddr];    
            return true;
        }
        
        return false;
    }
    
    public bool CpuWrite(ushort addr, byte data)
    {
        uint mappedAddr = 0;
        if (_mapper != null && _mapper.CpuMapWrite(addr, ref mappedAddr))
        {
            _vPrgMemory[(int)mappedAddr] = data;    
            return true;
        }
        
        return false;
    }

    public bool PpuRead(ushort addr, ref byte data, bool bReadOnly = false)
    {
        uint mappedAddr = 0;
        if (_mapper != null && _mapper.PpuMapRead(addr, ref mappedAddr))
        {
            data = _vChrMemory[(int)mappedAddr];    
            return true;
        }
        
        return false;
    }
    
    public bool PpuWrite(ushort addr, byte data)
    {
        uint mappedAddr = 0;
        if (_mapper != null && _mapper.PpuMapWrite(addr, ref mappedAddr))
        {
            _vChrMemory[(int)mappedAddr] = data;    
            return true;
        }
        
        return false;
    }

    public void Reset()
    {
        if (_mapper != null)
        {
            _mapper.Reset();
        }
    }

    public Mirror GetMirror()
    {
        if (_mapper != null)
        {
            return _mapper.Mirror();
        }
        return _mirror;
    }

    public Mapper? GetMapper()
    {
        return _mapper;
    }

    public CartridgeInfo GetInfo()
    {
        var info = new CartridgeInfo
        {
            MapperId = _nMapperId,
            PrgBanks = _nPrgBanks,
            ChrBanks = _nChrBanks,
            MirrorMode = GetMirror(),
            HasTrainer = (_header.Mapper1 & 0x04) != 0,
            IsValid = _bImageValid,
            FileFormat = _nFileType == 1 ? "iNES" : "NES 2.0",
        };
        
        info.MapperName = _nMapperId switch
        {
            0 => "NROM",
            1 => "MMC1",
            2 => "UxROM",
            3 => "CNROM",
            4 => "MMC3",
            66 => "GxROM",
            _ => $"Unknown ({_nMapperId})"
        };

        // Определяем TV систему
        if ((_header.TvSystem1 & 0x01) != 0)
            info.TvSystem = "PAL";
        else if ((_header.TvSystem2 & 0x01) != 0)
            info.TvSystem = "Dendy";
        else
            info.TvSystem = "NTSC";

        return info;
    }

    public byte[] GetPatternTable0()
    {
        return _vChrMemory.Take(0x1000).ToArray();
    }

    public byte[] GetPatternTable1()
    {
        return _vChrMemory.Skip(0x1000).Take(0x1000).ToArray();
    }

    private struct Header
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] Name;
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