using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using Devices.Bus.v1;
using Devices.Cartridge;
using Devices.CPU;
using Devices.PPU;

namespace UI;

public partial class MainWindow : Window
{
    private TextBlock _cpuInfoTextBlock;
    private StackPanel _flagsPanel;

    private Image _screenImage;
    private WriteableBitmap _bitmap;

    private Image _patternTable0Image;
    private Image _patternTable1Image;

    private Timer _updateTimer;
    private Bus _nes;
    private Cartridge _cart;

    private string _path = "/home/miki/work/NesRoms/" + "nestest.nes";
    private bool _isEmuRunning = false;

    private byte _selectedPallet = 0;
    private Dictionary<ushort, string> _asmMap;

    public MainWindow()
    {
        InitializeComponent();
        DisassemblyTextBlock = this.FindControl<TextBlock>("DisassemblyTextBlock");
        KeyDown += OnKeyDown;

        MinWidth = 600;
        MinHeight = 600;

        _nes = new Bus();
        _nes.Ppu = new Ppu2C02();

        _cart = new Cartridge(_path);
        _nes.InsertCartridge(_cart);

        _nes.Cpu.Reset();

        _screenImage = ScreenImage;
        _cpuInfoTextBlock = CpuInfoTextBlock;
        _flagsPanel = FlagsPanel;
        _patternTable0Image = PatternTable0Image;
        _patternTable1Image = PatternTable1Image;

        _updateTimer = new Timer(16);
        _updateTimer.Elapsed += (_, _) => Dispatcher.UIThread.Post(UpdateCpuInfo);
        _updateTimer.Start();
    }

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
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
        }

        if (_isEmuRunning)
        {
            return;
        }

        if (e.Key == Key.C)
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

        if (e.Key == Key.F)
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

    private void RenderSpriteToImage(Sprite sprite)
    {
        var width = 256;
        var height = 240;

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

        using (var fb = _bitmap.Lock())
        {
            unsafe
            {
                uint* buffer = (uint*)fb.Address;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        var color = sprite.PColData[y * width + x];

                        buffer[y * width + x] = color.ToUInt32();
                    }
                }
            }
        }

        Dispatcher.UIThread.Post(() => { _screenImage.Source = _bitmap; });
    }

    private void RenderPatternTable(Sprite sprite, Image imageControl)
    {
        int width = 128;
        int height = 128;

        var bitmap = new WriteableBitmap(
            new PixelSize(width, height),
            new Vector(96, 96),
            Avalonia.Platform.PixelFormat.Bgra8888,
            Avalonia.Platform.AlphaFormat.Unpremul);

        using (var fb = bitmap.Lock())
        {
            unsafe
            {
                uint* buffer = (uint*)fb.Address;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        var color = sprite.PColData[y * width + x];

                        buffer[y * width + x] = color.ToUInt32();
                    }
                }
            }
        }

        Dispatcher.UIThread.Post(() => { imageControl.Source = bitmap; });
    }

    private Dictionary<ushort, string> DisassembleAround(ushort pc, int before = 5, int after = 5)
    {
        var map = new Dictionary<ushort, string>();

        // Дизассемблируем назад: максимум 20 байт до pc
        ushort scanStart = (ushort)(pc > 0x20 ? pc - 0x20 : 0x0000);
        var tempMap = _nes.Cpu.Disassemble(scanStart, (ushort)(pc + 0x20));

        // Собираем ключи в список и ищем точное положение PC
        var keys = new List<ushort>(tempMap.Keys);
        keys.Sort();

        int pcIndex = keys.FindIndex(k => k == pc);
        if (pcIndex == -1)
            pcIndex = keys.FindLastIndex(k => k < pc); // ищем ближайшую предыдущую

        // Гарантируем границы
        int startIndex = Math.Max(pcIndex - before, 0);
        int endIndex = Math.Min(pcIndex + after, keys.Count - 1);

        // Только нужные строки
        for (int i = startIndex; i <= endIndex; i++)
        {
            map[keys[i]] = tempMap[keys[i]];
        }

        return map;
    }

    private void UpdateCpuInfo()
    {
        if (_isEmuRunning)
        {
            do
            {
                _nes.Clock();
            } while (!_nes.Ppu.FrameComplete);

            _nes.Ppu.FrameComplete = false;
        }

        var cpu = _nes.Cpu;
        var status = cpu.Status;

        byte testResultLow = _nes.CpuRead(0x0002);
        byte testResultHigh = _nes.CpuRead(0x0003);
        ushort testResult = (ushort)(testResultLow | (testResultHigh << 8));

        // Обновляем текст CPU
        var sb = new StringBuilder();
        sb.AppendLine($"A  = 0x{cpu.A:X2}");
        sb.AppendLine($"X  = 0x{cpu.X:X2}");
        sb.AppendLine($"Y  = 0x{cpu.Y:X2}");
        sb.AppendLine($"Sp = 0x{cpu.Stkp:X2}");
        sb.AppendLine($"Pc = 0x{cpu.Pc:X4}");
        sb.AppendLine($"Opcode = {cpu.Lookup[cpu.Opcode].Name}");
        sb.AppendLine($"Test Result: 0x{testResult:X4}");

        var flagNames = new[] { "N", "V", "U", "B", "D", "I", "Z", "C" };
        var flagValues = new[]
        {
            Flags6502.N, Flags6502.V, Flags6502.U, Flags6502.B,
            Flags6502.D, Flags6502.I, Flags6502.Z, Flags6502.C
        };

        _cpuInfoTextBlock.Text = sb.ToString();

        // Обновляем панель флагов с цветным отображением
        _flagsPanel.Children.Clear();

        var flagsRow = new StackPanel { Orientation = Orientation.Horizontal };
        var valuesRow = new StackPanel { Orientation = Orientation.Horizontal };

        for (int i = 0; i < flagNames.Length; i++)
        {
            bool isSet = (status & (byte)flagValues[i]) != 0;
            var color = isSet ? Brushes.Green : Brushes.Red;

            var flagText = new TextBlock
            {
                Text = flagNames[i],
                Foreground = color,
                FontWeight = FontWeight.Bold,
                Margin = new Thickness(4, 0)
            };

            var valueText = new TextBlock
            {
                Text = isSet ? "1" : "0",
                Foreground = color,
                FontWeight = FontWeight.Bold,
                Margin = new Thickness(4, 0)
            };

            flagsRow.Children.Add(flagText);
            valuesRow.Children.Add(valueText);
        }

        _flagsPanel.Children.Add(flagsRow);
        _flagsPanel.Children.Add(valuesRow);

        var screen = _nes.Ppu.GetScreen();
        RenderSpriteToImage(screen);

        RenderPatternTable(_nes.Ppu.GetPatternTable(0, _selectedPallet), _patternTable0Image);
        RenderPatternTable(_nes.Ppu.GetPatternTable(1, _selectedPallet), _patternTable1Image);
        
        _asmMap = DisassembleAround(_nes.Cpu.Pc, 10, 10);

        ushort pc = _nes.Cpu.Pc;

        var disasmSb = new StringBuilder();

        foreach (var line in _asmMap)
        {
            if (line.Key == pc)
                disasmSb.AppendLine($"> {line.Value}");
            else
                disasmSb.AppendLine($"  {line.Value}");
        }

        DisassemblyTextBlock.Text = disasmSb.ToString();
    }
}