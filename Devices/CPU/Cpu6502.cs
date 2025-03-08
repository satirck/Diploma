namespace Devices.CPU;

using Bus;

public class Cpu6502: ICpu
{
    private Bus? _bus;

    private List<Instruction> _lookup = null!;

    private byte _a = 0x00;
    private byte _x = 0x00;
    private byte _y = 0x00;
    private byte _stkp = 0x00;
    private ushort _pc = 0x0000;
    private byte _status = 0x00;
    private byte _fetched = 0x00;
    private ushort _temp = 0x0000;
    private ushort _addrAbs = 0x0000;
    private ushort _addRel = 0x0000;
    private byte _opcode = 0x00;
    private byte _cycles = 0;

    public void ConnectBus(Bus bus)
    {
        _bus = bus;
        _lookup =
        [
            new Instruction("BRK", Brk, Imm, 7),
            new Instruction("ORA", Ora, Izx, 6),
            new Instruction("???", Xxx, Imp, 2),
            new Instruction("???", Xxx, Imp, 8),
            new Instruction("???", Nop, Imp, 3),
            new Instruction("ORA", Ora, Zp0, 3),
            new Instruction("ASL", Asl, Zp0, 5),
            new Instruction("???", Xxx, Imp, 5),
            new Instruction("PHP", Php, Imp, 3),
            new Instruction("ORA", Ora, Imm, 2),
            new Instruction("ASL", Asl, Imp, 2),
            new Instruction("???", Xxx, Imp, 2),
            new Instruction("???", Nop, Imp, 4),
            new Instruction("ORA", Ora, Abs, 4),
            new Instruction("ASL", Asl, Abs, 6),
            new Instruction("???", Xxx, Imp, 6),
            new Instruction("BPL", Bpl, Rel, 2),
            new Instruction("ORA", Ora, Izy, 5),
            new Instruction("???", Xxx, Imp, 2),
            new Instruction("???", Xxx, Imp, 8),
            new Instruction("???", Nop, Imp, 4),
            new Instruction("ORA", Ora, Zpx, 4),
            new Instruction("ASL", Asl, Zpx, 6),
            new Instruction("???", Xxx, Imp, 6),
            new Instruction("CLC", Clc, Imp, 2),
            new Instruction("ORA", Ora, Aby, 4),
            new Instruction("???", Nop, Imp, 2),
            new Instruction("???", Xxx, Imp, 7),
            new Instruction("???", Nop, Imp, 4),
            new Instruction("ORA", Ora, Abx, 4),
            new Instruction("ASL", Asl, Abx, 7),
            new Instruction("???", Xxx, Imp, 7),
            new Instruction("JSR", Jsr, Abs, 6),
            new Instruction("AND", And, Izx, 6),
            new Instruction("???", Xxx, Imp, 2),
            new Instruction("???", Xxx, Imp, 8),
            new Instruction("BIT", Bit, Zp0, 3),
            new Instruction("AND", And, Zp0, 3),
            new Instruction("ROL", Rol, Zp0, 5),
            new Instruction("???", Xxx, Imp, 5),
            new Instruction("PLP", Plp, Imp, 4),
            new Instruction("AND", And, Imm, 2),
            new Instruction("ROL", Rol, Imp, 2),
            new Instruction("???", Xxx, Imp, 2),
            new Instruction("BIT", Bit, Abs, 4),
            new Instruction("AND", And, Abs, 4),
            new Instruction("ROL", Rol, Abs, 6),
            new Instruction("???", Xxx, Imp, 6),
            new Instruction("BMI", Bmi, Rel, 2),
            new Instruction("AND", And, Izy, 5),
            new Instruction("???", Xxx, Imp, 2),
            new Instruction("???", Xxx, Imp, 8),
            new Instruction("???", Nop, Imp, 4),
            new Instruction("AND", And, Zpx, 4),
            new Instruction("ROL", Rol, Zpx, 6),
            new Instruction("???", Xxx, Imp, 6),
            new Instruction("SEC", Sec, Imp, 2),
            new Instruction("AND", And, Aby, 4),
            new Instruction("???", Nop, Imp, 2),
            new Instruction("???", Xxx, Imp, 7),
            new Instruction("???", Nop, Imp, 4),
            new Instruction("AND", And, Abx, 4),
            new Instruction("ROL", Rol, Abx, 7),
            new Instruction("???", Xxx, Imp, 7),
            new Instruction("RTI", Rti, Imp, 6),
            new Instruction("EOR", Eor, Izx, 6),
            new Instruction("???", Xxx, Imp, 2),
            new Instruction("???", Xxx, Imp, 8),
            new Instruction("???", Nop, Imp, 3),
            new Instruction("EOR", Eor, Zp0, 3),
            new Instruction("LSR", Lsr, Zp0, 5),
            new Instruction("???", Xxx, Imp, 5),
            new Instruction("PHA", Pha, Imp, 3),
            new Instruction("EOR", Eor, Imm, 2),
            new Instruction("LSR", Lsr, Imp, 2),
            new Instruction("???", Xxx, Imp, 2),
            new Instruction("JMP", Jmp, Abs, 3),
            new Instruction("EOR", Eor, Abs, 4),
            new Instruction("LSR", Lsr, Abs, 6),
            new Instruction("???", Xxx, Imp, 6),
            new Instruction("BVC", Bvc, Rel, 2),
            new Instruction("EOR", Eor, Izy, 5),
            new Instruction("???", Xxx, Imp, 2),
            new Instruction("???", Xxx, Imp, 8),
            new Instruction("???", Nop, Imp, 4),
            new Instruction("EOR", Eor, Zpx, 4),
            new Instruction("LSR", Lsr, Zpx, 6),
            new Instruction("???", Xxx, Imp, 6),
            new Instruction("CLI", Cli, Imp, 2),
            new Instruction("EOR", Eor, Aby, 4),
            new Instruction("???", Nop, Imp, 2),
            new Instruction("???", Xxx, Imp, 7),
            new Instruction("???", Nop, Imp, 4),
            new Instruction("EOR", Eor, Abx, 4),
            new Instruction("LSR", Lsr, Abx, 7),
            new Instruction("???", Xxx, Imp, 7),
            new Instruction("RTS", Rts, Imp, 6),
            new Instruction("ADC", Adc, Izx, 6),
            new Instruction("???", Xxx, Imp, 2),
            new Instruction("???", Xxx, Imp, 8),
            new Instruction("???", Nop, Imp, 3),
            new Instruction("ADC", Adc, Zp0, 3),
            new Instruction("ROR", Ror, Zp0, 5),
            new Instruction("???", Xxx, Imp, 5),
            new Instruction("PLA", Pla, Imp, 4),
            new Instruction("ADC", Adc, Imm, 2),
            new Instruction("ROR", Ror, Imp, 2),
            new Instruction("???", Xxx, Imp, 2),
            new Instruction("JMP", Jmp, Ind, 5),
            new Instruction("ADC", Adc, Abs, 4),
            new Instruction("ROR", Ror, Abs, 6),
            new Instruction("???", Xxx, Imp, 6),
            new Instruction("BVS", Bvs, Rel, 2),
            new Instruction("ADC", Adc, Izy, 5),
            new Instruction("???", Xxx, Imp, 2),
            new Instruction("???", Xxx, Imp, 8),
            new Instruction("???", Nop, Imp, 4),
            new Instruction("ADC", Adc, Zpx, 4),
            new Instruction("ROR", Ror, Zpx, 6),
            new Instruction("???", Xxx, Imp, 6),
            new Instruction("SEI", Sei, Imp, 2),
            new Instruction("ADC", Adc, Aby, 4),
            new Instruction("???", Nop, Imp, 2),
            new Instruction("???", Xxx, Imp, 7),
            new Instruction("???", Nop, Imp, 4),
            new Instruction("ADC", Adc, Abx, 4),
            new Instruction("ROR", Ror, Abx, 7),
            new Instruction("???", Xxx, Imp, 7),
            new Instruction("???", Nop, Imp, 2),
            new Instruction("STA", Sta, Izx, 6),
            new Instruction("???", Nop, Imp, 2),
            new Instruction("???", Xxx, Imp, 6),
            new Instruction("STY", Sty, Zp0, 3),
            new Instruction("STA", Sta, Zp0, 3),
            new Instruction("STX", Stx, Zp0, 3),
            new Instruction("???", Xxx, Imp, 3),
            new Instruction("DEY", Dey, Imp, 2),
            new Instruction("???", Nop, Imp, 2),
            new Instruction("TXA", Txa, Imp, 2),
            new Instruction("???", Xxx, Imp, 2),
            new Instruction("STY", Sty, Abs, 4),
            new Instruction("STA", Sta, Abs, 4),
            new Instruction("STX", Stx, Abs, 4),
            new Instruction("???", Xxx, Imp, 4),
            new Instruction("BCC", Bcc, Rel, 2),
            new Instruction("STA", Sta, Izy, 6),
            new Instruction("???", Xxx, Imp, 2),
            new Instruction("???", Xxx, Imp, 6),
            new Instruction("STY", Sty, Zpx, 4),
            new Instruction("STA", Sta, Zpx, 4),
            new Instruction("STX", Stx, Zpy, 4),
            new Instruction("???", Xxx, Imp, 4),
            new Instruction("TYA", Tya, Imp, 2),
            new Instruction("STA", Sta, Aby, 5),
            new Instruction("TXS", Txs, Imp, 2),
            new Instruction("???", Xxx, Imp, 5),
            new Instruction("???", Nop, Imp, 5),
            new Instruction("STA", Sta, Abx, 5),
            new Instruction("???", Xxx, Imp, 5),
            new Instruction("???", Xxx, Imp, 5),
            new Instruction("LDY", Ldy, Imm, 2),
            new Instruction("LDA", Lda, Izx, 6),
            new Instruction("LDX", Ldx, Imm, 2),
            new Instruction("???", Xxx, Imp, 6),
            new Instruction("LDY", Ldy, Zp0, 3),
            new Instruction("LDA", Lda, Zp0, 3),
            new Instruction("LDX", Ldx, Zp0, 3),
            new Instruction("???", Xxx, Imp, 3),
            new Instruction("TAY", Tay, Imp, 2),
            new Instruction("LDA", Lda, Imm, 2),
            new Instruction("TAX", Tax, Imp, 2),
            new Instruction("???", Xxx, Imp, 2),
            new Instruction("LDY", Ldy, Abs, 4),
            new Instruction("LDA", Lda, Abs, 4),
            new Instruction("LDX", Ldx, Abs, 4),
            new Instruction("???", Xxx, Imp, 4),
            new Instruction("BCS", Bcs, Rel, 2),
            new Instruction("LDA", Lda, Izy, 5),
            new Instruction("???", Xxx, Imp, 2),
            new Instruction("???", Xxx, Imp, 5),
            new Instruction("LDY", Ldy, Zpx, 4),
            new Instruction("LDA", Lda, Zpx, 4),
            new Instruction("LDX", Ldx, Zpy, 4),
            new Instruction("???", Xxx, Imp, 4),
            new Instruction("CLV", Clv, Imp, 2),
            new Instruction("LDA", Lda, Aby, 4),
            new Instruction("TSX", Tsx, Imp, 2),
            new Instruction("???", Xxx, Imp, 4),
            new Instruction("LDY", Ldy, Abx, 4),
            new Instruction("LDA", Lda, Abx, 4),
            new Instruction("LDX", Ldx, Aby, 4),
            new Instruction("???", Xxx, Imp, 4),
            new Instruction("CPY", Cpy, Imm, 2),
            new Instruction("CMP", Cmp, Izx, 6),
            new Instruction("???", Nop, Imp, 2),
            new Instruction("???", Xxx, Imp, 8),
            new Instruction("CPY", Cpy, Zp0, 3),
            new Instruction("CMP", Cmp, Zp0, 3),
            new Instruction("DEC", Dec, Zp0, 5),
            new Instruction("???", Xxx, Imp, 5),
            new Instruction("INY", Iny, Imp, 2),
            new Instruction("CMP", Cmp, Imm, 2),
            new Instruction("DEX", Dex, Imp, 2),
            new Instruction("???", Xxx, Imp, 2),
            new Instruction("CPY", Cpy, Abs, 4),
            new Instruction("CMP", Cmp, Abs, 4),
            new Instruction("DEC", Dec, Abs, 6),
            new Instruction("???", Xxx, Imp, 6),
            new Instruction("BNE", Bne, Rel, 2),
            new Instruction("CMP", Cmp, Izy, 5),
            new Instruction("???", Xxx, Imp, 2),
            new Instruction("???", Xxx, Imp, 8),
            new Instruction("???", Nop, Imp, 4),
            new Instruction("CMP", Cmp, Zpx, 4),
            new Instruction("DEC", Dec, Zpx, 6),
            new Instruction("???", Xxx, Imp, 6),
            new Instruction("CLD", Cld, Imp, 2),
            new Instruction("CMP", Cmp, Aby, 4),
            new Instruction("NOP", Nop, Imp, 2),
            new Instruction("???", Xxx, Imp, 7),
            new Instruction("???", Nop, Imp, 4),
            new Instruction("CMP", Cmp, Abx, 4),
            new Instruction("DEC", Dec, Abx, 7),
            new Instruction("???", Xxx, Imp, 7),
            new Instruction("CPX", Cpx, Imm, 2),
            new Instruction("SBC", Sbc, Izx, 6),
            new Instruction("???", Nop, Imp, 2),
            new Instruction("???", Xxx, Imp, 8),
            new Instruction("CPX", Cpx, Zp0, 3),
            new Instruction("SBC", Sbc, Zp0, 3),
            new Instruction("INC", Inc, Zp0, 5),
            new Instruction("???", Xxx, Imp, 5),
            new Instruction("INX", Inx, Imp, 2),
            new Instruction("SBC", Sbc, Imm, 2),
            new Instruction("NOP", Nop, Imp, 2),
            new Instruction("???", Sbc, Imp, 2),
            new Instruction("CPX", Cpx, Abs, 4),
            new Instruction("SBC", Sbc, Abs, 4),
            new Instruction("INC", Inc, Abs, 6),
            new Instruction("???", Xxx, Imp, 6),
            new Instruction("BEQ", Beq, Rel, 2),
            new Instruction("SBC", Sbc, Izy, 5),
            new Instruction("???", Xxx, Imp, 2),
            new Instruction("???", Xxx, Imp, 8),
            new Instruction("???", Nop, Imp, 4),
            new Instruction("SBC", Sbc, Zpx, 4),
            new Instruction("INC", Inc, Zpx, 6),
            new Instruction("???", Xxx, Imp, 6),
            new Instruction("SED", Sed, Imp, 2),
            new Instruction("SBC", Sbc, Aby, 4),
            new Instruction("NOP", Nop, Imp, 2),
            new Instruction("???", Xxx, Imp, 7),
            new Instruction("???", Nop, Imp, 4),
            new Instruction("SBC", Sbc, Abx, 4),
            new Instruction("INC", Inc, Abx, 7),
            new Instruction("???", Xxx, Imp, 7)
        ];
    }

