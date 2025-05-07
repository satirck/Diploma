namespace Devices.CPU;

public partial class Cpu6502
{
    public Cpu6502()
    {
        Lookup =
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
        Lookup =
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
        AddrAbs = (ushort)(Read(Pc) & -0x00FF);
        Pc++;
        return 0;
    }

    public byte Zpx()
    {
        AddrAbs = (ushort)((Read(Pc) + X) & 0x00FF);
        Pc++;
        return 0;
    }

    public byte Zpy()
    {
        AddrAbs = (ushort)((Read(Pc) + Y) & 0x00FF);
        Pc++;
        return 0;
    }

    public byte Rel()
    {
        AddRel = Read(Pc);
        Pc++;

        if ((AddRel & 0x80) != 0)
        {
            AddRel |= 0xFF00;
        }

        return 0;
    }

    public byte Abs()
    {
        ushort lo = Read(Pc);
        Pc++;
        ushort hi = Read(Pc);
        Pc++;

        AddrAbs = (ushort)((hi << 8) | lo);
        // Console.WriteLine($"Cpu: AddrAbs = {AddrAbs}");
        return 0;
    }

    public byte Abx()
    {
        ushort lo = Read(Pc);
        Pc++;
        ushort hi = Read(Pc);
        Pc++;

        AddrAbs = (ushort)((hi << 8) | lo);
        AddrAbs += X;

        return (byte)((AddrAbs & 0xFF00) != hi << 8 ? 1 : 0);
    }

    public byte Aby()
    {
        ushort lo = Read(Pc);
        Pc++;
        ushort hi = Read(Pc);
        Pc++;

        AddrAbs = (ushort)((hi << 8) | lo);
        AddrAbs += Y;

        return (byte)((AddrAbs & 0xFF00) != hi << 8 ? 1 : 0);
    }

    public byte Ind()
    {
        ushort ptrLo = Read(Pc);
        Pc++;
        ushort ptrHi = Read(Pc);
        Pc++;

        ushort ptr = (ushort)((ptrHi << 8) | ptrLo);
        ushort ptrInc = (ushort)(ptr + 1);

        if (ptrLo == 0x00FF) // page boundary bug behavior
        {
            AddrAbs = (ushort)((Read((ushort)(ptr & 0xFF00)) << 8) | Read(ptr));
        }
        else // normal behavior
        {
            AddrAbs = (ushort)(Read(ptrInc) << 8 | Read(ptr));
        }

        return 0;
    }

    public byte Izx()
    {
        ushort t = Read(Pc);
        Pc++;

        ushort lo = Read((ushort)((t + X) & 0x00FF));
        ushort hi = Read((ushort)((t + X + 1) & 0x00FF));

        AddrAbs = (ushort)((hi << 8) | lo);

        return 0;
    }

    public byte Izy()
    {
        ushort t = Read(Pc);
        Pc++;

        ushort lo = Read((ushort)(t & 0x00FF));
        ushort hi = Read((ushort)((t + 1) & 0x00FF));

        AddrAbs = (ushort)(((hi << 8) | lo) + Y);

        if ((AddrAbs & 0xFF00) != (hi << 8))
            return 1;

        return 0;
    }
    
    // Opcodes
    public byte Adc()
    {
        Fetch();
        ushort temp = (ushort)(A + Fetched + GetFlag(Flags6502.C));
        SetFlag(Flags6502.C, temp > 255);
        SetFlag(Flags6502.Z, (temp & 0x00FF) == 0);
        SetFlag(Flags6502.V, (~(A ^ Fetched) & (A ^ temp) & 0x0080) != 0);
        SetFlag(Flags6502.N, (temp & 0x80) != 0);

        A = (byte)(temp & 0x00FF);

        return 1;
    }

    public byte And()
    {
        Fetch();
        A = (byte)(A & Fetched);

        SetFlag(Flags6502.Z, A == 0x00);
        SetFlag(Flags6502.N, (A & 0x80) != 0);

        return 1;
    }

