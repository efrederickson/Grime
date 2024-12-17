using Grime.Core.Executor;

namespace Grime.Core.Tests.Executor
{
    [TestClass]
    public sealed class TestInstructionSet
    {
        CPU BuildCPU(byte[] data, byte[] initmem)
        {
            var pages = new Page64[2]
            {
                new(
                    data,
                    (ulong)data.Length,
                    0x00000000,
                    0,
                    PFlags.R | PFlags.X
                ),
                new(
                    initmem,
                    (ulong)initmem.Length,
                    0x10000000,
                    0,
                    PFlags.R| PFlags.W
                ),
            };
            var mem = new VirtualMemory64(pages);
            ulong rip = 0;
            return new CPU(mem, rip);
        }

        [TestMethod]
        public unsafe void Test_Op_0x39()
        {
            var cpu = BuildCPU(
            [
                0x8B, 0x04, 0x25, 0x00, 0x00, 0x00, 0x10, // MOV eax, 1
                0x8B, 0x1C, 0x25, 0x04, 0x00, 0x00, 0x10, // MOV ebx, 0
                0x39, 0x03                                // CMP eax, ebx
            ], [
                0x01, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00
            ]);
            cpu.Cycle();
            cpu.Cycle();
            cpu.Cycle();

            Assert.IsTrue(cpu.rax == 1);
            Assert.IsTrue(cpu.rbx == 0);
            Assert.IsTrue((cpu.rflags & RFlags.ZERO_FLAG__) == 0, $"Expected ZF to be 0, got {(cpu.rflags & RFlags.ZERO_FLAG__)}");

            cpu = BuildCPU(
            [
                0x8B, 0x04, 0x25, 0x00, 0x00, 0x00, 0x10, // MOV eax, 1
                0x8B, 0x1C, 0x25, 0x00, 0x00, 0x00, 0x10, // MOV ebx, 1 
                0x39, 0x03                                // CMP eax, ebx
            ], [
                0x01, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00
            ]);
            cpu.Cycle();
            cpu.Cycle();
            cpu.Cycle();

            Assert.IsTrue(cpu.rax == 1);
            Assert.IsTrue(cpu.rbx == 1);
            Assert.IsTrue((cpu.rflags & RFlags.ZERO_FLAG__) == RFlags.ZERO_FLAG__, $"Expected ZFLAGS to be nonzero, got {(cpu.rflags & RFlags.ZERO_FLAG__)}");
        }
    }
}
