namespace NES;

public class Olc6502
{
    private Bus? _bus;

    private List<Instruction> _lookup;

    public byte A = 0x00;
    public byte X = 0x00;
    public byte Y = 0x00;
    public byte Stkp = 0x00;
    public byte Pc = 0x00;
    public byte Status = 0x00;
    public byte Fetched = 0x00;
    public ushort AddrAbs = 0x0000;
    public ushort AddRel = 0x0000;
    public byte Opcode = 0x00;
    public byte Cycles = 0;
    
    public void ConnectBus(Bus bus)
    {
        _bus = bus;
        _lookup = new List<Instruction>
        {
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
        };
    }
    
    // Addressing Modes
    public byte Imp()
    {
        Fetched = A;
        return 0;
    }

    public byte Imm()
    {
        AddrAbs = Pc++;
        return 0;
    }

    public byte Zp0()
    {
        AddrAbs = Read(Pc);
        Pc++;
        AddrAbs &= 0x00FF;
        return 0;
    }
    public byte Zpx() { return 0; }
    public byte Zpy() { return 0; }
    public byte Rel() { return 0; }
    public byte Abs() { return 0; }
    public byte Abx() { return 0; }
    public byte Aby() { return 0; }
    public byte Ind() { return 0; }
    public byte Izx() { return 0; }
    public byte Izy() { return 0; }
    
    // Opcodes
    public byte Adc() { return 0; }
    public byte And() { return 0; }
    public byte Asl() { return 0; }
    public byte Bcc() { return 0; }
    public byte Bcs() { return 0; }
    public byte Beq() { return 0; }
    public byte Bit() { return 0; }
    public byte Bmi() { return 0; }
    public byte Bne() { return 0; }
    public byte Bpl() { return 0; }
    public byte Brk() { return 0; }
    public byte Bvc() { return 0; }
    public byte Bvs() { return 0; }
    public byte Clc() { return 0; }
    public byte Cld() { return 0; }
    public byte Cli() { return 0; }
    public byte Clv() { return 0; }
    public byte Cmp() { return 0; }
    public byte Cpx() { return 0; }
    public byte Cpy() { return 0; }
    public byte Dec() { return 0; }
    public byte Dex() { return 0; }
    public byte Dey() { return 0; }
    public byte Eor() { return 0; }
    public byte Inc() { return 0; }
    public byte Inx() { return 0; }
    public byte Iny() { return 0; }
    public byte Jmp() { return 0; }
    public byte Jsr() { return 0; }
    public byte Lda() { return 0; }
    public byte Ldx() { return 0; }
    public byte Ldy() { return 0; }
    public byte Lsr() { return 0; }
    public byte Nop() { return 0; }
    public byte Ora() { return 0; }
    public byte Pha() { return 0; }
    public byte Php() { return 0; }
    public byte Pla() { return 0; }
    public byte Plp() { return 0; }
    public byte Rol() { return 0; }
    public byte Ror() { return 0; }
    public byte Rti() { return 0; }
    public byte Rts() { return 0; }
    public byte Sbc() { return 0; }
    public byte Sec() { return 0; }
    public byte Sed() { return 0; }
    public byte Sei() { return 0; }
    public byte Sta() { return 0; }
    public byte Stx() { return 0; }
    public byte Sty() { return 0; }
    public byte Tax() { return 0; }
    public byte Tay() { return 0; }
    public byte Tsx() { return 0; }
    public byte Txa() { return 0; }
    public byte Txs() { return 0; }
    public byte Tya() { return 0; }
    public byte Xxx() { return 0; }

    public void Clock()
    {
        if (Cycles == 0)
        {
            Opcode = Read(Pc);
            Pc++;
            
            Cycles = _lookup[Opcode].Cycles;

            byte addCycle1 = _lookup[Opcode].AddrMode.Invoke();
            byte addCycle2 = _lookup[Opcode].Operate.Invoke();
            
            Cycles = (byte)(Cycles + (addCycle1 & addCycle2));
        }

        Cycles--;
    }
    
    public void Reset() { }
    public void Irq() { }
    public void Nmi() { }
    
    public byte Fetch() {return 0x00; }
    
    private void Write(ushort addr, byte value) { _bus!.Write(addr, value); }
    private byte Read(ushort addr) { return _bus!.Read(addr, false); }

    private byte GetFlag(Flags6502 f)
    {
        return (byte)(((Status & (byte)f) > 0) ? 1 : 0);
    }

    private void SetFlag(Flags6502 f, bool v)
    {
        if (v)
        {
            Status = (byte)(Status | (byte)f);
        }
        else
        {
            Status = (byte)(Status & ~(byte)f);
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
        N = (1 << 7)  // Negative
    }

    protected class Instruction(string name, Func<byte> operate, Func<byte> addrMode, byte cycles)
    {
        public string Name { get; set; } = name;
        public Func<byte> Operate { get; set; } = operate;
        public Func<byte> AddrMode { get; set; } = addrMode;
        public byte Cycles { get; set; } = cycles;
    }
}
