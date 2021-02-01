using System;



namespace cpu6502
{
    /// <summary>
    /// Basic design largely stolen from https://github.com/gianlucag/mos6502
    /// </summary>
    public class CPU
    {
        private byte _a; // Accumulator
        private byte _x; // X Register
        private byte _y; // Y Register
        private byte _sp; // Stack Pointer
        private byte _sr; // Status Register
        private ushort _ip; // instruction Pointer


        private bool _flag_b; // breakpoint

        private bool _illegalOpcode = false;

        private byte[] _ram;

        public const ushort NMI_VECTOR = 0xFFFA;
        public const ushort RESET_VECTOR = 0xFFFC;
        public const ushort BREAK_IRQ_VECTOR = 0xFFFE;

        private Instruction[] _instructionTable = null;


        public CPU()
        {
            InitTable();
            Reset();
        }


        #region Direct Access

        public byte Accumulator
        {
            get => _a;
            set => _a = value;
        }

        public byte XRegister
        {
            get => _x;
            set => _x = value;
        }

        public byte YRegister
        {
            get => _y;
            set => _y = value;
        }

        public byte StackPointer
        {
            get => _sp;
            set => _sp = value;
        }

        public byte StatusRegister
        {
            get => _sr;
            set => _sr = value;
        }

        public ushort InstructionPointer
        {
            get => _ip;
            set => _ip = value;
        }

        public bool NegativeFlag => ((_sr & 0x80) == 0x80);
        public void SetNegative(bool bitVal)
        {
            if (bitVal)
            {
                _sr |= 0x80;
            }
            else
            {
                _sr = (byte)(_sr & ~0x80);
            }
        }
        public bool OverflowFlag => ((_sr & 0x40) == 0x40);
        public void SetOverflow(bool bitVal)
        {
            if (bitVal)
            {
                _sr |= 0x40;
            }
            else
            {
                _sr = (byte)(_sr & ~0x40);
            }
        }

        public bool ZeroFlag => ((_sr & 0x02) == 0x02);
        public void SetZero(bool bitVal)
        {
            if (bitVal)
            {
                _sr |= 0x02;
            }
            else
            {
                _sr = (byte)(_sr & ~0x02);
            }
        }

        public bool InterruptFlag => ((_sr & 0x04) == 0x04);
        public void SetInterrupt(bool bitVal)
        {
            if (bitVal)
            {
                _sr |= 0x04;
            }
            else
            {
                _sr = (byte)(_sr & ~0x04);
            }
        }


        public bool BreakpointFlag
        {
            get => _flag_b;
            set => _flag_b = value;
        }

        public bool DecimalFlag => ((_sr & 0x08) == 0x08);
        public void SetDecimal(bool bitVal)
        {
            if (bitVal)
            {
                _sr |= 0x08;
            }
            else
            {
                _sr = (byte)(_sr & ~0x08);
            }
        }

        public bool CarryFlag => ((_sr & 0x01) == 0x01);
        public void SetCarry(bool bitVal)
        {
            if (bitVal)
            {
                _sr |= 0x01;
            }
            else
            {
                _sr = (byte)(_sr & ~0x01);
            }
        }


        public byte[] RAM
        {
            get => _ram;
            set => _ram = value;
        }

        public bool IllegalOpCode => _illegalOpcode;

        #endregion

        public void LoadProgram(ushort addr, byte[] program)
        {
            foreach(byte data in program)
            {
                _ram[addr++] = data;
            }
        }

        public void Run(int maxCycles)
        {
            while (maxCycles > 0 && !IllegalOpCode)
            {
                maxCycles -= ExecuteNextInstruction();
            }
        }
        public Instruction GetInstruction(byte opCode)
        {
            return _instructionTable[opCode];
        }

        public int ExecuteNextInstruction()
        {
            int nCycles = 0;
            Instruction instruction = GetNextInstruction();
            if (instruction.Instr == 0x00)
            {
                throw new CPUBreakException();
            }
            instruction.Execute();
            return nCycles;
        }

        public Instruction GetNextInstruction()
        {
            return GetInstruction(IncrementInstructionPointer());
        }


        #region AddressModes

        public ushort AddressMode_ACC()
        {
            return 0;
        }

        public ushort AddressMode_IMM()
        {
            return _ip++;
        }

        public ushort AddressMode_ABS()
        {
            ushort addr = ReadAddress(_ip);
            _ip += 2;
            return addr;
        }

        public ushort AddressMode_ZER()
        {
            return ReadByte(_ip++);
        }

        public ushort AddressMode_IMP()
        {
            return 0;
        }

        public ushort AddressMode_REL()
        {
            ushort offset;
            offset = ReadByte(_ip++);
            if ((offset & 0x80) > 0)
            {
                offset |= 0xFF00;
            }
            return (ushort)(_ip + offset);
        }

