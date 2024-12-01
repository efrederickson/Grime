namespace Grime.Core.Executor
{
    public unsafe class CPU
    {
        // eax = lower half of rax, ax = lower half of eax, ah = upper half of ax, al = lower half of ax
        // so on for other registers and pointers. remember endianness...

        /// <summary>
        /// 4 64-bit general purpose registers
        /// </summary>
        public ulong rax, rbx, rcx, rdx;

        /// <summary>
        /// Used to pass 2nd argument to functions
        /// </summary>
        public ulong rsi;

        /// <summary>
        /// Used to pass 1st argument to functions
        /// </summary>
        public ulong rdi;

        /// <summary>
        /// 64-bit stack pointer
        /// </summary>
        public ulong rsp;

        /// <summary>
        /// 64-bit base pointer
        /// </summary>
        public ulong rbp;

        public ushort ss, cs, ds, es, fs, gs;

        /// <summary>
        /// See RFlags enum
        /// </summary>
        public RFlags rflags;

        /// <summary>
        /// 64-bit instruction pointer to the virtual address
        /// </summary>
        public ulong rip;

        /// <summary>
        /// from fetch
        /// </summary>
        public byte* instructionPtr = null;

        /// <summary>
        /// from decode
        /// </summary>
        InstructionSetExecutor? instructionDelegate = null;

        public VirtualMemory64 Memory { get; private set; }

        // Other private flags
        private bool exit = false;

        public CPU(VirtualMemory64 memory, ulong rip)
        {
            // Zero all registers
            rax = rbx = rcx = rdx = rsi = rdi = ss = cs = ds = es = fs = gs = 0;
            rflags = 0;
            this.rip = rip;
            Memory = memory ?? throw new ArgumentNullException(nameof(memory));

            // Init stack pointer to the end of the last page
            // I have no idea if this is where it should be going
            var lastPage = memory.Pages[memory.Pages.Keys.Last()];
            rsp = lastPage.VirtualAddress + lastPage.Size;

            Console.WriteLine($"CPU: initialize registers to 0, rflags to, memory page count={Memory.Pages.Count}, rsp={rsp:X}");
        }

        public bool Cycle()
        {
            Fetch();
            Decode();
            return Execute();
        }

        public unsafe void Fetch()
        {
            var page = Memory.GetPage(rip);
            if ((page.Flags & PFlags.X) != PFlags.X)
            {
                throw new AccessViolationException($"Address {rip:X} is not in a page marked X");
            }

            instructionPtr = page.PointerAt(rip);
            //Console.WriteLine($"cpu fetch {rip:X} = {*instructionPtr:X}");
        }

        public void Decode()
        {
            if (instructionPtr == null)
            {
                throw new InvalidOperationException("No instruction pointer");
            }
            // one byte opcode (or first byte)
            ushort opcode = *instructionPtr;
            // Determine opcode length
            if (opcode == 0x66)
            {
                // Operand size prefix 
                throw new NotImplementedException($"Operand size prefix 0x66 not implemented");
            }
            else if (opcode != 0x0F)
            {
                //Console.WriteLine($"CPU: decode instruction {*instructionPtr:X}");
                instructionDelegate = InstructionSet.Lookup(opcode);
                rip++; // Move one byte
                instructionPtr++;
            }
            else
            {
                if (*(instructionPtr + 1) == 0x38)
                {
                    throw new NotImplementedException($"Three-byte opcode {*instructionPtr:X} is not implemented");
                }
                if (*(instructionPtr + 1) == 0x3A)
                {
                    throw new NotImplementedException($"Three-byte opcode {*instructionPtr:X} is not implemented");
                }
                throw new NotImplementedException($"Two-byte opcode {*instructionPtr:X} is not implemented");
            }
        }

        public bool Execute()
        {
            if (instructionDelegate == null)
            {
                throw new InvalidOperationException("Instruction has not been decoded");
            }

            //Console.WriteLine($"CPU: execute instruction");
            rflags = instructionDelegate(this);
            return !exit;
        }

        public void Exit()
        {
            exit = true;
        }

        /// <summary>
        /// Push to stack
        /// </summary>
        /// <param name="value"></param>
        public void Push(uint value)
        {
            // Convert value to little endian bytes
            byte[] valueBytes =
            [
                (byte)(value >> 0),
                (byte)(value >> 8),
                (byte)(value >> 16),
                (byte)(value >> 24),
            ];
            Memory.Write(rsp - 4, valueBytes, 4);
            Console.WriteLine($"CPU: push rsp={rsp:X} new rsp={rsp - 4:X} {valueBytes[0]:X} {valueBytes[1]:X} {valueBytes[2]:X} {valueBytes[3]:X}");
            rsp -= 4;
        }

        /// <summary>
        /// Pop from stack
        /// </summary>
        /// <returns></returns>
        public uint Pop()
        {
            byte[] valueBytes = Memory.Read(rsp, 4);

            // Convert value from little endian bytes
            var res = (uint)(
                valueBytes[0] |
                valueBytes[1] << 8 |
                valueBytes[2] << 16 |
                valueBytes[3] << 24
            );
            Console.WriteLine($"CPU: pop  rsp={rsp:X} new rsp={rsp - 4:X} {valueBytes[0]:X} {valueBytes[1]:X} {valueBytes[2]:X} {valueBytes[3]:X} res={res:X}");
            rsp += 4;
            return res;
        }

        /// <summary>
        /// Return a pointer to the register referenced in the R/M bits of the Mod R/M byte
        /// </summary>
        /// <param name="modrm">ModR/M byte</param>
        /// <returns>Pointer to appropriate register</returns>
        public unsafe uint* DecodeRM(byte modrm)
        {
            // See Table 2-2. 32-Bit Addressing Forms with the ModR/M Byte in the Intel manual
            switch (modrm & 0b00000111)
            {
                // FIXME: map from rXX to eXX
                case 0b00000000: { fixed (ulong* p = &rax) { return (uint*)(p + (sizeof(uint))); } }
                case 0b00000001: { fixed (ulong* p = &rcx) { return (uint*)p + (sizeof(uint)); }; }
                case 0b00000010: { fixed (ulong* p = &rdx) { return (uint*)p + (sizeof(uint)); }; }
                case 0b00000011: { fixed (ulong* p = &rbx) { return (uint*)p + (sizeof(uint)); }; }
                case 0b00000100: { throw new NotImplementedException($"Not implemented and/or need to reference SIB byte"); }
                case 0b00000101: { throw new NotImplementedException($"disp32 not implemented"); }
                case 0b00000111: { fixed (ulong* p = &rdi) { return (uint*)p + (sizeof(uint)); }; }
                case 0b00000110: { fixed (ulong* p = &rsi) { return (uint*)p + (sizeof(uint)); }; }
                default:
                    throw new InvalidInstructionException($"Impossible! modrm mod bytes {modrm:b} matches no binary case");
            };
        }

        public unsafe byte* DecodeModRM(byte* instructionPointer)
        {
            // See https://en.wikipedia.org/wiki/ModR/M
            // See https://stackoverflow.com/questions/8518917/x86-mov-opcode-disassembling
            // See https://stackoverflow.com/questions/48992007/x86-whats-wrong-with-sib-byte-00-100-101-combination-in-32-bits
            // See http://www.c-jump.com/CIS77/CPU/x86/X77_0100_sib_byte_layout.htm
            // https://logix.cz/michal/doc/i386/chp17-02.htm / https://www.reddit.com/r/osdev/comments/i58mwn/need_some_help_understanding_x86_opcode_modrm_byte/
            // See Table 2-2. 32-Bit Addressing Forms with the ModR/M Byte in the Intel manual
            // Bit    7-6    5-4-3   2-1-0
            // Usage  "Mod"  "Reg"   "R/M"
            // Bit    7-6    5-4-3   2-1-0
            // Usage  SCALE  INDEX   BASE
            // Mod R/M 0x04  Mod=00 000 100
            // SIB 0x25

            // I cannot figure this mod/rm+sib out. hardcoding known values for now
            if (*instructionPointer == 0x04 && *(instructionPointer + 1) == 0x25)
            {
                fixed (ulong* addr = &rax)
                {
                    instructionPtr += 2;
                    rip += 2;
                    return (byte*)addr + (sizeof(uint)); // FIXME Go from RAX to EAX
                }
            }
            if (*instructionPointer == 0x1C)
            {
                fixed (ulong* addr = &rbx)
                {
                    instructionPtr += 2;
                    rip += 2;
                    return (byte*)addr + (sizeof(uint)); // FIXME Go from R to E
                }
            }

            if ((*instructionPointer & 0b11000000) == 0b11000000)
            {
                throw new NotImplementedException($"ModRM with mod 11 ({*instructionPointer:B}) is not implemented");
            }

            if ((*instructionPointer & 0b111) == 0b100)
            {
                // SIB byte follows
                //return DecodeSibByte(++instructionPointer);
            }
            else
            {
                throw new NotImplementedException($"Direct register (R/M != 0b100) is not implemented");
            }
            throw new NotImplementedException($"Unknown Mod R/M byte {*instructionPointer}");
        }

        public unsafe byte* DecodeSibByte(byte* instructionPointer)
        {
            throw new NotImplementedException("im so confused");
        }
    }
}
