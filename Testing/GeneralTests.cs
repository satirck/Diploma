using Devices.Bus;
using Devices.Bus.v1;
using Devices.CPU;

namespace Testing;

public class GeneralTests
{
    [Test]
    public void TestCreatingSimpleBus()
    {
        IBus nes = new SimpleBus();
        
        if (nes.Cpu == null) Assert.Fail();
        
        Assert.Pass();
    }

    [Test]
    public void TestLoadingNesTest()
    {
        IBus nes = new SimpleBus();
        LoadNesTestProgram(nes);
        ushort resetVector = (ushort)(nes.CpuRead(0xFFFC) | (nes.CpuRead(0xFFFD) << 8));
        nes.Cpu.Pc = 0xC000;
        
        int ticks = 0;

        while (
            // (nes.Cpu.Opcode != 0x00 || ticks == 0) && 
            ticks < 1000000)
        {
            ticks++;
            nes.Clock();

            if (nes.Cpu.Complete()) 
            {
                Console.WriteLine($"Command completed at tick {ticks}");
                // Читаем результат тестов
                byte testResultLow = nes.CpuRead(0x0002);
                byte testResultHigh = nes.CpuRead(0x0003);
                ushort testResult = (ushort)(testResultLow | (testResultHigh << 8));

                Console.WriteLine($"Tick: {ticks} | Test Result: 0x{testResult:X4}");
            }
        }
    }

    private void LoadNesTestProgram(IBus nes)
    {
        string path = "/home/miki/Downloads/nestest.nes";

        if (!File.Exists(path))
        {
            Assert.Fail($"Файл {path} не найден.");
        }

        byte[] romBytes = File.ReadAllBytes(path);

        if (romBytes.Length < 0x4010)
        {
            Assert.Fail("Файл слишком мал, чтобы быть валидным ROM.");
        }
        
        int prgRomStart = 0x0010;
        int prgRomSize = 0x4000;

        byte[] prgRom = new byte[prgRomSize];
        Array.Copy(romBytes, prgRomStart, prgRom, 0, prgRomSize);

        for (int i = 0; i < prgRomSize; i++)
        {
            nes.CpuWrite((ushort)(0x8000 + i), prgRom[i]);
        }

        for (int i = 0; i < prgRomSize; i++)
        {
            nes.CpuWrite((ushort)(0xC000 + i), prgRom[i]);
        }
    }
}