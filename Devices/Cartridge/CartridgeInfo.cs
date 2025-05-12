using Devices.PPU;

namespace Devices.Cartridge;

public class CartridgeInfo
{
    public string FileName { get; set; } = string.Empty;
    public string MapperName { get; set; } = string.Empty;
    public byte MapperId { get; set; }
    public byte PrgBanks { get; set; }
    public byte ChrBanks { get; set; }
    public Mirror MirrorMode { get; set; }
    public bool HasTrainer { get; set; }
    public bool IsValid { get; set; }
    public string TvSystem { get; set; } = string.Empty;
    public string FileFormat { get; set; } = string.Empty;
} 