        public ushort AddressMode_ABI()
        {
            byte lo = ReadByte(_ip++);
            byte hi = ReadByte(_ip++);
            ushort addr = (ushort)(lo + hi * 256);
            lo = ReadByte(addr);

#if CMOS_INDRECT_JMP_FIX
            hi = ReadByte((ushort)(addr + 1));
#else
            // Bug in original 6502
            hi = ReadByte((ushort)((addr & 0xFF00) + ((addr + 1) & 0xFF)));
#endif

            return (ushort)(lo + hi * 256);
        }

        public ushort AddressMode_ZEX()
        {
            byte val = ReadByte(_ip++);
            return (ushort) ((val + _x) % 256);
        }

        public ushort AddressMode_ZEY()
        {
            byte val = ReadByte(_ip++);
            return (ushort)((val + _y) % 256);
        }

        public ushort AddressMode_ABX()
        {
            byte lo = ReadByte(_ip++);
            byte hi = ReadByte(_ip++);
            ushort addr = (ushort)(lo + hi * 256);
            return (ushort)(addr + _x);
        }

        public ushort AddressMode_ABY()
        {
            byte lo = ReadByte(_ip++);
            byte hi = ReadByte(_ip++);
            ushort addr = (ushort)(lo + hi * 256);
            return (ushort)(addr + _y);
        }

        public ushort AddressMode_INX()
        {
            byte lo = (byte) ((ReadByte(_ip++) + _x) % 256);
            byte hi = (byte) ((lo + 1) % 256);
            ushort addr = (ushort)(lo + hi * 256);
            return addr;
        }

        public ushort AddressMode_INY()
        {
            byte lo = ReadByte(_ip++);
            byte hi = (byte)((lo + 1) % 256);
            ushort addr = (ushort)(lo + hi * 256 + _y);
            return addr;
        }

        #endregion


        #region OpCodes

        public void OpCode_ILLEGAL(ushort data)
        {
            _illegalOpcode = true;
        }

        public void OpCode_ADC(ushort data)
        {
            byte val = ReadByte(data);
            byte carry = CarryFlag ? 1 : 0;
            ushort tmp = (ushort) (_a + val + carry);
            SetZero((tmp & 0xFF) == 0);
            if (DecimalFlag)
            {
                if (((_a & 0xF) + (val & 0xF) + carry) > 9)
                {
                    tmp += 6;
                }
                SetNegative((tmp & 0x80) > 0);
                SetOverflow((!(((_a ^ val) & 0x80) > 0) && (((_a ^ tmp) & 0x80) > 0)));
                if (tmp > 0x99)
                {
                    tmp += 96;
                }
                SetCarry(tmp > 0x99);
            }
            else
            {
                SetNegative((tmp & 0x80) > 0);
                SetOverflow((!(((_a ^ val) & 0x80) > 0) && (((_a ^ tmp) & 0x80) > 0)));
                SetCarry(tmp > 0xFF);
            }
            _a = (byte) (tmp & 0xFF);
        }

        public void OpCode_AND(ushort data)
        {
            byte val = (byte) (ReadByte(data) & _a);
            SetNegative((val & 0x80) > 0);
            SetZero(val == 0);
            _a = val;
        }

        public void OpCode_ASL(ushort data)
        {
            byte val = ReadByte(data);
            SetCarry((val & 0x80) > 0);
            val <<= 1;
            val &= 0xFF;
            SetNegative((val & 0x80) > 0);
            SetZero(val == 0);
            WriteByte(data, val);
        }

        public void OpCode_ASL_ACC(ushort data)
        {
            byte val = ReadByte(data);
            SetCarry((val & 0x80) > 0);
            val <<= 1;
            val &= 0xFF;
            SetNegative((val & 0x80) > 0);
            SetZero(val == 0);
            _a = val;
        }

        public void OpCode_BCC(ushort data)
        {
            if (!CarryFlag)
            {
                _ip = data;
            }
        }

        public void OpCode_BCS(ushort data)
        {
            if (CarryFlag)
            {
                _ip = data;
            }
        }

