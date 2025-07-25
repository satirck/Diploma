# Система сохранения состояния NES эмулятора

## Обзор

Система сохранения состояния позволяет сохранять и загружать полное состояние эмулятора NES, включая состояние CPU, PPU, картриджа и всех мапперов.

## Архитектура

### Основные компоненты

1. **Bus** - центральный компонент, координирующий сохранение/загрузку всех устройств
2. **CPU (Cpu6502)** - сохраняет состояние регистров и внутренних переменных
3. **PPU (Ppu2C02)** - сохраняет состояние видеопроцессора, включая память и регистры
4. **Cartridge** - сохраняет состояние картриджа и маппера
5. **Mapper** - базовый класс и конкретные реализации для каждого типа маппера

### Формат файла сохранения

Файл сохранения имеет следующую структуру:

```
[Версия формата: "NES_SAVE_v1.0"]
[Состояние Bus]
[Состояние CPU]
[Состояние PPU]
[Состояние Cartridge]
```

## Использование

### В коде

```csharp
// Сохранение состояния
nes.SaveState("savegame.sav");

// Загрузка состояния
nes.LoadState("savegame.sav");
```

### В UI

1. Загрузите ROM файл
2. В меню File выберите "Save State..." для сохранения
3. В меню File выберите "Load State..." для загрузки

## Поддерживаемые мапперы

### Mapper000 (NROM)
- Простой маппер без состояния
- Только базовые данные

### Mapper001 (MMC1)
- Сохраняет регистры загрузки
- Сохраняет управляющие регистры
- Сохраняет банки PRG и CHR
- Сохраняет PRG RAM

### Mapper002 (UxROM)
- Сохраняет текущий банк PRG

### Другие мапперы
- Добавьте реализацию SaveState/LoadState для каждого маппера

## Расширение системы

### Добавление нового маппера

1. Унаследуйте от базового класса `Mapper`
2. Реализуйте методы `SaveState` и `LoadState`:

```csharp
public override void SaveState(BinaryWriter writer)
{
    base.SaveState(writer);
    // Сохраните специфичные для маппера данные
    writer.Write(yourVariable);
}

public override void LoadState(BinaryReader reader)
{
    base.LoadState(reader);
    // Загрузите специфичные для маппера данные
    yourVariable = reader.ReadByte();
}
```

### Добавление новых устройств

1. Добавьте методы `SaveState` и `LoadState` в устройство
2. Вызовите их в `Bus.SaveState` и `Bus.LoadState`

## Совместимость

- Файлы сохранения привязаны к версии эмулятора
- При изменении формата сохраняется версия для обратной совместимости
- Несовместимые версии вызывают исключение

## Ограничения

- Сохранение состояния не включает внешние устройства (контроллеры)
- Размер файла может быть значительным для игр с большим объемом памяти
- Сохранение во время DMA может привести к нестабильности

## Примеры использования

### Базовое сохранение

```csharp
var nes = new Bus();
nes.InsertCartridge(cartridge);

// Запуск эмуляции
for (int i = 0; i < 1000; i++)
{
    nes.Clock();
}

// Сохранение
nes.SaveState("checkpoint.sav");
```

### Загрузка и продолжение

```csharp
// Загрузка состояния
nes.LoadState("checkpoint.sav");

// Продолжение эмуляции
for (int i = 0; i < 1000; i++)
{
    nes.Clock();
}
```

### Обработка ошибок

```csharp
try
{
    nes.LoadState("savegame.sav");
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"Failed to load state: {ex.Message}");
}
catch (FileNotFoundException)
{
    Console.WriteLine("Save file not found");
}
``` 