    // Addressing Modes
    public byte Imp()
    {
        _fetched = _a;
        return 0;
    }

    public byte Imm()
    {
        _addrAbs = _pc++;
        return 0;
    }

    public byte Zp0()
    {
        _addrAbs = (ushort)(Read(_pc) & -0x00FF);
        _pc++;
        return 0;
    }

    public byte Zpx()
    {
        _addrAbs = (ushort)((Read(_pc) + _x) & 0x00FF);
        _pc++;
        return 0;
    }

    public byte Zpy()
    {
        _addrAbs = (ushort)((Read(_pc) + _y) & 0x00FF);
        _pc++;
        return 0;
    }

    public byte Rel()
    {
        _addRel = Read(_pc);
        _pc++;

        if ((_addRel & 0x80) != 0)
        {
            _addRel |= 0xFF00;
        }

        return 0;
    }

    public byte Abs()
    {
        ushort lo = Read(_pc);
        _pc++;
        ushort hi = Read(_pc);
        _pc++;

        _addrAbs = (ushort)((hi << 8) | lo);

        return 0;
    }

    public byte Abx()
    {
        ushort lo = Read(_pc);
        _pc++;
        ushort hi = Read(_pc);
        _pc++;

        _addrAbs = (ushort)((hi << 8) | lo);
        _addrAbs += _x;

        return (byte)((_addrAbs & 0xFF00) != hi << 8 ? 1 : 0);
    }

