using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using Avalonia.Platform.Storage;
using Devices.Bus.v1;
using Devices.Cartridge;
using Devices.CPU;
using Devices.PPU;

namespace UI;

public partial class MainWindow : Window
{
    private TextBlock _cpuInfoTextBlock;
    private StackPanel _flagsNamesRow;
    private StackPanel _flagsValuesRow;
    private List<TextBlock> _flagValueBlocks = new();

    private Image _screenImage;
    private WriteableBitmap? _bitmap;

    private Image _patternTable0Image;
    private Image _patternTable1Image;

    private Timer _updateTimer;
    private Bus? _nes;
    private Cartridge? _cart;

    private bool _isEmuRunning;
    private byte _selectedPallet = 1;
    private Dictionary<ushort, string> _asmMap = new();

    public MainWindow()
    {
        InitializeComponent();
        DisassemblyTextBlock = this.FindControl<TextBlock>("DisassemblyTextBlock");
        KeyDown += OnKeyDown;

        MinWidth = 600;
        MinHeight = 600;

        _screenImage = ScreenImage;
        _cpuInfoTextBlock = CpuInfoTextBlock;
        _patternTable0Image = PatternTable0Image;
        _patternTable1Image = PatternTable1Image;

        _flagsNamesRow = this.FindControl<StackPanel>("FlagsNamesRow")!;
        _flagsValuesRow = this.FindControl<StackPanel>("FlagsValuesRow")!;

        InitFlagUi();

        _updateTimer = new Timer(16);
        _updateTimer.Elapsed += (_, _) => Dispatcher.UIThread.Post(UpdateCpuInfo);
        _updateTimer.Start();
    }

    private void InitFlagUi()
    {
        var flagNames = new[] { "N", "V", "U", "B", "D", "I", "Z", "C" };
        _flagValueBlocks = new List<TextBlock>();

        foreach (var name in flagNames)
        {
            _flagsNamesRow.Children.Add(new TextBlock
            {
                Text = name,
                Margin = new Thickness(4, 0),
                FontWeight = FontWeight.Bold,
            });

            var valBlock = new TextBlock
            {
                Text = "0",
                Margin = new Thickness(4, 0),
                FontWeight = FontWeight.Bold,
            };

            _flagValueBlocks.Add(valBlock);
            _flagsValuesRow.Children.Add(valBlock);
        }
    }

    private void UpdateFlagUi(byte status)
    {
        var flagValues = new[]
        {
            Flags6502.N, Flags6502.V, Flags6502.U, Flags6502.B,
            Flags6502.D, Flags6502.I, Flags6502.Z, Flags6502.C
        };

        for (int i = 0; i < flagValues.Length; i++)
        {
            bool isSet = (status & (byte)flagValues[i]) != 0;
            _flagValueBlocks[i].Text = isSet ? "1" : "0";
            _flagValueBlocks[i].Foreground = isSet ? Brushes.Green : Brushes.Red;
        }
    }

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (_nes == null) return;

        if (e.Key == Key.R)
        {
            _nes.Cpu.Reset();
            _isEmuRunning = false;
        }

        if (e.Key == Key.P)
        {
            _isEmuRunning = !_isEmuRunning;
        }

        if (e.Key == Key.N)
        {
            ++_selectedPallet;
            _selectedPallet &= 0x07;

            UpdatePatternTable();
        }

        if (_isEmuRunning) return;

        if (e.Key == Key.C)
        {
            do { _nes.Clock(); } while (!_nes.Cpu.Complete());
            do { _nes.Clock(); } while (_nes.Cpu.Complete());
        }

