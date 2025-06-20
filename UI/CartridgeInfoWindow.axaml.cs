using Avalonia.Controls;
using Devices.Cartridge;

namespace UI;

public partial class CartridgeInfoWindow : Window
{
    public CartridgeInfoWindow()
    {
        InitializeComponent();
    }

    public void UpdateCartridgeInfo(CartridgeInfo? cartridgeInfo)
    {
        if (cartridgeInfo == null)
        {
            NoCartridgeTextBlock.IsVisible = true;
            CartridgeInfo.IsVisible = false;
        }
        else
        {
            MapperName.Text = cartridgeInfo.MapperName;
            MapperId.Text = cartridgeInfo.MapperId.ToString();
            PrgBanks.Text = cartridgeInfo.PrgBanks.ToString();
            ChrBanks.Text = cartridgeInfo.ChrBanks.ToString();
            MirrorMode.Text = cartridgeInfo.MirrorMode.ToString();
            HasTrainer.Text = cartridgeInfo.HasTrainer ? "Yes" : "No";
            IsValid.Text = cartridgeInfo.IsValid ? "Yes" : "No";
            TvSystem.Text = cartridgeInfo.TvSystem;
            FileFormat.Text = cartridgeInfo.FileFormat;
        }
    }
}