    public byte Aby()
    {
        ushort lo = Read(_pc);
        _pc++;
        ushort hi = Read(_pc);
        _pc++;

        _addrAbs = (ushort)((hi << 8) | lo);
        _addrAbs += _y;

        return (byte)((_addrAbs & 0xFF00) != hi << 8 ? 1 : 0);
    }

    public byte Ind()
    {
        ushort ptrLo = Read(_pc);
        _pc++;
        ushort ptrHi = Read(_pc);
        _pc++;

        ushort ptr = (ushort)((ptrHi << 8) | ptrLo);
        ushort ptrInc = (ushort)(ptr + 1);

        if (ptrLo == 0x00FF) // page boundary bug behavior
        {
            _addrAbs = (ushort)((Read((ushort)(ptr & 0xFF00)) << 8) | Read(ptr));
        }
        else // normal behavior
        {
            _addrAbs = (ushort)(Read(ptrInc) << 8 | Read(ptr));
        }

        return 0;
    }

    public byte Izx()
    {
        ushort t = Read(_pc);
        _pc++;

        ushort lo = Read((ushort)((t + _x) & 0x00FF));
        ushort hi = Read((ushort)((t + _x + 1) & 0x00FF));

        _addrAbs = (ushort)((hi << 8) | lo);

        return 0;
    }

