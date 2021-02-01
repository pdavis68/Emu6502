using cpu6502;
using Xunit;

namespace test6502.InstructionTests
{
    public class BranchTests : CPUTest
    {
        [Theory]
        [InlineData(true, 100)]
        [InlineData(true, 0x0000)]
        [InlineData(true, 0xFFFF)]
        [InlineData(false, 100)]
        [InlineData(false, 0x0000)]
        [InlineData(false, 0xFFFF)]
        public void OpCode_BCC(bool carryFlag, ushort addr)
        {
            CPU cpu = new CPU();
            cpu.RAM = GetCleanRam();
            cpu.SetCarry(carryFlag);
            cpu.InstructionPointer = 0x8888;
            cpu.OpCode_BCC(addr);
            if (!carryFlag)
            {
                Assert.Equal(addr, cpu.InstructionPointer);
            }
            else
            {
                Assert.Equal(0x8888, cpu.InstructionPointer );
            }
        }

        [Theory]
        [InlineData(true, 100)]
        [InlineData(true, 0x0000)]
        [InlineData(true, 0xFFFF)]
        [InlineData(false, 100)]
        [InlineData(false, 0x0000)]
        [InlineData(false, 0xFFFF)]
        public void OpCode_BCS(bool carryFlag, ushort addr)
        {
            CPU cpu = new CPU();
            cpu.RAM = GetCleanRam();
            cpu.SetCarry(carryFlag);
            cpu.InstructionPointer = 0x8888;
            cpu.OpCode_BCS(addr);
            if (carryFlag)
            {
                Assert.Equal(addr, cpu.InstructionPointer);
            }
            else
            {
                Assert.Equal(0x8888, cpu.InstructionPointer);
            }
        }

        [Theory]
        [InlineData(true, 100)]
        [InlineData(true, 0x0000)]
        [InlineData(true, 0xFFFF)]
        [InlineData(false, 100)]
        [InlineData(false, 0x0000)]
        [InlineData(false, 0xFFFF)]
        public void OpCode_BEQ(bool zeroFlag, ushort addr)
        {
            CPU cpu = new CPU();
            cpu.RAM = GetCleanRam();
            cpu.SetZero(zeroFlag);
            cpu.InstructionPointer = 0x8888;
            cpu.OpCode_BEQ(addr);
            if (zeroFlag)
            {
                Assert.Equal(addr, cpu.InstructionPointer);
            }
            else
            {
                Assert.Equal(0x8888, cpu.InstructionPointer);
            }
        }

        [Theory]
        [InlineData(true, 100)]
        [InlineData(true, 0x0000)]
        [InlineData(true, 0xFFFF)]
        [InlineData(false, 100)]
        [InlineData(false, 0x0000)]
        [InlineData(false, 0xFFFF)]
        public void OpCode_BNE(bool zeroFlag, ushort addr)
        {
            CPU cpu = new CPU();
            cpu.RAM = GetCleanRam();
            cpu.SetZero(zeroFlag);
            cpu.InstructionPointer = 0x8888;
            cpu.OpCode_BNE(addr);
            if (!zeroFlag)
            {
                Assert.Equal(addr, cpu.InstructionPointer);
            }
            else
            {
                Assert.Equal(0x8888, cpu.InstructionPointer);
            }
        }

        [Theory]
        [InlineData(true, 100)]
        [InlineData(true, 0x0000)]
        [InlineData(true, 0xFFFF)]
        [InlineData(false, 100)]
        [InlineData(false, 0x0000)]
        [InlineData(false, 0xFFFF)]
        public void OpCode_BMI(bool negativeFlag, ushort addr)
        {
            CPU cpu = new CPU();
            cpu.RAM = GetCleanRam();
            cpu.SetNegative(negativeFlag);
            cpu.InstructionPointer = 0x8888;
            cpu.OpCode_BMI(addr);
            if (negativeFlag)
            {
                Assert.Equal(addr, cpu.InstructionPointer);
            }
            else
            {
                Assert.Equal(0x8888, cpu.InstructionPointer);
            }
        }

        [Theory]
        [InlineData(true, 100)]
        [InlineData(true, 0x0000)]
        [InlineData(true, 0xFFFF)]
        [InlineData(false, 100)]
        [InlineData(false, 0x0000)]
        [InlineData(false, 0xFFFF)]
        public void OpCode_BPL(bool negativeFlag, ushort addr)
        {
            CPU cpu = new CPU();
            cpu.RAM = GetCleanRam();
            cpu.SetNegative(negativeFlag);
            cpu.InstructionPointer = 0x8888;
            cpu.OpCode_BPL(addr);
            if (!negativeFlag)
            {
                Assert.Equal(addr, cpu.InstructionPointer);
            }
            else
            {
                Assert.Equal(0x8888, cpu.InstructionPointer);
            }
        }

        [Theory]
        [InlineData(true, 100)]
        [InlineData(true, 0x0000)]
        [InlineData(true, 0xFFFF)]
        [InlineData(false, 100)]
        [InlineData(false, 0x0000)]
        [InlineData(false, 0xFFFF)]
        public void OpCode_BVC(bool overflowFlag, ushort addr)
        {
            CPU cpu = new CPU();
            cpu.RAM = GetCleanRam();
            cpu.SetOverflow(overflowFlag);
            cpu.InstructionPointer = 0x8888;
            cpu.OpCode_BVC(addr);
            if (!overflowFlag)
            {
                Assert.Equal(addr, cpu.InstructionPointer);
            }
            else
            {
                Assert.Equal(0x8888, cpu.InstructionPointer);
            }
        }

        [Theory]
        [InlineData(true, 100)]
        [InlineData(true, 0x0000)]
        [InlineData(true, 0xFFFF)]
        [InlineData(false, 100)]
        [InlineData(false, 0x0000)]
        [InlineData(false, 0xFFFF)]
        public void OpCode_BVS(bool overflowFlag, ushort addr)
        {
            CPU cpu = new CPU();
            cpu.RAM = GetCleanRam();
            cpu.SetOverflow(overflowFlag);
            cpu.InstructionPointer = 0x8888;
            cpu.OpCode_BVS(addr);
            if (overflowFlag)
            {
                Assert.Equal(addr, cpu.InstructionPointer);
            }
            else
            {
                Assert.Equal(0x8888, cpu.InstructionPointer);
            }
        }

    }
}
