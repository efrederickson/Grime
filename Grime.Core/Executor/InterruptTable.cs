namespace Grime.Core.Executor
{
    public unsafe class InterruptTable
    {
        readonly static Dictionary<uint, InstructionSetExecutor> Interrupts = new()
         {
             { 0x80, syscall }
         };

        public static InstructionSetExecutor GetInterrupt(uint interrupt, CPU cpu)
        {
            if (!Interrupts.TryGetValue(interrupt, out InstructionSetExecutor? executor))
            {
                throw new InvalidInstructionException($"Invalid interrupt {interrupt:X}");
            }
            return executor;
        }

        private unsafe static ulong syscall(CPU cpu)
        {
            uint syscall = (uint)cpu.rax;
            return Syscalls.Execute(syscall, cpu);
        }
    }
}