    public byte Izy()
    {
        ushort t = Read(_pc);
        _pc++;

        ushort lo = Read((ushort)(t & 0x00FF));
        ushort hi = Read((ushort)((t + 1) & 0x00FF));

        _addrAbs = (ushort)(((hi << 8) | lo) + _y);

        if ((_addrAbs & 0xFF00) != (hi << 8))
            return 1;

        return 0;
    }

    // Opcodes
    public byte Adc()
    {
        Fetch();
        ushort temp = (ushort)(_a + _fetched + GetFlag(Flags6502.C));
        SetFlag(Flags6502.C, temp > 255);
        SetFlag(Flags6502.Z, (temp & 0x00FF) == 0);
        SetFlag(Flags6502.V, (~(_a ^ _fetched) & (_a ^ temp) & 0x0080) != 0);
        SetFlag(Flags6502.N, (temp & 0x80) != 0);

        _a = (byte)(temp & 0x00FF);

        return 1;
    }

    public byte And()
    {
        Fetch();
        _a = (byte)(_a & _fetched);

        SetFlag(Flags6502.Z, _a == 0x00);
        SetFlag(Flags6502.N, (_a & 0x80) != 0);

        return 1;
    }

    public byte Asl()
    {
        Fetch();
        ushort temp = (ushort)(_fetched << 1);

        SetFlag(Flags6502.C, (temp & 0xFF00) != 0);
        SetFlag(Flags6502.Z, (temp & 0x00FF) == 0);
        SetFlag(Flags6502.N, (temp & 0x80) != 0);

        if (_lookup[_opcode].AddrMode == Imp)
            _a = (byte)(temp & 0x00FF);
        else
            Write(_addrAbs, (byte)(temp & 0x00FF));

        return 0;
    }


