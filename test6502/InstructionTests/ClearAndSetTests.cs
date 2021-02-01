using cpu6502;
using Xunit;

namespace test6502.InstructionTests
{
    public class ClearAndSet : CPUTest
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void OpCode_CLC(bool carryFlag)
        {
            CPU cpu = new CPU();
            cpu.RAM = GetCleanRam();
            cpu.SetCarry(carryFlag);
            cpu.OpCode_CLC(0x0000);

            Assert.False(cpu.CarryFlag);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void OpCode_SEC(bool carryFlag)
        {
            CPU cpu = new CPU();
            cpu.RAM = GetCleanRam();
            cpu.SetCarry(carryFlag);
            cpu.OpCode_SEC(0x0000);

            Assert.True(cpu.CarryFlag);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void OpCode_CLD(bool decimalFlag)
        {
            CPU cpu = new CPU();
            cpu.RAM = GetCleanRam();
            cpu.SetDecimal(decimalFlag);
            cpu.OpCode_CLD(0x0000);

            Assert.False(cpu.DecimalFlag);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void OpCode_SED(bool decimalFlag)
        {
            CPU cpu = new CPU();
            cpu.RAM = GetCleanRam();
            cpu.SetDecimal(decimalFlag);
            cpu.OpCode_SED(0x0000);

            Assert.True(cpu.DecimalFlag);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void OpCode_CLI(bool interruptFlag)
        {
            CPU cpu = new CPU();
            cpu.RAM = GetCleanRam();
            cpu.SetInterrupt(interruptFlag);
            cpu.OpCode_CLI(0x0000);

            Assert.False(cpu.InterruptFlag);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void OpCode_SEI(bool interruptFlag)
        {
            CPU cpu = new CPU();
            cpu.RAM = GetCleanRam();
            cpu.SetInterrupt(interruptFlag);
            cpu.OpCode_SEI(0x0000);

            Assert.True(cpu.InterruptFlag);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void OpCode_CLV(bool overflowFlag)
        {
            CPU cpu = new CPU();
            cpu.RAM = GetCleanRam();
            cpu.SetOverflow(overflowFlag);
            cpu.OpCode_CLV(0x0000);

            Assert.False(cpu.OverflowFlag);
        }


    }
}
