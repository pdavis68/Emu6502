using cpu6502;
using Xunit;

namespace test6502.InstructionTests
{
    public class LSR : CPUTest
    {
        [Theory]
        [InlineData(10, 100)]
        [InlineData(0x00, 100)]
        [InlineData(0xFF, 100)]
        [InlineData(0, 0x0000)]
        [InlineData(0, 0xFFFF)]
        [InlineData(0xFF, 0x0000)]
        [InlineData(0xFF, 0xFFFF)]
        public void OpCode_LSR(byte val, ushort addr)
        {
            CPU cpu = new CPU();
            cpu.RAM = GetCleanRam();
            cpu.RAM[addr] = val;
            cpu.OpCode_LSR(addr);

            byte newVal = (byte) (val >> 1);
            Assert.Equal(newVal, cpu.RAM[addr]);
            Assert.Equal(newVal == 0, cpu.ZeroFlag);
            Assert.Equal((val & 0x01) > 0, cpu.CarryFlag);
            Assert.False(cpu.NegativeFlag);
        }

        [Theory]
        [InlineData(10, 100)]
        [InlineData(0x00, 100)]
        [InlineData(0xFF, 100)]
        [InlineData(0, 0x0000)]
        [InlineData(0, 0xFFFF)]
        [InlineData(0xFF, 0x0000)]
        [InlineData(0xFF, 0xFFFF)]
        public void OpCode_LSR_ACC(byte val, ushort addr)
        {
            CPU cpu = new CPU();
            cpu.RAM = GetCleanRam();
            cpu.RAM[addr] = val;
            cpu.OpCode_LSR_ACC(addr);

            byte newVal = (byte)(val >> 1);
            Assert.Equal(newVal, cpu.Accumulator);
            Assert.Equal(newVal == 0, cpu.ZeroFlag);
            Assert.Equal((val & 0x01) > 0, cpu.CarryFlag);
            Assert.False(cpu.NegativeFlag);
        }
    }
}