    public byte Asl()
    {
        Fetch();
        ushort temp = (ushort)(Fetched << 1);

        SetFlag(Flags6502.C, (temp & 0xFF00) != 0);
        SetFlag(Flags6502.Z, (temp & 0x00FF) == 0);
        SetFlag(Flags6502.N, (temp & 0x80) != 0);

        if (Lookup[Opcode].AddrMode == Imp)
            A = (byte)(temp & 0x00FF);
        else
            Write(AddrAbs, (byte)(temp & 0x00FF));

        return 0;
    }


    public byte Bcc()
    {
        if (GetFlag(Flags6502.C) == 0)
        {
            Cycles++;
            AddrAbs = (ushort)(Pc + AddRel);

            if ((AddrAbs & 0xFF00) != (Pc & 0xFF00))
            {
                Cycles++;
            }

            Pc = AddrAbs;
        }

        return 0;
    }

    public byte Bcs()
    {
        if (GetFlag(Flags6502.C) == 1)
        {
            Cycles++;
            AddrAbs = (ushort)(Pc + AddRel);

            if ((AddrAbs & 0xFF00) != (Pc & 0xFF00))
            {
                Cycles++;
            }

            Pc = AddrAbs;
        }

        return 0;
    }

    public byte Beq()
    {
        if (GetFlag(Flags6502.Z) == 1)
        {
            Cycles++;
            AddrAbs = (ushort)(Pc + AddRel);

            if ((AddrAbs & 0xFF00) != (Pc & 0xFF00))
            {
                Cycles++;
            }

            Pc = AddrAbs;
        }

        return 0;
    }

    public byte Bit()
    {
        Fetch();
        ushort temp = (ushort)(A & Fetched);

        SetFlag(Flags6502.Z, (temp & 0x00FF) == 0);
        SetFlag(Flags6502.N, (Fetched & (1 << 7)) != 0);
        SetFlag(Flags6502.V, (Fetched & (1 << 6)) != 0);

        return 0;
    }

    public byte Bmi()
    {
        if (GetFlag(Flags6502.N) == 1)
        {
            Cycles++;
            AddrAbs = (ushort)(Pc + AddRel);

            if ((AddrAbs & 0xFF00) != (Pc & 0xFF00))
            {
                Cycles++;
            }

            Pc = AddrAbs;
        }

        return 0;
    }

    public byte Bne()
    {
        if (GetFlag(Flags6502.Z) == 0)
        {
            Cycles++;
            AddrAbs = (ushort)(Pc + AddRel);

            if ((AddrAbs & 0xFF00) != (Pc & 0xFF00))
            {
                Cycles++;
            }

            Pc = AddrAbs;
        }

        return 0;
    }

    public byte Bpl()
    {
        if (GetFlag(Flags6502.N) == 0)
        {
            Cycles++;
            AddrAbs = (ushort)(Pc + AddRel);

            if ((AddrAbs & 0xFF00) != (Pc & 0xFF00))
            {
                Cycles++;
            }

            Pc = AddrAbs;
        }

        return 0;
    }

    public byte Brk()
    {
        Pc++;

        SetFlag(Flags6502.I, true);
        Write((ushort)(0x0100 + Stkp), (byte)((Pc >> 8) & 0x00FF));
        Stkp--;
        Write((ushort)(0x0100 + Stkp), (byte)(Pc & 0x00FF));
        Stkp--;

        SetFlag(Flags6502.B, true);
        Write((ushort)(0x0100 + Stkp), Status);
        Stkp--;
        SetFlag(Flags6502.B, false);

        Pc = (ushort)(Read(0xFFFE) | (Read(0xFFFF) << 8));

        return 0;
    }

    public byte Bvc()
    {
        if (GetFlag(Flags6502.V) == 0)
        {
            Cycles++;
            AddrAbs = (ushort)(Pc + AddRel);

            if ((AddrAbs & 0xFF00) != (Pc & 0xFF00))
            {
                Cycles++;
            }

            Pc = AddrAbs;
        }

        return 0;
    }