    public byte Bcc()
    {
        if (GetFlag(Flags6502.C) == 0)
        {
            _cycles++;
            _addrAbs = (ushort)(_pc + _addRel);

            if ((_addrAbs & 0xFF00) != (_pc & 0xFF00))
            {
                _cycles++;
            }

            _pc = _addrAbs;
        }

        return 0;
    }

    public byte Bcs()
    {
        if (GetFlag(Flags6502.C) == 1)
        {
            _cycles++;
            _addrAbs = (ushort)(_pc + _addRel);

            if ((_addrAbs & 0xFF00) != (_pc & 0xFF00))
            {
                _cycles++;
            }

            _pc = _addrAbs;
        }

        return 0;
    }

    public byte Beq()
    {
        if (GetFlag(Flags6502.Z) == 1)
        {
            _cycles++;
            _addrAbs = (ushort)(_pc + _addRel);

            if ((_addrAbs & 0xFF00) != (_pc & 0xFF00))
            {
                _cycles++;
            }

            _pc = _addrAbs;
        }

        return 0;
    }

    public byte Bit()
    {
        Fetch();
        ushort temp = (ushort)(_a & _fetched);

        SetFlag(Flags6502.Z, (temp & 0x00FF) == 0);
        SetFlag(Flags6502.N, (_fetched & (1 << 7)) != 0);
        SetFlag(Flags6502.V, (_fetched & (1 << 6)) != 0);

        return 0;
    }

    public byte Bmi()
    {
        if (GetFlag(Flags6502.N) == 1)
        {
            _cycles++;
            _addrAbs = (ushort)(_pc + _addRel);

            if ((_addrAbs & 0xFF00) != (_pc & 0xFF00))
            {
                _cycles++;
            }

            _pc = _addrAbs;
        }

        return 0;
    }

    public byte Bne()
    {
        if (GetFlag(Flags6502.Z) == 0)
        {
            _cycles++;
            _addrAbs = (ushort)(_pc + _addRel);

            if ((_addrAbs & 0xFF00) != (_pc & 0xFF00))
            {
                _cycles++;
            }

            _pc = _addrAbs;
        }

        return 0;
    }

    public byte Bpl()
    {
        if (GetFlag(Flags6502.N) == 0)
        {
            _cycles++;
            _addrAbs = (ushort)(_pc + _addRel);

            if ((_addrAbs & 0xFF00) != (_pc & 0xFF00))
            {
                _cycles++;
            }

            _pc = _addrAbs;
        }

        return 0;
    }

    public byte Brk()
    {
        _pc++;

        SetFlag(Flags6502.I, true);
        Write((ushort)(0x0100 + _stkp), (byte)((_pc >> 8) & 0x00FF));
        _stkp--;
        Write((ushort)(0x0100 + _stkp), (byte)(_pc & 0x00FF));
        _stkp--;

        SetFlag(Flags6502.B, true);
        Write((ushort)(0x0100 + _stkp), _status);
        _stkp--;
        SetFlag(Flags6502.B, false);

        _pc = (ushort)(Read(0xFFFE) | (Read(0xFFFF) << 8));

        return 0;
    }

    public byte Bvc()
    {
        if (GetFlag(Flags6502.V) == 0)
        {
            _cycles++;
            _addrAbs = (ushort)(_pc + _addRel);

            if ((_addrAbs & 0xFF00) != (_pc & 0xFF00))
            {
                _cycles++;
            }

            _pc = _addrAbs;
        }

        return 0;
    }

    public byte Bvs()
    {
        if (GetFlag(Flags6502.V) == 1)
        {
            _cycles++;
            _addrAbs = (ushort)(_pc + _addRel);

            if ((_addrAbs & 0xFF00) != (_pc & 0xFF00))
            {
                _cycles++;
            }

            _pc = _addrAbs;
        }

        return 0;
    }

    public byte Clc()
    {
        SetFlag(Flags6502.C, false);
        return 0;
    }

    public byte Cld()
    {
        SetFlag(Flags6502.D, false);
        return 0;
    }

