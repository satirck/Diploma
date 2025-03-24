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

namespace UI;

public partial class MainWindow : Window
{
    private IBus _nes;
    private TextBlock _cpuInfoTextBlock;
    private TextBlock _statusInfoTextBlock;
    private Grid _mainGrid;
    private Timer _updateTimer;

    private bool _isPaused = true;

    public MainWindow()
    {
        InitializeComponent();
        KeyDown += OnKeyDown;
        
        MinWidth = 600;
        MinHeight = 600;
        
        _nes = new SimpleBus();
        LoadTestProgram();

        _mainGrid = this.FindControl<Grid>("MainGrid");

        SetupCpuInfoPanel();

        _updateTimer = new Timer(10);
        _updateTimer.Elapsed += (_, _) => Dispatcher.UIThread.Post(UpdateCpuInfo); // Обновляем в UI потоке
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
    }

    private void LoadTestProgram()
    {
        string path = "/home/miki/Downloads/nestest.nes";

        byte[] romBytes = File.ReadAllBytes(path);

        int prgRomStart = 0x0010;
        int prgRomSize = 0x4000;
        _nes.Cpu.Pc = 0xC000;

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

    private void SetupCpuInfoPanel()
    {
        // Добавляем правую панель
        _cpuInfoTextBlock = new TextBlock
        {
            Text = "CPU Info",
            FontSize = 14,
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            TextWrapping = TextWrapping.Wrap,
            Margin = new Avalonia.Thickness(10)
        };

        _statusInfoTextBlock = new TextBlock
        {
            Text = "Status",
            FontSize = 14,
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            TextWrapping = TextWrapping.Wrap,
            Margin = new Avalonia.Thickness(10)
        };

        var separator = new Border
        {
            Background = Brushes.White,
            Width = 2
        };

        var cpuInfoPanel = new StackPanel
        {
            Children = { _cpuInfoTextBlock },
            Margin = new Avalonia.Thickness(10)
        };

        var statusPanel = new StackPanel
        {
            Children = { _statusInfoTextBlock },
            Margin = new Avalonia.Thickness(10)
        };

        // Добавляем колонки в Grid
        _mainGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
        _mainGrid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(2))); // Полоса-разделитель
        _mainGrid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(150))); // Колонка CPU
        _mainGrid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(150))); // Колонка Status

        _mainGrid.Children.Add(separator);
        _mainGrid.Children.Add(cpuInfoPanel);
        _mainGrid.Children.Add(statusPanel);

        // Указываем строки для элементов
        Grid.SetColumn(separator, 1);
        Grid.SetColumn(cpuInfoPanel, 2);
        Grid.SetColumn(statusPanel, 3);
    }

    private void UpdateCpuInfo()
    {
        if (!_isPaused)
        {
            _nes.Clock();
        }
        
        var cpu = _nes.Cpu;
        var sb = new StringBuilder();
        sb.AppendLine($"A  = 0x{cpu.A:X2}");
        sb.AppendLine($"X  = 0x{cpu.X:X2}");
        sb.AppendLine($"Y  = 0x{cpu.Y:X2}");
        sb.AppendLine($"SP = 0x{cpu.Stkp:X2}");
        sb.AppendLine($"PC = 0x{cpu.Pc:X4}");
        sb.AppendLine($"Status = 0x{cpu.Status:X2}");
        sb.AppendLine($"Opcode = {cpu.Opcode:X2} - {cpu.Lookup[cpu.Opcode].Name}");

        // Обновляем текст в UI
        _cpuInfoTextBlock.Text = sb.ToString();
        _statusInfoTextBlock.Text = $"Flags: {Convert.ToString(cpu.Status, 2).PadLeft(8, '0')}";
    }
}
