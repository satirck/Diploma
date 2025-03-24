using System;
using System.IO;
using System.Text;
using System.Timers;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using Devices.Bus;
using Devices.Bus.v1;
using Devices.CPU;

namespace UI;

public partial class MainWindow : Window
{
    private TextBlock _cpuInfoTextBlock;
    private Grid _mainGrid;
    private StackPanel _flagsPanel;

    private Timer _updateTimer;
    private bool _isPaused = true;
    private IBus _nes;
    
    public MainWindow()
    {
        InitializeComponent();
        KeyDown += OnKeyDown;

        MinWidth = 600;
        MinHeight = 600;

        _nes = new SimpleBus();
        LoadTestProgram();
        _nes.Cpu.Reset();
        _nes.Cpu.Pc = 0xC000;
        
        _mainGrid = this.FindControl<Grid>("MainGrid") ?? throw new Exception("Main grid cannot be created");
        SetupUi();

        _updateTimer = new Timer(10);
        _updateTimer.Elapsed += (_, _) => Dispatcher.UIThread.Post(UpdateCpuInfo);
        _updateTimer.Start();
    }

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Space && _isPaused)
        {
            _nes.Clock();
        }

        if (e.Key == Key.P)
        {
            _isPaused = !_isPaused;
        }
        
        if (e.Key == Key.R)
        {
            _nes.Cpu.Reset();
            _nes.Cpu.Pc = 0xC000;
        }
    }

    private void LoadTestProgram()
    {
        string path = "/home/miki/Downloads/nestest.nes";
        byte[] romBytes = File.ReadAllBytes(path);
        int prgRomStart = 0x0010;
        int prgRomSize = 0x4000;

        byte[] prgRom = new byte[prgRomSize];
        Array.Copy(romBytes, prgRomStart, prgRom, 0, prgRomSize);

        for (int i = 0; i < prgRomSize; i++)
        {
            _nes.CpuWrite((ushort)(0x8000 + i), prgRom[i]);
        }

        for (int i = 0; i < prgRomSize; i++)
        {
            _nes.CpuWrite((ushort)(0xC000 + i), prgRom[i]);
        }
    }

    private void SetupUi()
    {
        _cpuInfoTextBlock = new TextBlock
        {
            Text = "CPU Info",
            FontSize = 14,
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            TextWrapping = TextWrapping.Wrap,
            Margin = new Avalonia.Thickness(10)
        };

        _flagsPanel = new StackPanel
        {
            Orientation = Orientation.Vertical,
            Margin = new Avalonia.Thickness(10)
        };

        var separator = new Border
        {
            Background = Brushes.Gray,
            Width = 2,
            VerticalAlignment = VerticalAlignment.Stretch
        };

        var cpuInfoPanel = new StackPanel
        {
            Children = { _cpuInfoTextBlock, _flagsPanel }, // Добавляем _flagsPanel
            Margin = new Avalonia.Thickness(10)
        };

        _mainGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
        _mainGrid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(2))); // Полоса-разделитель
        _mainGrid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(200))); // Колонка CPU Info

        _mainGrid.Children.Add(separator);
        _mainGrid.Children.Add(cpuInfoPanel);

        Grid.SetColumn(separator, 1);
        Grid.SetColumn(cpuInfoPanel, 2);
    }

    private void UpdateCpuInfo()
    {
        if (!_isPaused)
        {
            _nes.Clock();
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

        if (cpu.Lookup[cpu.Opcode].Name == "BRL")
        {
            _isPaused = true;
            Title = "Tests are done";
        }

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
                Margin = new Avalonia.Thickness(4, 0)
            };

            var valueText = new TextBlock
            {
                Text = isSet ? "1" : "0",
                Foreground = color,
                FontWeight = FontWeight.Bold,
                Margin = new Avalonia.Thickness(4, 0)
            };

            flagsRow.Children.Add(flagText);
            valuesRow.Children.Add(valueText);
        }

        _flagsPanel.Children.Add(flagsRow);
        _flagsPanel.Children.Add(valuesRow);
    }
}