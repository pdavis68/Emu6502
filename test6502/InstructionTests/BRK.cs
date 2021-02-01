using cpu6502;
using Xunit;

namespace test6502.InstructionTests
{
    public class BRK : CPUTest
    {
        [Theory]
        [InlineData(0x0000, 0x0000)]
        [InlineData(0x0000, 0x8888)]
        [InlineData(0x0000, 0xFFFF)]
        [InlineData(0xFFFF, 0x0000)]
        [InlineData(0xFFFF, 0x8888)]
        [InlineData(0xFFFF, 0xFFFF)]
        [InlineData(100, 0x0000)]
        [InlineData(100, 0x8888)]
        [InlineData(100, 0xFFFF)]
        public void OpCode_BRK(ushort ipAddr, ushort irqAddr)
        {
            CPU cpu = new CPU();
            cpu.RAM = GetCleanRam();
            cpu.InstructionPointer = ipAddr;
            cpu.RAM[CPU.BREAK_IRQ_VECTOR] = (byte) (irqAddr / 256);
            cpu.RAM[CPU.BREAK_IRQ_VECTOR + 1] = (byte) (irqAddr % 256);

            ushort sp = cpu.StackPointer;

            cpu.OpCode_BRK(ipAddr);

            Assert.Equal(cpu.InstructionPointer, irqAddr);
            Assert.Equal(sp - 2, cpu.StackPointer);
            Assert.Equal(cpu.RAM[sp + 0x100 - 1], (byte)((ipAddr + 1) % 256));
            Assert.Equal(cpu.RAM[sp + 0x100 - 2], (byte)((ipAddr + 1) / 256));
        }
    }
}