    public byte Cli()
    {
        SetFlag(Flags6502.I, false);
        return 0;
    }

    public byte Clv()
    {
        SetFlag(Flags6502.V, false);
        return 0;
    }

    public byte Cmp()
    {
        Fetch();
        ushort temp = (ushort)(_a - _fetched);

        SetFlag(Flags6502.C, _a >= _fetched);
        SetFlag(Flags6502.Z, (temp & 0x00FF) == 0);
        SetFlag(Flags6502.N, (temp & 0x0080) != 0);

        return 1;
    }

    public byte Cpx()
    {
        Fetch();
        ushort temp = (ushort)(_x - _fetched);

        SetFlag(Flags6502.C, _x >= _fetched);
        SetFlag(Flags6502.Z, (temp & 0x00FF) == 0);
        SetFlag(Flags6502.N, (temp & 0x0080) != 0);

        return 0;
    }

    public byte Cpy()
    {
        Fetch();
        ushort temp = (ushort)(_y - _fetched);

        SetFlag(Flags6502.C, _y >= _fetched);
        SetFlag(Flags6502.Z, (temp & 0x00FF) == 0);
        SetFlag(Flags6502.N, (temp & 0x0080) != 0);

        return 0;
    }

    public byte Dec()
    {
        Fetch();
        ushort temp = (ushort)(_fetched - 1);
        Write(_addrAbs, (byte)(temp & 0x00FF));

        SetFlag(Flags6502.Z, (temp & 0x00FF) == 0);
        SetFlag(Flags6502.N, (temp & 0x0080) != 0);

        return 0;
    }

    public byte Dex()
    {
        _x--;
        SetFlag(Flags6502.Z, _x == 0x00);
        SetFlag(Flags6502.N, (_x & 0x80) != 0);

        return 0;
    }

    public byte Dey()
    {
        _y--;
        SetFlag(Flags6502.Z, _y == 0x00);
        SetFlag(Flags6502.N, (_y & 0x80) != 0);

        return 0;
    }

    public byte Eor()
    {
        Fetch();
        _a ^= _fetched;
        SetFlag(Flags6502.Z, _a == 0x00);
        SetFlag(Flags6502.N, (_a & 0x80) != 0);
        return 1;
    }

    public byte Inc()
    {
        Fetch();
        ushort temp = (ushort)(_fetched + 1);
        Write(_addrAbs, (byte)(temp & 0x00FF));

        SetFlag(Flags6502.Z, (temp & 0x00FF) == 0);
        SetFlag(Flags6502.N, (temp & 0x0080) != 0);

        return 0;
    }

    public byte Inx()
    {
        _x++;
        SetFlag(Flags6502.Z, _x == 0x00);
        SetFlag(Flags6502.N, (_x & 0x80) != 0);
        return 0;
    }

    public byte Iny()
    {
        _y++;
        SetFlag(Flags6502.Z, _y == 0x00);
        SetFlag(Flags6502.N, (_y & 0x80) != 0);
        return 0;
    }

    public byte Jmp()
    {
        _pc = _addrAbs;
        return 0;
    }

    public byte Jsr()
    {
        _pc--; // Decrement PC before pushing onto the stack

        Write((ushort)(0x0100 + _stkp), (byte)((_pc >> 8) & 0x00FF));
        _stkp--;
        Write((ushort)(0x0100 + _stkp), (byte)(_pc & 0x00FF));
        _stkp--;

        _pc = _addrAbs;
        return 0;
    }

    public byte Lda()
    {
        Fetch();
        _a = _fetched;
        SetFlag(Flags6502.Z, _a == 0x00);
        SetFlag(Flags6502.N, (_a & 0x80) != 0);
        return 1;
    }

    public byte Ldx()
    {
        Fetch();
        _x = _fetched;
        SetFlag(Flags6502.Z, _x == 0x00);
        SetFlag(Flags6502.N, (_x & 0x80) != 0);
        return 1;
    }

    public byte Ldy()
    {
        Fetch();
        _y = _fetched;
        SetFlag(Flags6502.Z, _y == 0x00);
        SetFlag(Flags6502.N, (_y & 0x80) != 0);
        return 1;
    }

    public byte Lsr()
    {
        Fetch();
        SetFlag(Flags6502.C, (_fetched & 0x0001) != 0);
        ushort temp = (ushort)(_fetched >> 1);
        SetFlag(Flags6502.Z, (temp & 0x00FF) == 0x0000);
        SetFlag(Flags6502.N, (temp & 0x0080) != 0);

        if (_lookup[_opcode].AddrMode == Imp)
            _a = (byte)(temp & 0x00FF);
        else
            Write(_addrAbs, (byte)(temp & 0x00FF));

        return 0;
    }