        public void OpCode_BEQ(ushort data)
        {
            if (ZeroFlag)
            {
                _ip = data;
            }
        }
        public void OpCode_BMI(ushort data)
        {
            if (NegativeFlag)
            {
                _ip = data;
            }
        }
        public void OpCode_BNE(ushort data)
        {
            if (!ZeroFlag)
            {
                _ip = data;
            }
        }
        public void OpCode_BPL(ushort data)
        {
            if (!NegativeFlag)
            {
                _ip = data;
            }
        }
        public void OpCode_BVC(ushort data)
        {
            if (!OverflowFlag)
            {
                _ip = data;
            }    
        }
        public void OpCode_BVS(ushort data)
        {
            if (OverflowFlag)
            {
                _ip = data;
            }
        }
        public void OpCode_BIT(ushort data)
        {
            byte val = ReadByte(data);
            byte res = (byte) (_a & val);
            StatusRegister = (byte) ((_sr & 0x3F) | (byte)(val & 0xC0));
            SetZero(res == 0);
        }
        public void OpCode_BRK(ushort data)
        {
            _ip++;
            StackPush((byte) ((_ip >> 8) & 0xFF));
            StackPush((byte)(_ip & 0xFF));
            SetInterrupt(true);
            _ip = ReadAddress(BREAK_IRQ_VECTOR);
            return;
        }
        public void OpCode_CLC(ushort data)
        {
            SetCarry(false);
        }
        public void OpCode_CLD(ushort data)
        {
            SetDecimal(false);
        }
        public void OpCode_CLI(ushort data)
        {
            SetInterrupt(false);
        }
        public void OpCode_CLV(ushort data)
        {
            SetOverflow(false);
        }
        public void OpCode_CMP(ushort data)
        {
            ushort res = (ushort) (_a - ReadByte(data));
            SetCarry(res < 0x100);
            SetNegative((res & 0x80) > 0);
            SetZero(res == 0);
        }

        public void OpCode_CPX(ushort data)
        {
            ushort res = (ushort)(_x - ReadByte(data));
            SetCarry(res < 0x100);
            SetNegative((res & 0x80) > 0);
            SetZero(res == 0);
        }
        public void OpCode_CPY(ushort data)
        {
            ushort res = (ushort)(_y - ReadByte(data));
            SetCarry(res < 0x100);
            SetNegative((res & 0x80) > 0);
            SetZero(res == 0);
        }
        public void OpCode_DEC(ushort data)
        {
            byte val = ReadByte(data);
            val = (byte)((val - 1) % 256);
            SetNegative((val & 0x80) > 0);
            SetZero(val == 0);
            WriteByte(data, val);
        }
        public void OpCode_DEX(ushort data)
        {
            byte val = _x;
            val = (byte)((val - 1) % 256);
            SetNegative((val & 0x80) > 0);
            SetZero(val == 0);
            _x = val;
        }

        public void OpCode_DEY(ushort data)
        {
            byte val = _y;
            val = (byte)((val - 1) % 256);
            SetNegative((val & 0x80) > 0);
            SetZero(val == 0);
            _y = val;
        }

        public void OpCode_EOR(ushort data)
        {
            byte val = ReadByte(data);
            val = (byte) (_a ^ val);
            SetNegative((val & 0x80) > 0);
            SetZero(val == 0);
            _a = val;
        }

        public void OpCode_INC(ushort data)
        {
            byte val = ReadByte(data);
            val = (byte)((val + 1) % 256);
            SetNegative((val & 0x80) > 0);
            SetZero(val == 0);
            WriteByte(data, val);
        }

        public void OpCode_INX(ushort data)
        {
            byte val = _x;
            val = (byte)((val + 1) % 256);
            SetNegative((val & 0x80) > 0);
            SetZero(val == 0);
            _x = val;
        }

        public void OpCode_INY(ushort data)
        {
            byte val = _y;
            val = (byte)((val + 1) % 256);
            SetNegative((val & 0x80) > 0);
            SetZero(val == 0);
            _y = val;
        }

        public void OpCode_JMP(ushort data)
        {
            _ip = data;
        }

        public void OpCode_JSR(ushort data)
        {
            _ip--;
            StackPush((byte) ((_ip >> 8) & 0xFF));
            StackPush((byte)(_ip & 0xFF));
            _ip = data; ;
        }

        public void OpCode_LDA(ushort data)
        {
            byte val = ReadByte(data);
            SetNegative((val & 0x80) > 0);
            SetZero(val == 0);
            _a = val;
        }

        public void OpCode_LDX(ushort data)
        {
            byte val = ReadByte(data);
            SetNegative((val & 0x80) > 0);
            SetZero(val == 0);
            _x = val;
        }

        public void OpCode_LDY(ushort data)
        {
            byte val = ReadByte(data);
            SetNegative((val & 0x80) > 0);
            SetZero(val == 0);
            _y = val;
        }

        public void OpCode_LSR(ushort data)
        {
            byte val = ReadByte(data);
            SetCarry((val & 0x01) > 0);
            val >>= 1;
            SetNegative(false);
            SetZero(val == 0);
            WriteByte(data, val);
        }

        public void OpCode_LSR_ACC(ushort data)
        {
            byte val = ReadByte(data);
            SetCarry((val & 0x01) > 0);
            val >>= 1;
            SetNegative(false);
            SetZero(val == 0);
            _a = val;
        }

        public void OpCode_NOP(ushort data)
        {
            
        }

        public void OpCode_ORA(ushort data)
        {
            byte val = ReadByte(data);
            val = (byte) (_a | val);
            SetNegative((val & 0x80) > 0);
            SetZero(val == 0);
            _a = val;
        }

