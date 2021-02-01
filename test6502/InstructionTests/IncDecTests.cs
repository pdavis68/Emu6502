using cpu6502;
using Xunit;

namespace test6502.InstructionTests
{
    public class IncDecTests : CPUTest
    {
        [Theory]
        [InlineData(0x00)]
        [InlineData(10)]
        [InlineData(0xFF)]
        public void OpCode_INC(byte val)
        {
            CPU cpu = new CPU();
            cpu.RAM = GetCleanRam();
            cpu.RAM[0x100] = val;
            cpu.OpCode_INC(0x100);

            byte newVal = (byte)(val + 1);
            Assert.Equal((newVal & 0x80) > 0, cpu.NegativeFlag);
            Assert.Equal((newVal == 0), cpu.ZeroFlag);
            Assert.Equal(newVal, cpu.RAM[0x100]);
        }

        [Theory]
        [InlineData(0x00)]
        [InlineData(10)]
        [InlineData(0xFF)]
        public void OpCode_DEC(byte val)
        {
            CPU cpu = new CPU();
            cpu.RAM = GetCleanRam();
            cpu.RAM[0x100] = val;
            cpu.OpCode_DEC(0x100);

            byte newVal = (byte)(val - 1);
            Assert.Equal((newVal & 0x80) > 0, cpu.NegativeFlag);
            Assert.Equal((newVal == 0), cpu.ZeroFlag);
            Assert.Equal(newVal, cpu.RAM[0x100]);
        }

        [Theory]
        [InlineData(0x00)]
        [InlineData(10)]
        [InlineData(0xFF)]
        public void OpCode_INX(byte val)
        {
            CPU cpu = new CPU();
            cpu.RAM = GetCleanRam();
            cpu.XRegister = val;
            cpu.OpCode_INX(0x100);

            byte newVal = (byte)(val + 1);
            Assert.Equal((newVal & 0x80) > 0, cpu.NegativeFlag);
            Assert.Equal((newVal == 0), cpu.ZeroFlag);
            Assert.Equal(newVal, cpu.XRegister);
        }

        [Theory]
        [InlineData(0x00)]
        [InlineData(10)]
        [InlineData(0xFF)]
        public void OpCode_DEX(byte val)
        {
            CPU cpu = new CPU();
            cpu.RAM = GetCleanRam();
            cpu.XRegister = val;
            cpu.OpCode_DEX(0x100);

            byte newVal = (byte)(val - 1);
            Assert.Equal((newVal & 0x80) > 0, cpu.NegativeFlag);
            Assert.Equal((newVal == 0), cpu.ZeroFlag);
            Assert.Equal(newVal, cpu.XRegister);
        }

        [Theory]
        [InlineData(0x00)]
        [InlineData(10)]
        [InlineData(0xFF)]
        public void OpCode_INY(byte val)
        {
            CPU cpu = new CPU();
            cpu.RAM = GetCleanRam();
            cpu.YRegister = val;
            cpu.OpCode_INY(0x100);

            byte newVal = (byte)(val + 1);
            Assert.Equal((newVal & 0x80) > 0, cpu.NegativeFlag);
            Assert.Equal((newVal == 0), cpu.ZeroFlag);
            Assert.Equal(newVal, cpu.YRegister);
        }

        [Theory]
        [InlineData(0x00)]
        [InlineData(10)]
        [InlineData(0xFF)]
        public void OpCode_DEY(byte val)
        {
            CPU cpu = new CPU();
            cpu.RAM = GetCleanRam();
            cpu.YRegister = val;
            cpu.OpCode_DEY(0x100);

            byte newVal = (byte)(val - 1);
            Assert.Equal((newVal & 0x80) > 0, cpu.NegativeFlag);
            Assert.Equal((newVal == 0), cpu.ZeroFlag);
            Assert.Equal(newVal, cpu.YRegister);
        }
    }
}