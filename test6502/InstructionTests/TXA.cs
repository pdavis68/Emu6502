using cpu6502;
using Xunit;

namespace test6502.InstructionTests
{
    public class TXA : CPUTest
    {
        [Theory]
        [InlineData(0)]
        [InlineData(10)]
        [InlineData(0x7F)]
        [InlineData(0xFF)]
        public void OpCode_TXA(byte val)
        {
            CPU cpu = new CPU();
            cpu.RAM = GetCleanRam();
            cpu.XRegister = val;
            cpu.OpCode_TXA(0);

            Assert.Equal((val & 0x80) > 0, cpu.NegativeFlag);
            Assert.Equal(val == 0, cpu.ZeroFlag);
            Assert.Equal(val, cpu.XRegister);
            Assert.Equal(val, cpu.Accumulator);
        }
    }
}
