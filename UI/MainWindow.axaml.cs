using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using Devices.Bus;
using Devices.Cartridge;
using Devices.CPU;
using Devices.PPU;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Timers;
using Devices.Bus;

namespace UI;

public partial class MainWindow : Window
{
    private Cartridge? _cart = null;
    private readonly Bus _nes = new();
    private readonly Timer _timer = new(1000d / 60d);

    private bool _isEmuRunning = false;
    private byte _selectedPalette = 0;
    private readonly HashSet<Key> _pressedKeys = [];

    public MainWindow()
    {
        InitializeComponent();
        
        var logFilePath = "log.txt";
        var fileStream = new FileStream(logFilePath, FileMode.Create, FileAccess.Write);
        var streamWriter = new StreamWriter(fileStream)
        {
            AutoFlush = true
        };
        Console.SetOut(streamWriter);
    }

    private void UpdateRegisters()
    {
        A.Text = $"0x{_nes.Cpu.A:X2}";
        X.Text = $"0x{_nes.Cpu.X:X2}";
        Y.Text = $"0x{_nes.Cpu.Y:X2}";
        Sp.Text = $"0x{_nes.Cpu.Stkp:X2}";
        Pc.Text = $"0x{_nes.Cpu.Pc:X4}";
        Opcode.Text = $"{_nes.Cpu.Lookup[_nes.Cpu.Opcode].Name}";
        TestResult.Text = $"0x{(ushort)(_nes.CpuRead(0x0002) | (_nes.CpuRead(0x0003) << 8)):X4}";
    }

    private void UpdateFlags()
    {
        N.Fill = (_nes.Cpu.Status & (byte)Flags6502.N) != 0 ? Brushes.Lime : Brushes.Red;
        V.Fill = (_nes.Cpu.Status & (byte)Flags6502.V) != 0 ? Brushes.Lime : Brushes.Red;
        U.Fill = (_nes.Cpu.Status & (byte)Flags6502.U) != 0 ? Brushes.Lime : Brushes.Red;
        B.Fill = (_nes.Cpu.Status & (byte)Flags6502.B) != 0 ? Brushes.Lime : Brushes.Red;
        D.Fill = (_nes.Cpu.Status & (byte)Flags6502.D) != 0 ? Brushes.Lime : Brushes.Red;
        I.Fill = (_nes.Cpu.Status & (byte)Flags6502.I) != 0 ? Brushes.Lime : Brushes.Red;
        Z.Fill = (_nes.Cpu.Status & (byte)Flags6502.Z) != 0 ? Brushes.Lime : Brushes.Red;
        C.Fill = (_nes.Cpu.Status & (byte)Flags6502.C) != 0 ? Brushes.Lime : Brushes.Red;
    }

    private void UpdatePatternTables()
    {
        RenderSpriteToImage(_nes.Ppu.GetPatternTable(0, _selectedPalette), PatternTable0Image);
        RenderSpriteToImage(_nes.Ppu.GetPatternTable(1, _selectedPalette), PatternTable1Image);
    }

    private void UpdatePalettes()
    {
        for (int i = 0; i < Palettes.Children.Count; i++)
        {
            StackPanel row = (StackPanel)Palettes.Children[i];

            for (int j = 0; j < row.Children.Count; j++)
            {
                Grid grid = (Grid)row.Children[j];
                StackPanel palette = (StackPanel)grid.Children[0];
                Border border = (Border)grid.Children[1];

                byte currentPalette = (byte)(i * row.Children.Count + j);
                border.IsVisible = currentPalette == _selectedPalette;

                for (int k = 0; k < palette.Children.Count; k++)
                {
                    Pixel color = _nes.Ppu.GetColorFromPaletteRam(currentPalette, (byte)k);
                    Rectangle rectangle = (Rectangle)palette.Children[k];
                    rectangle.Fill = new SolidColorBrush(color.ToBgra8888());
                }
            }
        }
    }

    private void UpdateAssembly()
    {
        Dictionary<ushort, string> asmMap = DisassembleAround(_nes.Cpu.Pc, 5, 5);

        StringBuilder asmSb = new();
        foreach (var line in asmMap)
            asmSb.AppendLine(line.Key == _nes.Cpu.Pc ? $"> {line.Value}" : $"  {line.Value}");

        AsmTextBlock.Text = asmSb.ToString().TrimEnd();
    }

    private void UpdateOam()
    {
        Span<byte> oamBytes = _nes.Ppu.OAMAsBytes;

        StringBuilder oamSb = new();
        for (int i = 0; i < 26; i++)
        {
            oamSb.Append($"{i:X2}: ({oamBytes[i * 4 + 3],3}, {oamBytes[i * 4 + 0],3}) ");
            oamSb.AppendLine($"ID: {oamBytes[i * 4 + 1]:X2} AT: {oamBytes[i * 4 + 2]:X2}");
        }

        OamTextBlock.Text = oamSb.ToString().TrimEnd();
    }

    private unsafe void RenderSpriteToImage(Sprite sprite, Image image)
    {
        WriteableBitmap bitmap = (WriteableBitmap)image.Source!;
        int width = bitmap.PixelSize.Width;
        int height = bitmap.PixelSize.Height;

        if (sprite.PColData.Count != width * height)
        {
            Console.WriteLine($"[ERROR] PColData has unexpected size: {sprite.PColData.Count}");
            return;
        }

        using var fb = bitmap.Lock();

        uint* buffer = (uint*)fb.Address;
        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                buffer[y * width + x] = sprite.PColData[y * width + x].ToBgra8888();

        image.InvalidateVisual();
    }