    public byte Bvs()
    {
        if (GetFlag(Flags6502.V) == 1)
        {
            Cycles++;
            AddrAbs = (ushort)(Pc + AddRel);

            if ((AddrAbs & 0xFF00) != (Pc & 0xFF00))
            {
                Cycles++;
            }

            Pc = AddrAbs;
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
        ushort temp = (ushort)(A - Fetched);

        SetFlag(Flags6502.C, A >= Fetched);
        SetFlag(Flags6502.Z, (temp & 0x00FF) == 0);
        SetFlag(Flags6502.N, (temp & 0x0080) != 0);

        return 1;
    }

    public byte Cpx()
    {
        Fetch();
        ushort temp = (ushort)(X - Fetched);

        SetFlag(Flags6502.C, X >= Fetched);
        SetFlag(Flags6502.Z, (temp & 0x00FF) == 0);
        SetFlag(Flags6502.N, (temp & 0x0080) != 0);

        return 0;
    }

    public byte Cpy()
    {
        Fetch();
        ushort temp = (ushort)(Y - Fetched);

        SetFlag(Flags6502.C, Y >= Fetched);
        SetFlag(Flags6502.Z, (temp & 0x00FF) == 0);
        SetFlag(Flags6502.N, (temp & 0x0080) != 0);

        return 0;
    }

    public byte Dec()
    {
        Fetch();
        ushort temp = (ushort)(Fetched - 1);
        Write(AddrAbs, (byte)(temp & 0x00FF));

        SetFlag(Flags6502.Z, (temp & 0x00FF) == 0);
        SetFlag(Flags6502.N, (temp & 0x0080) != 0);

        return 0;
    }

    public byte Dex()
    {
        X--;
        SetFlag(Flags6502.Z, X == 0x00);
        SetFlag(Flags6502.N, (X & 0x80) != 0);

        return 0;
    }

    public byte Dey()
    {
        Y--;
        SetFlag(Flags6502.Z, Y == 0x00);
        SetFlag(Flags6502.N, (Y & 0x80) != 0);

        return 0;
    }

    public byte Eor()
    {
        Fetch();
        A ^= Fetched;
        SetFlag(Flags6502.Z, A == 0x00);
        SetFlag(Flags6502.N, (A & 0x80) != 0);
        return 1;
    }

    public byte Inc()
    {
        Fetch();
        ushort temp = (ushort)(Fetched + 1);
        Write(AddrAbs, (byte)(temp & 0x00FF));

        SetFlag(Flags6502.Z, (temp & 0x00FF) == 0);
        SetFlag(Flags6502.N, (temp & 0x0080) != 0);

        return 0;
    }

    public byte Inx()
    {
        X++;
        SetFlag(Flags6502.Z, X == 0x00);
        SetFlag(Flags6502.N, (X & 0x80) != 0);
        return 0;
    }

    public byte Iny()
    {
        Y++;
        SetFlag(Flags6502.Z, Y == 0x00);
        SetFlag(Flags6502.N, (Y & 0x80) != 0);
        return 0;
    }

    public byte Jmp()
    {
        Pc = AddrAbs;
        return 0;
    }

    public byte Jsr()
    {
        Pc--; // Decrement PC before pushing onto the stack

        Write((ushort)(0x0100 + Stkp), (byte)((Pc >> 8) & 0x00FF));
        Stkp--;
        Write((ushort)(0x0100 + Stkp), (byte)(Pc & 0x00FF));
        Stkp--;

        Pc = AddrAbs;
        return 0;
    }

    public byte Lda()
    {
        Fetch();
        A = Fetched;
        SetFlag(Flags6502.Z, A == 0x00);
        SetFlag(Flags6502.N, (A & 0x80) != 0);
        // Console.WriteLine($"Cpu.LDA: {A}");
        return 1;
    }

    public byte Ldx()
    {
        Fetch();
        X = Fetched;
        SetFlag(Flags6502.Z, X == 0x00);
        SetFlag(Flags6502.N, (X & 0x80) != 0);
        return 1;
    }

    public byte Ldy()
    {
        Fetch();
        Y = Fetched;
        SetFlag(Flags6502.Z, Y == 0x00);
        SetFlag(Flags6502.N, (Y & 0x80) != 0);
        return 1;
    }

    public byte Lsr()
    {
        Fetch();
        SetFlag(Flags6502.C, (Fetched & 0x0001) != 0);
        ushort temp = (ushort)(Fetched >> 1);
        SetFlag(Flags6502.Z, (temp & 0x00FF) == 0x0000);
        SetFlag(Flags6502.N, (temp & 0x0080) != 0);

        if (Lookup[Opcode].AddrMode == Imp)
            A = (byte)(temp & 0x00FF);
        else
            Write(AddrAbs, (byte)(temp & 0x00FF));

        return 0;
    }

    public byte Nop()
    {
        // Некоторые NOP могут быть разными
        // Дополнительные операции основаны на https://wiki.nesdev.com/w/index.php/CPU_unofficial_opcodes
        // и будут добавляться для совместимости с играми
        // В идеале будет покрыта вся незаконная операция
        switch (Opcode)
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
        A = (byte)(A | Fetched);
        SetFlag(Flags6502.Z, A == 0x00);
        SetFlag(Flags6502.N, (A & 0x80) != 0);
        return 1;
    }

    public byte Pha()
    {
        Write((ushort)(0x0100 + Stkp), A);
        Stkp--;
        return 0;
    }

    public byte Php()
    {
        Write((ushort)(0x0100 + Stkp), (byte)(Status | (byte)Flags6502.B | (byte)Flags6502.U));
        SetFlag(Flags6502.B, false);
        SetFlag(Flags6502.U, false);
        Stkp--;
        return 0;
    }

    public byte Pla()
    {
        Stkp++;
        A = Read((ushort)(0x0100 + Stkp));
        SetFlag(Flags6502.Z, A == 0x00);
        SetFlag(Flags6502.N, (A & 0x80) != 0);
        return 0;
    }

    public byte Plp()
    {
        Stkp++;
        Status = Read((ushort)(0x0100 + Stkp));
        SetFlag(Flags6502.U, true);
        return 0;
    }

    public byte Rol()
    {
        Fetch();
        Temp = (ushort)((Fetched << 1) | GetFlag(Flags6502.C));
        SetFlag(Flags6502.C, (Temp & 0xFF00) != 0);
        SetFlag(Flags6502.Z, (Temp & 0x00FF) == 0x00);
        SetFlag(Flags6502.N, (Temp & 0x0080) != 0);

        if (Lookup[Opcode].AddrMode == Imp)
            A = (byte)(Temp & 0x00FF);
        else
            Write(AddrAbs, (byte)(Temp & 0x00FF));

        return 0;
    }

    public byte Ror()
    {
        Fetch();
        Temp = (ushort)((GetFlag(Flags6502.C) << 7) | (Fetched >> 1));
        SetFlag(Flags6502.C, (Fetched & 0x01) != 0);
        SetFlag(Flags6502.Z, (Temp & 0x00FF) == 0x00);
        SetFlag(Flags6502.N, (Temp & 0x0080) != 0);

        if (Lookup[Opcode].AddrMode == Imp)
            A = (byte)(Temp & 0x00FF);
        else
            Write(AddrAbs, (byte)(Temp & 0x00FF));

        return 0;
    }

    public byte Rti()
    {
        Stkp++;
        Status = Read((ushort)(0x0100 + Stkp));

        //TODO: check this if it is correct
        Status &= (byte)~(Flags6502.B | Flags6502.U);

        Stkp++;
        Pc = Read((ushort)(0x0100 + Stkp));
        Stkp++;
        Pc |= (ushort)(Read((ushort)(0x0100 + Stkp)) << 8);
        return 0;
    }

    public byte Rts()
    {
        Stkp++;
        Pc = Read((ushort)(0x0100 + Stkp));
        Stkp++;
        Pc |= (ushort)(Read((ushort)(0x0100 + Stkp)) << 8);

        Pc++;
        return 0;
    }

    public byte Sbc()
    {
        Fetch();

        ushort value = (ushort)(Fetched ^ 0x00FF);

        ushort temp = (ushort)(A + value + GetFlag(Flags6502.C));
        SetFlag(Flags6502.C, (temp & 0xFF00) != 0);
        SetFlag(Flags6502.Z, (temp & 0x00FF) == 0);
        SetFlag(Flags6502.V, ((temp ^ A) & (temp ^ value) & 0x0080) != 0);
        SetFlag(Flags6502.N, (temp & 0x0080) != 0);

        A = (byte)(temp & 0x00FF);

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
        Write(AddrAbs, A);
        return 0;
    }

    public byte Stx()
    {
        Write(AddrAbs, X);
        return 0;
    }

    public byte Sty()
    {
        Write(AddrAbs, Y);
        return 0;
    }

    public byte Tax()
    {
        X = A;
        SetFlag(Flags6502.Z, X == 0x00);
        SetFlag(Flags6502.N, (X & 0x80) != 0);
        return 0;
    }

    public byte Tay()
    {
        Y = A;
        SetFlag(Flags6502.Z, Y == 0x00);
        SetFlag(Flags6502.N, (Y & 0x80) != 0);
        return 0;
    }

    public byte Tsx()
    {
        X = Stkp;
        SetFlag(Flags6502.Z, X == 0x00);
        SetFlag(Flags6502.N, (X & 0x80) != 0);
        return 0;
    }

    public byte Txa()
    {
        A = X;
        SetFlag(Flags6502.Z, A == 0x00);
        SetFlag(Flags6502.N, (A & 0x80) != 0);
        return 0;
    }

    public byte Txs()
    {
        Stkp = X;
        return 0;
    }

    public byte Tya()
    {
        A = Y;
        SetFlag(Flags6502.Z, A == 0x00);
        SetFlag(Flags6502.N, (A & 0x80) != 0);
        return 0;
    }

    public byte Xxx()
    {
        return 0;
    }

    public void Clock()
    {
        if (Cycles == 0)
        {
            Opcode = Read(Pc);
            Pc++;

            Cycles = Lookup[Opcode].Cycles;

            // Console.WriteLine($"Executing [{Lookup[Opcode].Name}]");
            byte addCycle1 = Lookup[Opcode].AddrMode.Invoke();
            byte addCycle2 = Lookup[Opcode].Operate.Invoke();

            Cycles = (byte)(Cycles + (addCycle1 & addCycle2));
        }

        Cycles--;
    }

    public void Reset()
    {
        A = X = Y = 0;
        Stkp = 0xFD;
        Status = (byte)(0x00 | Flags6502.U);

        AddrAbs = 0xFFFC;
        ushort lo = Read((ushort)(AddrAbs + 0));
        ushort hi = Read((ushort)(AddrAbs + 1));

        Pc = (ushort)((hi << 8) | lo);
        
        AddRel = AddrAbs = 0x0000;
        Fetched = 0x00;

        Cycles = 8;
    }

    public void Irq()
    {
        if (GetFlag(Flags6502.I) == 0)
        {
            Write((ushort)(0x0100 + Stkp), (byte)((Pc >> 8) & 0x00FF));
            Stkp--;
            Write((ushort)(0x0100 + Stkp), (byte)(Pc & 0x00FF));
            Stkp--;

            SetFlag(Flags6502.B, false);
            SetFlag(Flags6502.U, true);
            SetFlag(Flags6502.I, true);

            Write((ushort)(0x0100 + Stkp), Status);
            Stkp--;

            AddrAbs = 0xFFFE;
            ushort lo = Read((ushort)(AddrAbs + 0));
            ushort hi = Read((ushort)(AddrAbs + 1));
            Pc = (ushort)((hi << 8) | lo);

            Cycles = 7;
        }
    }

    public void Nmi()
    {
        Write((ushort)(0x0100 + Stkp), (byte)((Pc >> 8) & 0x00FF));
        Stkp--;
        Write((ushort)(0x0100 + Stkp), (byte)(Pc & 0x00FF));
        Stkp--;

        SetFlag(Flags6502.B, false);
        SetFlag(Flags6502.U, true);
        SetFlag(Flags6502.I, true);

        Write((ushort)(0x0100 + Stkp), Status);
        Stkp--;

        AddrAbs = 0xFFFA;
        ushort lo = Read((ushort)(AddrAbs + 0));
        ushort hi = Read((ushort)(AddrAbs + 1));
        Pc = (ushort)((hi << 8) | lo);

        Cycles = 8;
    }
    
    public Dictionary<ushort, string> Disassemble(ushort nStart, ushort nStop)
    {
        ushort addr = nStart;
        byte value = 0x00, lo = 0x00, hi = 0x00;
        var mapLines = new Dictionary<ushort, string>();
        ushort lineAddr = 0;

        // Утилита для преобразования чисел в строку в шестнадцатеричной системе
        string Hex(uint n, byte d)
        {
            return n.ToString("X" + d);
        }

        while (addr <= (uint)nStop)
        {
            lineAddr = (ushort)addr;
            string sInst = "$" + Hex(addr, 4) + ": ";

            // Чтение инструкции
            byte opcode = _bus.CpuRead(addr, true); 
            addr++;
            sInst += Lookup[opcode].Name + " ";

            // Получение операндов в зависимости от режима адресации
            if (Lookup[opcode].AddrMode == Imp)
            {
                sInst += " {IMP}";
            }
            else if (Lookup[opcode].AddrMode == Imm)
            {
                value = _bus.CpuRead(addr, true); addr++;
                sInst += "#$" + Hex(value, 2) + " {IMM}";
            }
            else if (Lookup[opcode].AddrMode == Zp0)
            {
                lo = _bus.CpuRead(addr, true); addr++;
                hi = 0x00;
                sInst += "$" + Hex(lo, 2) + " {ZP0}";
            }
            else if (Lookup[opcode].AddrMode == Zpx)
            {
                lo = _bus.CpuRead(addr, true); addr++;
                hi = 0x00;
                sInst += "$" + Hex(lo, 2) + ", X {ZPX}";
            }
            else if (Lookup[opcode].AddrMode == Zpy)
            {
                lo = _bus.CpuRead(addr, true); addr++;
                hi = 0x00;
                sInst += "$" + Hex(lo, 2) + ", Y {ZPY}";
            }
            else if (Lookup[opcode].AddrMode == Izx)
            {
                lo = _bus.CpuRead(addr, true); addr++;
                hi = 0x00;
                sInst += "($" + Hex(lo, 2) + ", X) {IZX}";
            }
            else if (Lookup[opcode].AddrMode == Izy)
            {
                lo = _bus.CpuRead(addr, true); addr++;
                hi = 0x00;
                sInst += "($" + Hex(lo, 2) + "), Y {IZY}";
            }
            else if (Lookup[opcode].AddrMode == Abs)
            {
                lo = _bus.CpuRead(addr, true); addr++;
                hi = _bus.CpuRead(addr, true); addr++;
                sInst += "$" + Hex((ushort)((hi << 8) | lo), 4) + " {ABS}";
            }
            else if (Lookup[opcode].AddrMode == Abx)
            {
                lo = _bus.CpuRead(addr, true); addr++;
                hi = _bus.CpuRead(addr, true); addr++;
                sInst += "$" + Hex((ushort)((hi << 8) | lo), 4) + ", X {ABX}";
            }
            else if (Lookup[opcode].AddrMode == Aby)
            {
                lo = _bus.CpuRead(addr, true); addr++;
                hi = _bus.CpuRead(addr, true); addr++;
                sInst += "$" + Hex((ushort)((hi << 8) | lo), 4) + ", Y {ABY}";
            }
            else if (Lookup[opcode].AddrMode == Ind)
            {
                lo = _bus.CpuRead(addr, true); addr++;
                hi = _bus.CpuRead(addr, true); addr++;
                sInst += "($" + Hex((ushort)((hi << 8) | lo), 4) + ") {IND}";
            }
            else if (Lookup[opcode].AddrMode == Rel)
            {
                value = _bus.CpuRead(addr, true); addr++;
                sInst += "$" + Hex(value, 2) + " [$" + Hex((ushort)(addr + (sbyte)value), 4) + "] {REL}";
            }

            mapLines[lineAddr] = sInst;
        }

        return mapLines;
    }
}