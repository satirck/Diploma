using System;
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
    private Grid _mainGrid;
    private StackPanel _flagsPanel;

    private Image _screenImage;
    private WriteableBitmap _bitmap;

    private Image _patternTable0Image;
    private Image _patternTable1Image;

    private Timer _updateTimer;
    private Bus _nes;
    private Cartridge _cart;

    private string _path = "/home/miki/Downloads/nestest.nes";
    private bool _isEmuRunning = false;

    private byte _selectedPallet = 0;

    public MainWindow()
    {
        InitializeComponent();
        KeyDown += OnKeyDown;

        MinWidth = 600;
        MinHeight = 600;

        _nes = new Bus();
        _nes.Ppu = new Ppu2C02();

        _cart = new Cartridge(_path);
        _nes.InsertCartridge(_cart);

        _nes.Cpu.Reset();

        _mainGrid = this.FindControl<Grid>("MainGrid") ?? throw new Exception("Main grid cannot be created");
        SetupUi();

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

    private void SetupUi()
    {
        _mainGrid.RowDefinitions.Add(new RowDefinition(GridLength.Star));
        _mainGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
        _mainGrid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(2)));
        _mainGrid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(200)));

        // === ЭКРАН NES ===
        _screenImage = new Image
        {
            Width = 256,
            Height = 240,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Stretch = Stretch.None,
        };

        int width = 256;
        int height = 240;

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
                uint red = 0xFFFF0000;

                for (int i = 0; i < width * height; i++)
                    buffer[i] = red;
            }
        }

        _screenImage.Source = _bitmap;

        var imageContainer = new Grid
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };
        imageContainer.Children.Add(_screenImage);

        _mainGrid.Children.Add(imageContainer);
        Grid.SetColumn(imageContainer, 0);
        Grid.SetRow(imageContainer, 0);

        // === СЕПАРАТОР ===
        var separator = new Border
        {
            Background = Brushes.Gray,
            Width = 2,
            VerticalAlignment = VerticalAlignment.Stretch
        };

        _mainGrid.Children.Add(separator);
        Grid.SetColumn(separator, 1);
        Grid.SetRow(separator, 0);

        // === CPU INFO + Pattern Tables ===

        _cpuInfoTextBlock = new TextBlock
        {
            Text = "CPU Info",
            FontSize = 14,
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            TextWrapping = TextWrapping.Wrap,
            Margin = new Thickness(10)
        };

        _flagsPanel = new StackPanel
        {
            Orientation = Orientation.Vertical,
            Margin = new Thickness(10)
        };

        _patternTable0Image = new Image
        {
            Width = 128,
            Height = 128,
            Margin = new Thickness(5)
        };

        _patternTable1Image = new Image
        {
            Width = 128,
            Height = 128,
            Margin = new Thickness(5)
        };

        var patternTablePanel = new StackPanel
        {
            Orientation = Orientation.Vertical,
            Children = { _patternTable0Image, _patternTable1Image },
            Margin = new Thickness(10)
        };

        var cpuInfoPanel = new StackPanel
        {
            Children = { _cpuInfoTextBlock, _flagsPanel, patternTablePanel },
            Margin = new Thickness(10)
        };

        // Обернём всё в ScrollViewer, чтобы содержимое не обрезалось
        var scrollViewer = new ScrollViewer
        {
            Content = cpuInfoPanel,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto
        };

        _mainGrid.Children.Add(scrollViewer);
        Grid.SetColumn(scrollViewer, 2);
        Grid.SetRow(scrollViewer, 0);
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
                        byte r = (byte)color.X;
                        byte g = (byte)color.Y;
                        byte b = (byte)color.Z;
                        byte a = (byte)color.W;

                        buffer[y * width + x] = (uint)((a << 24) | (b << 16) | (g << 8) | r);
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
                        byte r = (byte)color.X;
                        byte g = (byte)color.Y;
                        byte b = (byte)color.Z;
                        byte a = (byte)color.W;

                        buffer[y * width + x] = (uint)((a << 24) | (b << 16) | (g << 8) | r);
                    }
                }
            }
        }

        Dispatcher.UIThread.Post(() => { imageControl.Source = bitmap; });
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
    }
}