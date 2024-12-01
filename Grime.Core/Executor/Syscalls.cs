using System.Text;

namespace Grime.Core.Executor
{
    public static class Syscalls
    {
        static readonly Dictionary<uint, InstructionSetExecutor> SyscallMapping = new()
        {
            { 0x01, exit },
            { 0x03, read },
            { 0x04, write }
        };

        public static RFlags Execute(uint syscall, CPU cpu)
        {
            if (!SyscallMapping.TryGetValue(syscall, out InstructionSetExecutor? executor))
            {
                throw new NotImplementedException($"Syscall {syscall:X} is not implemented");
            }
            return executor(cpu);
        }

        static unsafe RFlags exit(CPU cpu)
        {
            cpu.Exit();
            return 0;
        }

        static unsafe RFlags read(CPU cpu)
        {
            // same as write's fixmes
            uint fd = (uint)cpu.rbx;
            var dest = cpu.rcx;
            var count = cpu.rdx;
            var buff = new byte[count];
            for (ulong i = 0; i < count; i++)
            {
                buff[i] = (byte)Console.Read(); //FIXME: assumes ASCII
            }

            cpu.Memory.Write(dest, buff, count);
            return 0;
        }

        static unsafe RFlags write(CPU cpu)
        {
            // FIXME: casts 
            // FIXME: respect fd
            uint fd = (uint)cpu.rbx;
            var buffer = cpu.rcx;
            uint count = (uint)cpu.rdx;
            var data = cpu.Memory.Read(buffer, count);
            var sb = new StringBuilder();
            foreach (var piece in data)
            {
                sb.Append((char)piece);
            }

            //Console.Write(sb.ToString());
            Console.WriteLine($"OUT: '{sb.ToString()}'");
            return 0;
        }
    }
}