    private Dictionary<ushort, string> DisassembleAround(ushort pc, int before = 5, int after = 5)
    {
        ushort scanStart = (ushort)(pc > 0x20 ? pc - 0x20 : 0x0000);
        Dictionary<ushort, string> tempMap = _nes.Cpu.Disassemble(scanStart, (ushort)(pc + 0x20));
        List<ushort> keys = [.. tempMap.Keys]; keys.Sort();

        int pcIndex = keys.FindIndex(k => k == pc);
        if (pcIndex == -1)
            pcIndex = keys.FindLastIndex(k => k < pc);

        int startIndex = int.Max(pcIndex - before, 0);
        int endIndex = int.Min(pcIndex + after, keys.Count - 1);
        Dictionary<ushort, string> map = new(before + after + 1);

        for (int i = startIndex; i <= endIndex; i++)
            map[keys[i]] = tempMap[keys[i]];

        return map;
    }

    private byte PollController()
    {
        byte state = 0;

        if (_pressedKeys.Contains(Key.J)) state |= 0x80;        // A
        if (_pressedKeys.Contains(Key.K)) state |= 0x40;        // B
        if (_pressedKeys.Contains(Key.Space)) state |= 0x20;    // Select
        if (_pressedKeys.Contains(Key.Return)) state |= 0x10;   // Start

        if (_pressedKeys.Contains(Key.W)) state |= 0x08;        // Up
        if (_pressedKeys.Contains(Key.S)) state |= 0x04;        // Down
        if (_pressedKeys.Contains(Key.A)) state |= 0x02;        // Left
        if (_pressedKeys.Contains(Key.D)) state |= 0x01;        // Right

        return state;
    }

    private void UpdateCpuInfo()
    {
        if (_cart == null) return;

        _nes.Controller[0] = PollController();

        if (_isEmuRunning)
        {
            do
            {
                _nes.Clock();
            } while (!_nes.Ppu.FrameComplete);

            _nes.Ppu.FrameComplete = false;
        }

        RenderSpriteToImage(_nes.Ppu.GetScreen(), ScreenImage);
        UpdateRegisters();
        UpdateFlags();
        UpdatePatternTables();
        UpdatePalettes();
        UpdateAssembly();
        UpdateOam();
    }

    private async void FileOpen_Click(object? sender, RoutedEventArgs e)
    {
        IReadOnlyList<IStorageFile> files = await StorageProvider.OpenFilePickerAsync(new()
        {
            Title = "Open NES ROM",
            AllowMultiple = false,
            FileTypeFilter = [new("NES ROMs") { Patterns = ["*.nes"] }]
        });

        if (files.Count > 0)
        {
            try
            {
                _cart = new(files[0].Path.LocalPath);
                _nes.InsertCartridge(_cart);
                _isEmuRunning = true;
                _nes.Cpu.Reset();
            }
            catch (Exception ex)
            {
                ErrorWindow errorWindow = new();
                errorWindow.ErrorTextBlock.Text = ex.Message;
                await errorWindow.ShowDialog(this);
            }
        }
    }

    private async void CartridgeInfo_Click(object? sender, RoutedEventArgs e)
    {
        CartridgeInfoWindow cartridgeInfoWindow = new();
        cartridgeInfoWindow.UpdateCartridgeInfo(_cart?.GetInfo());
        await cartridgeInfoWindow.ShowDialog(this);
    }

    private void Window_Loaded(object? sender, RoutedEventArgs e)
    {
        ScreenImage.Source = new WriteableBitmap(new(256, 240), new(96, 96), PixelFormat.Bgra8888, AlphaFormat.Unpremul);
        PatternTable0Image.Source = new WriteableBitmap(new(128, 128), new(96, 96), PixelFormat.Bgra8888, AlphaFormat.Unpremul);
        PatternTable1Image.Source = new WriteableBitmap(new(128, 128), new(96, 96), PixelFormat.Bgra8888, AlphaFormat.Unpremul);

        _timer.Elapsed += (_, _) => Dispatcher.UIThread.Invoke(UpdateCpuInfo);
        _timer.Start();
    }

    private void Window_KeyDown(object? sender, KeyEventArgs e)
    {
        _pressedKeys.Add(e.Key);
        if (_cart == null) return;

        if (e.Key == Key.R)
        {
            _nes.Cpu.Reset();
        }
        else if (e.Key == Key.P)
        {
            _isEmuRunning = !_isEmuRunning;
        }
        else if (e.Key == Key.N)
        {
            ++_selectedPalette;
            _selectedPalette &= 0x07;
        }
        else if (e.Key == Key.C)
        {
            do
            {
                _nes.Clock();
            } while (!_nes.Cpu.Complete());

            do
            {
                _nes.Clock();
            } while (_nes.Cpu.Complete());
        }
        else if (e.Key == Key.F)
        {
            do
            {
                _nes.Clock();
            } while (!_nes.Ppu.FrameComplete);

            do
            {
                _nes.Clock();
            } while (!_nes.Cpu.Complete());

            _nes.Ppu.FrameComplete = false;
        }
    }

    private void Window_KeyUp(object? sender, KeyEventArgs e)
    {
        _pressedKeys.Remove(e.Key);
    }

    private void AsmButton_Click(object? sender, RoutedEventArgs e)
    {
        OamButton.IsChecked = false;
        AsmTextBlock.IsVisible = true;
        OamTextBlock.IsVisible = false;
    }

    private void OamButton_Click(object? sender, RoutedEventArgs e)
    {
        AsmButton.IsChecked = false;
        AsmTextBlock.IsVisible = false;
        OamTextBlock.IsVisible = true;
    }
}