        if (e.Key == Key.F)
        {
            do { _nes.Clock(); } while (!_nes.Ppu.FrameComplete);
            do { _nes.Clock(); } while (!_nes.Cpu.Complete());
            _nes.Ppu.FrameComplete = false;
        }
    }

    private void UpdatePatternTable()
    {
        if (_nes == null) return;
        RenderPatternTable(_nes.Ppu.GetPatternTable(0, _selectedPallet), _patternTable0Image);
        RenderPatternTable(_nes.Ppu.GetPatternTable(1, _selectedPallet), _patternTable1Image);
    }

    private void RenderSpriteToImage(Sprite sprite)
    {
        const int width = 256;
        const int height = 240;

        if (sprite.PColData.Count != width * height)
        {
            Console.WriteLine($"[ERROR] PColData has unexpected size: {sprite.PColData.Count}");
            return;
        }

        _bitmap = new WriteableBitmap(
            new PixelSize(width, height),
            new Vector(96, 96),
            Avalonia.Platform.PixelFormat.Bgra8888,
            Avalonia.Platform.AlphaFormat.Unpremul);

        using var fb = _bitmap.Lock();
        unsafe
        {
            uint* buffer = (uint*)fb.Address;
            for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                buffer[y * width + x] = sprite.PColData[y * width + x].ToBgra8888();
        }

        Dispatcher.UIThread.Post(() => { _screenImage.Source = _bitmap; });
    }

    private void RenderPatternTable(Sprite sprite, Image imageControl)
    {
        int width = 128, height = 128;

        var bitmap = new WriteableBitmap(
            new PixelSize(width, height),
            new Vector(96, 96),
            Avalonia.Platform.PixelFormat.Bgra8888,
            Avalonia.Platform.AlphaFormat.Unpremul);

        using var fb = bitmap.Lock();
        unsafe
        {
            uint* buffer = (uint*)fb.Address;
            for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                buffer[y * width + x] = sprite.PColData[y * width + x].ToBgra8888();
        }

        Dispatcher.UIThread.Post(() => { imageControl.Source = bitmap; });
    }

    private Dictionary<ushort, string> DisassembleAround(ushort pc, int before = 5, int after = 5)
    {
        var map = new Dictionary<ushort, string>();
        ushort scanStart = (ushort)(pc > 0x20 ? pc - 0x20 : 0x0000);
        var tempMap = _nes!.Cpu.Disassemble(scanStart, (ushort)(pc + 0x20));
        var keys = new List<ushort>(tempMap.Keys);
        keys.Sort();

        int pcIndex = keys.FindIndex(k => k == pc);
        if (pcIndex == -1)
            pcIndex = keys.FindLastIndex(k => k < pc);

        int startIndex = Math.Max(pcIndex - before, 0);
        int endIndex = Math.Min(pcIndex + after, keys.Count - 1);

        for (int i = startIndex; i <= endIndex; i++)
            map[keys[i]] = tempMap[keys[i]];

        return map;
    }

    private void UpdateCpuInfo()
    {
        if (_nes == null) return;

        if (_isEmuRunning)
        {
            do { _nes.Clock(); } while (!_nes.Ppu.FrameComplete);
            _nes.Ppu.FrameComplete = false;
        }

        var cpu = _nes.Cpu;
        var sb = new StringBuilder();

        ushort testResult = (ushort)(_nes.CpuRead(0x0002) | (_nes.CpuRead(0x0003) << 8));

        sb.AppendLine($"A  = 0x{cpu.A:X2}");
        sb.AppendLine($"X  = 0x{cpu.X:X2}");
        sb.AppendLine($"Y  = 0x{cpu.Y:X2}");
        sb.AppendLine($"Sp = 0x{cpu.Stkp:X2}");
        sb.AppendLine($"Pc = 0x{cpu.Pc:X4}");
        sb.AppendLine($"Opcode = {cpu.Lookup[cpu.Opcode].Name}");
        sb.AppendLine($"Test Result: 0x{testResult:X4}");

        _cpuInfoTextBlock.Text = sb.ToString();
        UpdateFlagUi(cpu.Status);

        RenderSpriteToImage(_nes.Ppu.GetScreen());

        _asmMap = DisassembleAround(cpu.Pc, 5, 5);

        var disasmSb = new StringBuilder();
        foreach (var line in _asmMap)
            disasmSb.AppendLine(line.Key == cpu.Pc ? $"> {line.Value}" : $"  {line.Value}");

        DisassemblyTextBlock.Text = disasmSb.ToString();
    }

    private async void OnOpenFileClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open NES ROM",
            AllowMultiple = false,
            FileTypeFilter = new[] 
            { 
                new FilePickerFileType("NES ROMs") 
                { 
                    Patterns = new[] { "*.nes" } 
                } 
            }
        });

        if (files.Count > 0 && files[0] is { } file)
        {
            var path = file.Path.LocalPath;
            try
            {
                _cart = new Cartridge(path);
                _nes = new Bus();
                _nes.InsertCartridge(_cart);
                _isEmuRunning = false;
                _nes.Cpu.Reset();
                UpdatePatternTable();
            }
            catch (Exception ex)
            {
                var dialog = new Window
                {
                    Title = "Error",
                    Content = new TextBlock
                    {
                        Text = $"Failed to load ROM: {ex.Message}",
                        Margin = new Thickness(20),
                        TextWrapping = TextWrapping.Wrap
                    },
                    SizeToContent = SizeToContent.WidthAndHeight,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };
                await dialog.ShowDialog(this);
            }
        }
    }
}