    public byte Nop()
    {
        // Некоторые NOP могут быть разными
        // Дополнительные операции основаны на https://wiki.nesdev.com/w/index.php/CPU_unofficial_opcodes
        // и будут добавляться для совместимости с играми
        // В идеале будет покрыта вся незаконная операция
        switch (_opcode)
        {
            case 0x1C:
            case 0x3C:
            case 0x5C:
            case 0x7C:
            case 0xDC:
            case 0xFC:
                return 1;
        }

        return 0;
    }

    public byte Ora()
    {
        Fetch();
        _a = (byte)(_a | _fetched);
        SetFlag(Flags6502.Z, _a == 0x00);
        SetFlag(Flags6502.N, (_a & 0x80) != 0);
        return 1;
    }

    public byte Pha()
    {
        Write((ushort)(0x0100 + _stkp), _a);
        _stkp--;
        return 0;
    }

    public byte Php()
    {
        Write((ushort)(0x0100 + _stkp), (byte)(_status | (byte)Flags6502.B | (byte)Flags6502.U));
        SetFlag(Flags6502.B, false);
        SetFlag(Flags6502.U, false);
        _stkp--;
        return 0;
    }

    public byte Pla()
    {
        _stkp++;
        _a = Read((ushort)(0x0100 + _stkp));
        SetFlag(Flags6502.Z, _a == 0x00);
        SetFlag(Flags6502.N, (_a & 0x80) != 0);
        return 0;
    }

    public byte Plp()
    {
        _stkp++;
        _status = Read((ushort)(0x0100 + _stkp));
        SetFlag(Flags6502.U, true);
        return 0;
    }

    public byte Rol()
    {
        Fetch();
        _temp = (ushort)((_fetched << 1) | GetFlag(Flags6502.C));
        SetFlag(Flags6502.C, (_temp & 0xFF00) != 0);
        SetFlag(Flags6502.Z, (_temp & 0x00FF) == 0x00);
        SetFlag(Flags6502.N, (_temp & 0x0080) != 0);

        if (_lookup[_opcode].AddrMode == Imp)
            _a = (byte)(_temp & 0x00FF);
        else
            Write(_addrAbs, (byte)(_temp & 0x00FF));

        return 0;
    }

    public byte Ror()
    {
        Fetch();
        _temp = (ushort)((GetFlag(Flags6502.C) << 7) | (_fetched >> 1));
        SetFlag(Flags6502.C, (_fetched & 0x01) != 0);
        SetFlag(Flags6502.Z, (_temp & 0x00FF) == 0x00);
        SetFlag(Flags6502.N, (_temp & 0x0080) != 0);

        if (_lookup[_opcode].AddrMode == Imp)
            _a = (byte)(_temp & 0x00FF);
        else
            Write(_addrAbs, (byte)(_temp & 0x00FF));

        return 0;
    }

    public byte Rti()
    {
        _stkp++;
        _status = Read((ushort)(0x0100 + _stkp));

        //TODO: check this if it is correct
        _status &= (byte)~(Flags6502.B | Flags6502.U);

        _stkp++;
        _pc = Read((ushort)(0x0100 + _stkp));
        _stkp++;
        _pc |= (ushort)(Read((ushort)(0x0100 + _stkp)) << 8);
        return 0;
    }

    public byte Rts()
    {
        _stkp++;
        _pc = Read((ushort)(0x0100 + _stkp));
        _stkp++;
        _pc |= (ushort)(Read((ushort)(0x0100 + _stkp)) << 8);

        _pc++;
        return 0;
    }

    public byte Sbc()
    {
        Fetch();

        ushort value = (ushort)(_fetched ^ 0x00FF);

        ushort temp = (ushort)(_a + value + GetFlag(Flags6502.C));
        SetFlag(Flags6502.C, (temp & 0xFF00) != 0);
        SetFlag(Flags6502.Z, (temp & 0x00FF) == 0);
        SetFlag(Flags6502.V, ((temp ^ _a) & (temp ^ value) & 0x0080) != 0);
        SetFlag(Flags6502.N, (temp & 0x0080) != 0);

        _a = (byte)(temp & 0x00FF);

        return 1;
    }

    public byte Sec()
    {
        SetFlag(Flags6502.C, true);
        return 0;
    }

    public byte Sed()
    {
        SetFlag(Flags6502.D, true);
        return 0;
    }

    public byte Sei()
    {
        SetFlag(Flags6502.I, true);
        return 0;
    }

    public byte Sta()
    {
        Write(_addrAbs, _a);
        return 0;
    }

    public byte Stx()
    {
        Write(_addrAbs, _x);
        return 0;
    }

    public byte Sty()
    {
        Write(_addrAbs, _y);
        return 0;
    }

