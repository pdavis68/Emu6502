using cpu6502;
using Xunit;

namespace test6502.ProgramTests
{
    public class SamplePrograms1 : CPUTest
    {

        /// <summary>
        /// LDA #$01
        /// STA $0200
        /// LDX $0200
        /// CPX #$01
        /// 
        /// a9 01 8d 00 02 ae 00 02 e0 01 
        /// </summary>
        [Fact]
        public void Sample_1_1()
        {
            CPU cpu = new CPU();
            cpu.RAM = GetCleanRam();
            byte[] program = GetProgramData("a9 01 8d 00 02 ae 00 02 e0 01");
            cpu.LoadProgram(0x600, program);
            cpu.InstructionPointer = 0x600;
            try
            {
                cpu.Run(100);
            }
            catch(CPUBreakException)
            {
                Assert.Equal(0x01, cpu.Accumulator);
                Assert.Equal(0x01, cpu.XRegister);
                Assert.Equal(0x00, cpu.YRegister);
                Assert.Equal(0x01, cpu.RAM[512]);
                Assert.True(cpu.ZeroFlag);
                Assert.True(cpu.CarryFlag);
                Assert.Equal(0x600 + program.Length + 1, cpu.InstructionPointer);
                return;
            }
            Assert.True(false, "Didn't get CPUBreakException");
        }


        /// <summary>
        /// LDA #$c0  ;Load the hex value $c0 into the A register
        /// TAX       ;Transfer the value in the A register to X
        /// INX; Increment the value in the X register
        /// ADC #$c4  ;Add the hex value $c4 to the A register
        /// 
        /// a9 c0 aa e8 69 c4 
        /// </summary>
        [Fact]
        public void Sample_1_2()
        {
            CPU cpu = new CPU();
            cpu.RAM = GetCleanRam();
            byte[] program = GetProgramData("a9 c0 aa e8 69 c4");
            cpu.LoadProgram(0x600, program);
            cpu.InstructionPointer = 0x600;
            try
            {
                cpu.Run(100);
            }
            catch (CPUBreakException)
            {
                Assert.Equal(0x84, cpu.Accumulator);
                Assert.Equal(0xC1, cpu.XRegister);
                Assert.Equal(0x00, cpu.YRegister);
                Assert.False(cpu.ZeroFlag);
                Assert.False(cpu.OverflowFlag);
                Assert.True(cpu.CarryFlag);
                Assert.True(cpu.NegativeFlag);
                Assert.Equal(0x600 + program.Length + 1, cpu.InstructionPointer);
                return;
            }
            Assert.True(false, "Didn't get CPUBreakException");
        }


        /// <summary>
        /// LDA #$90
        /// STA $01
        /// ADC $01
        /// 
        /// a9 90 85 01 65 01 
        /// </summary>
        [Fact]
        public void Sample_1_3()
        {
            CPU cpu = new CPU();
            cpu.RAM = GetCleanRam();
            byte[] program = GetProgramData("a9 90 85 01 65 01");
            cpu.LoadProgram(0x600, program);
            cpu.InstructionPointer = 0x600;
            try
            {
                cpu.Run(100);
            }
            catch (CPUBreakException)
            {
                Assert.Equal(0x20, cpu.Accumulator);
                Assert.Equal(0x00, cpu.XRegister);
                Assert.Equal(0x00, cpu.YRegister);
                Assert.False(cpu.ZeroFlag);
                Assert.True(cpu.OverflowFlag);
                Assert.True(cpu.CarryFlag);
                Assert.False(cpu.NegativeFlag);
                Assert.Equal(0x600 + program.Length + 1, cpu.InstructionPointer);
                return;
            }
            Assert.True(false, "Didn't get CPUBreakException");
        }

        /// <summary>
        /// LDX #$04
        /// TXA
        /// ADC #$23
        /// TAY
        /// CPY #$27
        /// BEQ THERE
        /// HERE:
        /// LDY #$FF
        /// STY $100
        /// JMP EXIT
        /// THERE:
        /// LDY #$FE
        /// STY $100
        /// EXIT:
        /// STX $102
        /// STA $103
        /// 
        /// a2 04 8a 69 23 a8 c0 27 f0 08 a0 ff 8c 00 01 4c 17 06 a0 fe 8c 00 01 8e 02 01 8d 03 01 
        /// </summary>
        [Fact]
        public void Sample_1_4()
        {
            CPU cpu = new CPU();
            cpu.RAM = GetCleanRam();
            byte[] program = GetProgramData("a2 04 8a 69 23 a8 c0 27 f0 08 a0 ff 8c 00 01 4c 17 06 a0 fe 8c 00 01 8e 02 01 8d 03 01");
            cpu.LoadProgram(0x600, program);
            cpu.InstructionPointer = 0x600;
            try
            {
                cpu.Run(100);
            }
            catch (CPUBreakException)
            {
                Assert.Equal(0x27, cpu.Accumulator);
                Assert.Equal(0x04, cpu.XRegister);
                Assert.Equal(0xFE, cpu.YRegister);
                Assert.False(cpu.ZeroFlag);
                Assert.False(cpu.OverflowFlag); // Broken ADC implementation
                Assert.True(cpu.CarryFlag);
                Assert.True(cpu.NegativeFlag);
                Assert.Equal(0xFE, cpu.RAM[0x100]);
                Assert.Equal(0x04, cpu.RAM[0x102]);
                Assert.Equal(0x27, cpu.RAM[0x103]);
                Assert.Equal(0x600 + program.Length + 1, cpu.InstructionPointer);
                return;
            }
            Assert.True(false, "Didn't get CPUBreakException");
        }

    }
}
