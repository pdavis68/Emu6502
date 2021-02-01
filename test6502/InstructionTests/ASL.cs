using cpu6502;
using Xunit;

namespace test6502.InstructionTests
{
    public class ASL : CPUTest
    {
        [Theory]
        [InlineData(10, 100)]
        [InlineData(0x00, 100)]
        [InlineData(0xFF, 100)]
        [InlineData(0, 0x0000)]
        [InlineData(0, 0xFFFF)]
        [InlineData(0xFF, 0x0000)]
        [InlineData(0xFF, 0xFFFF)]
        public void OpCode_ASL(byte val, ushort addr)
        {
            CPU cpu = new CPU();
            cpu.RAM = GetCleanRam();
            cpu.RAM[addr] = val;
            cpu.OpCode_ASL(addr);


            byte newVal = (byte) ((val << 1) & 0xFF);

            Assert.Equal((val & 0x80) > 0, cpu.CarryFlag);
            Assert.Equal((newVal & 0x80) > 0, cpu.NegativeFlag);
            Assert.Equal(newVal == 0, cpu.ZeroFlag);
            Assert.Equal(newVal, cpu.RAM[addr]);
        }

        [Theory]
        [InlineData(10, 100)]
        [InlineData(0x00, 100)]
        [InlineData(0xFF, 100)]
        [InlineData(0, 0x0000)]
        [InlineData(0, 0xFFFF)]
        [InlineData(0xFF, 0x0000)]
        [InlineData(0xFF, 0xFFFF)]
        public void OpCode_ASL_ACC(byte val, ushort addr)
        {
            CPU cpu = new CPU();
            cpu.RAM = GetCleanRam();
            cpu.RAM[addr] = val;
            cpu.OpCode_ASL_ACC(addr);


            byte newVal = (byte)((val << 1) & 0xFF);

            Assert.Equal((val & 0x80) > 0, cpu.CarryFlag);
            Assert.Equal((newVal & 0x80) > 0, cpu.NegativeFlag);
            Assert.Equal(newVal == 0, cpu.ZeroFlag);
            Assert.Equal(newVal, cpu.Accumulator);
        }
    }
}