using Grime.Core.Executor;

namespace Grime.Core.Tests.Executor
{
    [TestClass]
    public sealed class TestInstructionSet
    {
        CPU BuildCPU(byte[] data)
        {
            var pages = new Page64[1]
            {
                new(
                    data,
                    (ulong)data.Length,
                    0x00000000,
                    0,
                    PFlags.R | PFlags.X
                )
            };
            var mem = new VirtualMemory64(pages);
            ulong rip = 0;
            return new CPU(mem, rip);
        }

        [TestMethod]
        public void Test_Op_0x39()
        {
            var cpu = BuildCPU(
            [
                0x8B, 0x04, 0x25, 0x01, 0x00, 0x00, 0x00, // MOV eax, 1
                0xBB, 0x04, 0x25, 0x00, 0x00, 0x00, 0x00, // MOV ebx, 0
                0x39, 0x03                                // CMP eax, ebx
            ]);
            cpu.Cycle();
            cpu.Cycle();
            cpu.Cycle();
            Assert.IsTrue(cpu.rax == 1);
            Assert.IsTrue(cpu.rbx == 0);
            Assert.IsTrue((cpu.rflags & RFlags.ZERO_FLAG__) == 0, $"Expected ZF to be 0, got {(cpu.rflags & RFlags.ZERO_FLAG__)}");
            cpu = BuildCPU(
            [
                0x8B, 0x04, 0x25, 0x14, 0x00, 0x00, 0x00, // MOV eax, 1
                0xBB, 0x04, 0x25, 0x14, 0x00, 0x00, 0x00, // MOV ebx, 0
                0x39, 0x03                                // CMP eax, ebx
            ]);
            cpu.Cycle();
            cpu.Cycle();
            cpu.Cycle();
            Assert.IsTrue(cpu.rax == 1);
            Assert.IsTrue(cpu.rbx == 0);
            Assert.IsTrue((cpu.rflags & RFlags.ZERO_FLAG__) == RFlags.ZERO_FLAG__, $"Expected ZFLAGS to be nonzero, got {(cpu.rflags & RFlags.ZERO_FLAG__)}");
        }
    }
}