    public byte Tax()
    {
        _x = _a;
        SetFlag(Flags6502.Z, _x == 0x00);
        SetFlag(Flags6502.N, (_x & 0x80) != 0);
        return 0;
    }

    public byte Tay()
    {
        _y = _a;
        SetFlag(Flags6502.Z, _y == 0x00);
        SetFlag(Flags6502.N, (_y & 0x80) != 0);
        return 0;
    }

    public byte Tsx()
    {
        _x = _stkp;
        SetFlag(Flags6502.Z, _x == 0x00);
        SetFlag(Flags6502.N, (_x & 0x80) != 0);
        return 0;
    }

    public byte Txa()
    {
        _a = _x;
        SetFlag(Flags6502.Z, _a == 0x00);
        SetFlag(Flags6502.N, (_a & 0x80) != 0);
        return 0;
    }

    public byte Txs()
    {
        _stkp = _x;
        return 0;
    }

    public byte Tya()
    {
        _a = _y;
        SetFlag(Flags6502.Z, _a == 0x00);
        SetFlag(Flags6502.N, (_a & 0x80) != 0);
        return 0;
    }

    public byte Xxx()
    {
        return 0;
    }

    public void Clock()
    {
        if (_cycles == 0)
        {
            _opcode = Read(_pc);
            _pc++;

            _cycles = _lookup[_opcode].Cycles;

            byte addCycle1 = _lookup[_opcode].AddrMode.Invoke();
            byte addCycle2 = _lookup[_opcode].Operate.Invoke();

            _cycles = (byte)(_cycles + (addCycle1 & addCycle2));
        }

        _cycles--;
    }

    public void Reset()
    {
        _a = _x = _y = 0;
        _stkp = 0xFD;
        _status = (byte)(0x00 | Flags6502.U);

        _addrAbs = 0xFFFC;
        ushort lo = Read((ushort)(_addrAbs + 0));
        ushort hi = Read((ushort)(_addrAbs + 1));

        _pc = (ushort)((hi << 8) | lo);
        _addRel = _addrAbs = 0x0000;
        _fetched = 0x00;

        _cycles = 8;
    }

    public void Irq()
    {
        if (GetFlag(Flags6502.I) == 0)
        {
            Write((ushort)(0x0100 + _stkp), (byte)((_pc >> 8) & 0x00FF));
            _stkp--;
            Write((ushort)(0x0100 + _stkp), (byte)(_pc & 0x00FF));
            _stkp--;

            SetFlag(Flags6502.B, false);
            SetFlag(Flags6502.U, true);
            SetFlag(Flags6502.I, true);

            Write((ushort)(0x0100 + _stkp), _status);
            _stkp--;

            _addrAbs = 0xFFFE;
            ushort lo = Read((ushort)(_addrAbs + 0));
            ushort hi = Read((ushort)(_addrAbs + 1));
            _pc = (ushort)((hi << 8) | lo);

            _cycles = 7;
        }
    }

    public void Nmi()
    {
        Write((ushort)(0x0100 + _stkp), (byte)((_pc >> 8) & 0x00FF));
        _stkp--;
        Write((ushort)(0x0100 + _stkp), (byte)(_pc & 0x00FF));
        _stkp--;

        SetFlag(Flags6502.B, false);
        SetFlag(Flags6502.U, true);
        SetFlag(Flags6502.I, true);

        Write((ushort)(0x0100 + _stkp), _status);
        _stkp--;

        _addrAbs = 0xFFFA;
        ushort lo = Read((ushort)(_addrAbs + 0));
        ushort hi = Read((ushort)(_addrAbs + 1));
        _pc = (ushort)((hi << 8) | lo);

        _cycles = 8;
    }

    public byte Fetch()
    {
        if (_lookup[_opcode].AddrMode != Imp)
        {
            _fetched = Read(_addrAbs);
        }

        return _fetched;
    }

    private void Write(ushort addr, byte value)
    {
        _bus!.Write(addr, value);
    }

    private byte Read(ushort addr)
    {
        return _bus!.Read(addr);
    }

    private byte GetFlag(Flags6502 f)
    {
        return (byte)(((_status & (byte)f) > 0) ? 1 : 0);
    }

    private void SetFlag(Flags6502 f, bool v)
    {
        if (v)
        {
            _status = (byte)(_status | (byte)f);
        }
        else
        {
            _status = (byte)(_status & ~(byte)f);
        }
    }

    [Flags]
    private enum Flags6502 : byte
    {
        C = (1 << 0), // Carry Bit
        Z = (1 << 1), // Zero
        I = (1 << 2), // Disable Interrupts
        D = (1 << 3), // Decimal Mode (unused in this implementation)
        B = (1 << 4), // Break
        U = (1 << 5), // Unused
        V = (1 << 6), // Overflow
        N = (1 << 7) // Negative
    }
}