        public void OpCode_PHA(ushort data)
        {
            StackPush(_a);
        }

        public void OpCode_PHP(ushort data)
        {
            StackPush(StatusRegister);
        }

        public void OpCode_PLA(ushort data)
        {
            _a = StackPop();
            SetNegative((_a & 0x80) > 0);
            SetZero(_a == 0);
        }

        public void OpCode_PLP(ushort data)
        {
            StatusRegister = StackPop();
        }

        public void OpCode_ROL(ushort data)
        {
            ushort val = ReadByte(data);
            val <<= 1;
            if (CarryFlag)
            {
                val |= 0x01;
            }
            SetCarry(val > 0xFF);
            val &= 0xFF;
            SetNegative((val & 0x80) > 0);
            SetZero(val == 0);
            WriteByte(data, (byte)val);
        }

        public void OpCode_ROL_ACC(ushort data)
        {
            ushort val = _a;
            val <<= 1;
            if (CarryFlag)
            {
                val |= 0x01;
            }
            SetCarry(val > 0xFF);
            val &= 0xFF;
            SetNegative((val & 0x80) > 0);
            SetZero(val == 0);
            _a = (byte)val;
        }

        public void OpCode_ROR(ushort data)
        {
            ushort val = ReadByte(data);
            if (CarryFlag)
            {
                val |= 0x100;
            }
            SetCarry(val > 0x01);
            val >>= 1;
            val &= 0xFF;
            SetNegative((val & 0x80) > 0);
            SetZero(val == 0);
            WriteByte(data, (byte)val);
        }

        public void OpCode_ROR_ACC(ushort data)
        {
            ushort val = _a;
            if (CarryFlag)
            {
                val |= 0x100;
            }
            SetCarry(val > 0x01);
            val >>= 1;
            val &= 0xFF;
            SetNegative((val & 0x80) > 0);
            SetZero(val == 0);
            _a = (byte)val;
        }

        public void OpCode_RTI(ushort data)
        {
            StatusRegister = StackPop();
            byte lo = StackPop();
            byte hi = StackPop();
            _ip = (ushort) (hi * 256 + lo);
        }

        public void OpCode_RTS(ushort data)
        {
            byte lo = StackPop();
            byte hi = StackPop();
            _ip = (ushort)(hi * 256 + lo + 1);
        }

        public void OpCode_SBC(ushort data)
        {
            byte carry = CarryFlag ? 1 : 0;
            byte val = ReadByte(data);
            ushort tmp = (ushort) (_a - val - carry);
            SetNegative((tmp & 0x80) > 0);
            SetZero(tmp == 0);
            SetOverflow((((_a ^ tmp) & 0x80) > 0) && (((_a ^ val) & 0x80) > 0));
            if (DecimalFlag)
            {
                if (((_a & 0xF) - carry) < (val & 0xF))
                {
                    tmp -= 6;
                }
                if (tmp > 0x99)
                {
                    tmp -= 0x60;
                }
            }
            SetCarry(tmp < 0x100);
            _a = (byte) (tmp & 0xFF);
        }

        public void OpCode_SEC(ushort data)
        {
            SetCarry(true);
        }

        public void OpCode_SED(ushort data)
        {
            SetDecimal(true);
        }

        public void OpCode_SEI(ushort data)
        {
            SetInterrupt(true);
        }

        public void OpCode_STA(ushort data)
        {
            WriteByte(data, _a);
        }

        public void OpCode_STX(ushort data)
        {
            WriteByte(data, _x);
        }

        public void OpCode_STY(ushort data)
        {
            WriteByte(data, _y);
        }

        public void OpCode_TAX(ushort data)
        {
            byte val = _a;
            SetNegative((val & 0x80) > 0);
            SetZero(val == 0);
            _x = val;
        }

        public void OpCode_TAY(ushort data)
        {
            byte val = _a;
            SetNegative((val & 0x80) > 0);
            SetZero(val == 0);
            _y = val;
        }

        public void OpCode_TSX(ushort data)
        {
            byte val = _sp;
            SetNegative((val & 0x80) > 0);
            SetZero(val == 0);
            _x = val;
        }

        public void OpCode_TXA(ushort data)
        {
            byte val = _x;
            SetNegative((val & 0x80) > 0);
            SetZero(val == 0);
            _a = val;
        }

        public void OpCode_TXS(ushort data)
        {
            byte val = _x;
            SetNegative((val & 0x80) > 0);
            SetZero(val == 0);
            _sp = val;
        }

        public void OpCode_TYA(ushort data)
        {
            byte val = _y;
            SetNegative((val & 0x80) > 0);
            SetZero(val == 0);
            _a = val;
        }

        #endregion


        #region Private Methods

        /// <summary>
        /// Made into a function so we can potentially add events 
        /// </summary>
        private byte IncrementInstructionPointer()
        {
            byte ret = _ram[_ip];
            _ip++;
            return ret;
        }

        private void Reset()
        {
            _a = 0;
            _x = 0;
            _y = 0;
            _ip = RESET_VECTOR;
            _sp = 0xFD;
            _illegalOpcode = false;
        }

        private ushort ReadAddress(ushort twoBytes)
        {
            byte low = ReadByte(twoBytes++);
            byte high = ReadByte(twoBytes);
            return (ushort)(low + (high * 256));
        }

        private byte ReadByte(ushort addr)
        {
            return _ram[addr];
        }

        private void WriteByte(ushort addr, byte data)
        {
            _ram[addr] = data;
        }

        private void StackPush(byte val)
        {
            WriteByte((ushort) (_sp + 0x100), val);
            if (_sp == 0)
            {
                _sp = 0xFF;
            }
            else
            {
                _sp--;
            }
        }

        private byte StackPop()
        {
            if (_sp == 0xFF)
            {
                _sp = 0x00;
            }
            else
            {
                _sp++;
            }
            return ReadByte((ushort) (_sp + 0x100));
        }

        private void InitTable()
        {
            _instructionTable = new Instruction[256];
            // fill jump table with ILLEGALs
            for (int i = 0; i < 256; i++)
            {
                _instructionTable[i] = new Instruction((byte) i , this.AddressMode_IMP, OpCode_ILLEGAL, 0);
            }

            // insert opcodes
            _instructionTable[0x69] = new Instruction(0x69, AddressMode_IMM, OpCode_ADC, 2);
            _instructionTable[0x6D] = new Instruction(0x6D, AddressMode_ABS, OpCode_ADC, 4);
            _instructionTable[0x65] = new Instruction(0x65, AddressMode_ZER, OpCode_ADC, 3);
            _instructionTable[0x61] = new Instruction(0x61, AddressMode_INX, OpCode_ADC, 6);
            _instructionTable[0x71] = new Instruction(0x71, AddressMode_INY, OpCode_ADC, 6);
            _instructionTable[0x75] = new Instruction(0x75, AddressMode_ZEX, OpCode_ADC, 4);
            _instructionTable[0x7D] = new Instruction(0x7D, AddressMode_ABX, OpCode_ADC, 4);
            _instructionTable[0x79] = new Instruction(0x79, AddressMode_ABY, OpCode_ADC, 4);
            _instructionTable[0x29] = new Instruction(0x29, AddressMode_IMM, OpCode_AND, 2);
            _instructionTable[0x2D] = new Instruction(0x2D, AddressMode_ABS, OpCode_AND, 4);
            _instructionTable[0x25] = new Instruction(0x25, AddressMode_ZER, OpCode_AND, 3);
            _instructionTable[0x21] = new Instruction(0x21, AddressMode_INX, OpCode_AND, 6);
            _instructionTable[0x31] = new Instruction(0x31, AddressMode_INY, OpCode_AND, 5);
            _instructionTable[0x35] = new Instruction(0x35, AddressMode_ZEX, OpCode_AND, 4);
            _instructionTable[0x3D] = new Instruction(0x3D, AddressMode_ABX, OpCode_AND, 4);
            _instructionTable[0x39] = new Instruction(0x39, AddressMode_ABY, OpCode_AND, 4);
            _instructionTable[0x0E] = new Instruction(0x0E, AddressMode_ABS, OpCode_ASL, 6);
            _instructionTable[0x06] = new Instruction(0x06, AddressMode_ZER, OpCode_ASL, 5);
            _instructionTable[0x0A] = new Instruction(0x0A, AddressMode_ACC, OpCode_ASL_ACC, 2);
            _instructionTable[0x16] = new Instruction(0x16, AddressMode_ZEX, OpCode_ASL, 6);
            _instructionTable[0x1E] = new Instruction(0x1E, AddressMode_ABX, OpCode_ASL, 7);
            _instructionTable[0x90] = new Instruction(0x90, AddressMode_REL, OpCode_BCC, 2);
            _instructionTable[0xB0] = new Instruction(0xB0, AddressMode_REL, OpCode_BCS, 2);
            _instructionTable[0xF0] = new Instruction(0xF0, AddressMode_REL, OpCode_BEQ, 2);
            _instructionTable[0x30] = new Instruction(0x30, AddressMode_REL, OpCode_BMI, 2);
            _instructionTable[0xD0] = new Instruction(0xD0, AddressMode_REL, OpCode_BNE, 2);
            _instructionTable[0x10] = new Instruction(0x10, AddressMode_REL, OpCode_BPL, 2);
            _instructionTable[0x50] = new Instruction(0x50, AddressMode_REL, OpCode_BVC, 2);
            _instructionTable[0x70] = new Instruction(0x70, AddressMode_REL, OpCode_BVS, 2);
            _instructionTable[0x2C] = new Instruction(0x2C, AddressMode_ABS, OpCode_BIT, 4);
            _instructionTable[0x24] = new Instruction(0x24, AddressMode_ZER, OpCode_BIT, 3);
            _instructionTable[0x00] = new Instruction(0x00, AddressMode_REL, OpCode_BRK, 7);
            _instructionTable[0x18] = new Instruction(0x18, AddressMode_IMP, OpCode_CLC, 2);
            _instructionTable[0xD8] = new Instruction(0xD8, AddressMode_IMP, OpCode_CLD, 2);
            _instructionTable[0x58] = new Instruction(0x58, AddressMode_IMP, OpCode_CLI, 2);
            _instructionTable[0xB8] = new Instruction(0xB8, AddressMode_IMP, OpCode_CLV, 2);
            _instructionTable[0xC9] = new Instruction(0xC9, AddressMode_IMM, OpCode_CMP, 2);
            _instructionTable[0xCD] = new Instruction(0xCD, AddressMode_ABS, OpCode_CMP, 4);
            _instructionTable[0xC5] = new Instruction(0xC5, AddressMode_ZER, OpCode_CMP, 3);
            _instructionTable[0xC1] = new Instruction(0xC1, AddressMode_INX, OpCode_CMP, 6);
            _instructionTable[0xD1] = new Instruction(0xD1, AddressMode_INY, OpCode_CMP, 3);
            _instructionTable[0xD5] = new Instruction(0xD5, AddressMode_ZEX, OpCode_CMP, 4);
            _instructionTable[0xDD] = new Instruction(0xDD, AddressMode_ABX, OpCode_CMP, 4);
            _instructionTable[0xD9] = new Instruction(0xD9, AddressMode_ABY, OpCode_CMP, 4);
            _instructionTable[0xE0] = new Instruction(0xE0, AddressMode_IMM, OpCode_CPX, 2);
            _instructionTable[0xEC] = new Instruction(0xEC, AddressMode_ABS, OpCode_CPX, 4);
            _instructionTable[0xE4] = new Instruction(0xE4, AddressMode_ZER, OpCode_CPX, 3);
            _instructionTable[0xC0] = new Instruction(0xC0, AddressMode_IMM, OpCode_CPY, 2);
            _instructionTable[0xCC] = new Instruction(0xCC, AddressMode_ABS, OpCode_CPY, 4);
            _instructionTable[0xC4] = new Instruction(0xC4, AddressMode_ZER, OpCode_CPY, 3);
            _instructionTable[0xCE] = new Instruction(0xCE, AddressMode_ABS, OpCode_DEC, 6);
            _instructionTable[0xC6] = new Instruction(0xC6, AddressMode_ZER, OpCode_DEC, 5);
            _instructionTable[0xD6] = new Instruction(0xD6, AddressMode_ZEX, OpCode_DEC, 6);
            _instructionTable[0xDE] = new Instruction(0xDE, AddressMode_ABX, OpCode_DEC, 7);
            _instructionTable[0xCA] = new Instruction(0xCA, AddressMode_IMP, OpCode_DEX, 2);
            _instructionTable[0x88] = new Instruction(0x88, AddressMode_IMP, OpCode_DEY, 2);
            _instructionTable[0x49] = new Instruction(0x49, AddressMode_IMM, OpCode_EOR, 2);
            _instructionTable[0x4D] = new Instruction(0x4D, AddressMode_ABS, OpCode_EOR, 4);
            _instructionTable[0x45] = new Instruction(0x45, AddressMode_ZER, OpCode_EOR, 3);
            _instructionTable[0x41] = new Instruction(0x41, AddressMode_INX, OpCode_EOR, 6);
            _instructionTable[0x51] = new Instruction(0x51, AddressMode_INY, OpCode_EOR, 5);
            _instructionTable[0x55] = new Instruction(0x55, AddressMode_ZEX, OpCode_EOR, 4);
            _instructionTable[0x5D] = new Instruction(0x5D, AddressMode_ABX, OpCode_EOR, 4);
            _instructionTable[0x59] = new Instruction(0x59, AddressMode_ABY, OpCode_EOR, 4);
            _instructionTable[0xEE] = new Instruction(0xEE, AddressMode_ABS, OpCode_INC, 6);
            _instructionTable[0xE6] = new Instruction(0xE6, AddressMode_ZER, OpCode_INC, 5);
            _instructionTable[0xF6] = new Instruction(0xF6, AddressMode_ZEX, OpCode_INC, 6);
            _instructionTable[0xFE] = new Instruction(0xFE, AddressMode_ABX, OpCode_INC, 7);
            _instructionTable[0xE8] = new Instruction(0xE8, AddressMode_IMP, OpCode_INX, 2);
            _instructionTable[0xC8] = new Instruction(0xC8, AddressMode_IMP, OpCode_INY, 2);
            _instructionTable[0x4C] = new Instruction(0x4C, AddressMode_ABS, OpCode_JMP, 3);
            _instructionTable[0x6C] = new Instruction(0x6C, AddressMode_ABI, OpCode_JMP, 5);
            _instructionTable[0x20] = new Instruction(0x20, AddressMode_ABS, OpCode_JSR, 6);
            _instructionTable[0xA9] = new Instruction(0xA9, AddressMode_IMM, OpCode_LDA, 2);
            _instructionTable[0xAD] = new Instruction(0xAD, AddressMode_ABS, OpCode_LDA, 4);
            _instructionTable[0xA5] = new Instruction(0xA5, AddressMode_ZER, OpCode_LDA, 3);
            _instructionTable[0xA1] = new Instruction(0xA1, AddressMode_INX, OpCode_LDA, 6);
            _instructionTable[0xB1] = new Instruction(0xB1, AddressMode_INY, OpCode_LDA, 5);
            _instructionTable[0xB5] = new Instruction(0xB5, AddressMode_ZEX, OpCode_LDA, 4);
            _instructionTable[0xBD] = new Instruction(0xBD, AddressMode_ABX, OpCode_LDA, 4);
            _instructionTable[0xB9] = new Instruction(0xB9, AddressMode_ABY, OpCode_LDA, 4);
            _instructionTable[0xA2] = new Instruction(0xA2, AddressMode_IMM, OpCode_LDX, 2);
            _instructionTable[0xAE] = new Instruction(0xAE, AddressMode_ABS, OpCode_LDX, 4);
            _instructionTable[0xA6] = new Instruction(0xA6, AddressMode_ZER, OpCode_LDX, 3);
            _instructionTable[0xBE] = new Instruction(0xBE, AddressMode_ABY, OpCode_LDX, 4);
            _instructionTable[0xB6] = new Instruction(0xB6, AddressMode_ZEY, OpCode_LDX, 4);
            _instructionTable[0xA0] = new Instruction(0xA0, AddressMode_IMM, OpCode_LDY, 2);
            _instructionTable[0xAC] = new Instruction(0xAC, AddressMode_ABS, OpCode_LDY, 4);
            _instructionTable[0xA4] = new Instruction(0xA4, AddressMode_ZER, OpCode_LDY, 3);
            _instructionTable[0xB4] = new Instruction(0xB4, AddressMode_ZEX, OpCode_LDY, 4);
            _instructionTable[0xBC] = new Instruction(0xBC, AddressMode_ABX, OpCode_LDY, 4);
            _instructionTable[0x4E] = new Instruction(0x4E, AddressMode_ABS, OpCode_LSR, 6);
            _instructionTable[0x46] = new Instruction(0x46, AddressMode_ZER, OpCode_LSR, 5);
            _instructionTable[0x4A] = new Instruction(0x4A, AddressMode_ACC, OpCode_LSR_ACC, 2);
            _instructionTable[0x56] = new Instruction(0x56, AddressMode_ZEX, OpCode_LSR, 6);
            _instructionTable[0x5E] = new Instruction(0x5E, AddressMode_ABX, OpCode_LSR, 7);
            _instructionTable[0xEA] = new Instruction(0xEA, AddressMode_IMP, OpCode_NOP, 2);
            _instructionTable[0x09] = new Instruction(0x09, AddressMode_IMM, OpCode_ORA, 2);
            _instructionTable[0x0D] = new Instruction(0x0D, AddressMode_ABS, OpCode_ORA, 4);
            _instructionTable[0x05] = new Instruction(0x05, AddressMode_ZER, OpCode_ORA, 3);
            _instructionTable[0x01] = new Instruction(0x01, AddressMode_INX, OpCode_ORA, 6);
            _instructionTable[0x11] = new Instruction(0x11, AddressMode_INY, OpCode_ORA, 5);
            _instructionTable[0x15] = new Instruction(0x15, AddressMode_ZEX, OpCode_ORA, 4);
            _instructionTable[0x1D] = new Instruction(0x1D, AddressMode_ABX, OpCode_ORA, 4);
            _instructionTable[0x19] = new Instruction(0x19, AddressMode_ABY, OpCode_ORA, 4);
            _instructionTable[0x48] = new Instruction(0x48, AddressMode_IMP, OpCode_PHA, 3);
            _instructionTable[0x08] = new Instruction(0x08, AddressMode_IMP, OpCode_PHP, 3);
            _instructionTable[0x68] = new Instruction(0x68, AddressMode_IMP, OpCode_PLA, 4);
            _instructionTable[0x28] = new Instruction(0x28, AddressMode_IMP, OpCode_PLP, 4);
            _instructionTable[0x2E] = new Instruction(0x2E, AddressMode_ABS, OpCode_ROL, 6);
            _instructionTable[0x26] = new Instruction(0x26, AddressMode_ZER, OpCode_ROL, 5);
            _instructionTable[0x2A] = new Instruction(0x2A, AddressMode_ACC, OpCode_ROL_ACC, 2);
            _instructionTable[0x36] = new Instruction(0x36, AddressMode_ZEX, OpCode_ROL, 6);
            _instructionTable[0x3E] = new Instruction(0x3E, AddressMode_ABX, OpCode_ROL, 7);
            _instructionTable[0x6E] = new Instruction(0x6E, AddressMode_ABS, OpCode_ROR, 6);
            _instructionTable[0x66] = new Instruction(0x66, AddressMode_ZER, OpCode_ROR, 5);
            _instructionTable[0x6A] = new Instruction(0x6A, AddressMode_ACC, OpCode_ROR_ACC, 2);
            _instructionTable[0x76] = new Instruction(0x76, AddressMode_ZEX, OpCode_ROR, 6);
            _instructionTable[0x7E] = new Instruction(0x7E, AddressMode_ABX, OpCode_ROR, 7);
            _instructionTable[0x40] = new Instruction(0x40, AddressMode_IMP, OpCode_RTI, 6);
            _instructionTable[0x60] = new Instruction(0x60, AddressMode_IMP, OpCode_RTS, 6);
            _instructionTable[0xE9] = new Instruction(0xE9, AddressMode_IMM, OpCode_SBC, 2);
            _instructionTable[0xED] = new Instruction(0xED, AddressMode_ABS, OpCode_SBC, 4);
            _instructionTable[0xE5] = new Instruction(0xE5, AddressMode_ZER, OpCode_SBC, 3);
            _instructionTable[0xE1] = new Instruction(0xE1, AddressMode_INX, OpCode_SBC, 6);
            _instructionTable[0xF1] = new Instruction(0xF1, AddressMode_INY, OpCode_SBC, 5);
            _instructionTable[0xF5] = new Instruction(0xF5, AddressMode_ZEX, OpCode_SBC, 4);
            _instructionTable[0xFD] = new Instruction(0xFD, AddressMode_ABX, OpCode_SBC, 4);
            _instructionTable[0xF9] = new Instruction(0xF9, AddressMode_ABY, OpCode_SBC, 4);
            _instructionTable[0x38] = new Instruction(0x38, AddressMode_IMP, OpCode_SEC, 2);
            _instructionTable[0xF8] = new Instruction(0xF8, AddressMode_IMP, OpCode_SED, 2);
            _instructionTable[0x78] = new Instruction(0x78, AddressMode_IMP, OpCode_SEI, 2);
            _instructionTable[0x8D] = new Instruction(0x8D, AddressMode_ABS, OpCode_STA, 4);
            _instructionTable[0x85] = new Instruction(0x85, AddressMode_ZER, OpCode_STA, 3);
            _instructionTable[0x81] = new Instruction(0x81, AddressMode_INX, OpCode_STA, 6);
            _instructionTable[0x91] = new Instruction(0x91, AddressMode_INY, OpCode_STA, 6);
            _instructionTable[0x95] = new Instruction(0x95, AddressMode_ZEX, OpCode_STA, 4);
            _instructionTable[0x9D] = new Instruction(0x9D, AddressMode_ABX, OpCode_STA, 5);
            _instructionTable[0x99] = new Instruction(0x99, AddressMode_ABY, OpCode_STA, 5);
            _instructionTable[0x8E] = new Instruction(0x8E, AddressMode_ABS, OpCode_STX, 4);
            _instructionTable[0x86] = new Instruction(0x86, AddressMode_ZER, OpCode_STX, 3);
            _instructionTable[0x96] = new Instruction(0x96, AddressMode_ZEY, OpCode_STX, 4);
            _instructionTable[0x8C] = new Instruction(0x8C, AddressMode_ABS, OpCode_STY, 4);
            _instructionTable[0x84] = new Instruction(0x84, AddressMode_ZER, OpCode_STY, 3);
            _instructionTable[0x94] = new Instruction(0x94, AddressMode_ZEX, OpCode_STY, 4);
            _instructionTable[0xAA] = new Instruction(0xAA, AddressMode_IMP, OpCode_TAX, 2);
            _instructionTable[0xA8] = new Instruction(0xA8, AddressMode_IMP, OpCode_TAY, 2);
            _instructionTable[0xBA] = new Instruction(0xBA, AddressMode_IMP, OpCode_TSX, 2);
            _instructionTable[0x8A] = new Instruction(0x8A, AddressMode_IMP, OpCode_TXA, 2);
            _instructionTable[0x9A] = new Instruction(0x9A, AddressMode_IMP, OpCode_TXS, 2);
            _instructionTable[0x98] = new Instruction(0x98, AddressMode_IMP, OpCode_TYA, 2);

        }

        #endregion
    }
}
