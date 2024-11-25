namespace Grime.Core.Executor
{
    public unsafe delegate ulong InstructionSetExecutor(CPU cpu);

    /// <summary>
    /// Provide decoding and execution implementation of instructions
    /// </summary>
    public unsafe class InstructionSet
    {
        readonly static Dictionary<ushort, InstructionSetExecutor> OpcodeMap = new()
        {
            {0x39, Op_0x39 },
            {0x74, Op_0x74 },
            {0x83, Op_0x83 },
            {0x89, Op_0x89 },
            {0x8B, Op_0x8B },
            {0xB8, Op_0xB8 },
            {0xB9, Op_0xB9 },
            {0x90, Op_0x90 },
            {0xBA, Op_0xBA },
            {0xBB, Op_0xBB },
            {0xBC, Op_0xBC },
            {0xBD, Op_0xBD },
            {0xBE, Op_0xBE },
            {0xBF, Op_0xBF },
            {0xC3, Op_0xC3 },
            {0xCD, Op_0xCD },
            {0xE8, Op_0xE8 },
            {0xEB, Op_0xEB }
        };

        public static InstructionSetExecutor Lookup(ushort opcode)
        {
            if (!OpcodeMap.TryGetValue(opcode, out InstructionSetExecutor? value))
            {
                throw new InvalidInstructionException($"Invalid or not implemented instruction {opcode:X}");
            }
            return value;
        }

        /// <summary>
        /// Read little endian uint32 from ptr
        /// </summary>
        static unsafe uint ReadUInt32LE(byte* ptr)
        {
            return (uint)(
                *ptr++ |
                *ptr++ << 8 |
                *ptr++ << 16 |
                *ptr++ << 24
            );
        }

        /// <summary>
        /// CMP r/m16,r32
        /// </summary>
        unsafe static ulong Op_0x39(CPU cpu)
        {
            // http://sparksandflames.com/files/x86InstructionChart.html
            // CMP Ev Gv
            // E=A ModR/M byte follows the opcode and specifies the operand. The operand is either a general-purpose register or a memory address. If it is a memory address, the address is computed from a segment register and any of the following values: a base register, an index register, a scaling factor, a displacement.
            // G=The reg field of the ModR/M byte selects a general register (for example, AX (000)).
            // v=Word or doubleword, depending on operand-size attribute.
            var e = cpu.DecodeRM((byte)(*cpu.instructionPtr >> 3));
            var g = cpu.DecodeRM(*cpu.instructionPtr);
            Console.WriteLine($"0x39 CMP {*e:X} {*g:X}");
            cpu.rip++; // mod r/m byte
            if (*e == *g)
            {
                return (ulong)RFlags.ZERO_FLAG__;
            }

            return 0;
        }

        /// <summary>
        /// JE byte (jump if ZF=1 to rip+=byte)
        /// </summary>
        /// <param name="cpu"></param>
        /// <returns></returns>
        unsafe static ulong Op_0x74(CPU cpu)
        {
            var addr = GMath.UnsignedToSigned(*cpu.instructionPtr++);
            cpu.rip++; // jump byte
            ulong zf = cpu.rflags & (ulong)RFlags.ZERO_FLAG__;
            Console.WriteLine($"0x74 JE/JZ {addr:X} (ZF={zf})");
            if (zf == (ulong)RFlags.ZERO_FLAG__)
            {
                unchecked
                {
                    cpu.rip += (ulong)addr;
                }
            }
            return 0;
        }

        /// <summary>
        /// Overloaded opcode
        /// </summary>
        unsafe static ulong Op_0x83(CPU cpu)
        {
            // See: https://stackoverflow.com/questions/26607462/x86-opcode-instruction-decoding
            // http://ref.x86asm.net/coder64.html#x83
            // intel assembly manual: Table A-6. Opcode Extensions for One- and Two-byte Opcodes by Group Number *
            if ((*cpu.instructionPtr & 0b11000000) != 0b11000000)
            {
                throw new InvalidInstructionException($"Expected MOD bytes {(*cpu.instructionPtr & 0b11000000):B} to be 0b11000000 for instruction 0x83");
            }

            var extension = *cpu.instructionPtr & 0b00111000;
            cpu.rip++; // for modrm byte
            return extension switch
            {
                0 => Op_0x83_0_ADD(cpu, (byte)(extension & 0b00000111)), // ADD - 0x83 /0 ib (immediate byte) - FIXME i don't actually have to mask here
                _ => throw new NotImplementedException($"Extension {extension:B} to opcode 0x83 not implemented"),
            };
        }

        unsafe static ulong Op_0x83_0_ADD(CPU cpu, byte register)
        {
            // 83 /0 ib	ADD r/m16, imm8	Add sign-extended imm8 to r/m16
            // 83 /0 ib ADD r/m32, imm8 Add sign-extended imm8 to r/m32
            // The OF, SF, ZF, AF, CF, and PF flags are set according to the result.
            byte* dest = cpu.DecodeRM(register);
            var immediate = *++cpu.instructionPtr;
            Console.WriteLine($"0x83 ADD {*dest} imm {immediate}");
            *dest += immediate;
            cpu.rip++; // for immediate byte
            return (ulong)RFlags.CARRY_FLAG_; // FIXME: set flags
        }

        /// <summary>
        /// MOV 
        /// </summary>
        unsafe static ulong Op_0x89(CPU cpu)
        {
            byte* loc = cpu.DecodeModRM(cpu.instructionPtr);
            var memAddr = ReadUInt32LE(cpu.instructionPtr);
            byte[] data = [*loc++, *loc++, *loc++, *loc++]; // 4 bytes
            cpu.Memory.Write(memAddr, data, 4);
            Console.WriteLine($"0x89 MOV {memAddr:X} <- {data[0]:X} {data[1]:X} {data[2]:X} {data[3]:X}");
            cpu.rip += 4;
            return 0;
        }

        /// <summary>
        /// MOV r16/32 r/m 16/32
        /// mov Gv, Ev
        /// </summary>
        unsafe static ulong Op_0x8B(CPU cpu)
        {
            byte* loc = cpu.DecodeModRM(cpu.instructionPtr);
            var memAddr = ReadUInt32LE(cpu.instructionPtr);
            byte[] data = cpu.Memory.Read(memAddr, 4);
            *loc = data[0];
            *(loc + 1) = data[1];
            *(loc + 2) = data[2];
            *(loc + 3) = data[3];
            Console.WriteLine($"0x8B MOV {data[0]:X} {data[1]:X} {data[2]:X} {data[3]:X} -> {*loc:X}");
            cpu.rip += 4;
            return 0;
        }

        unsafe static ulong Op_0x90(CPU cpu)
        {
            Console.WriteLine("0x90 NOP");
            return 0;
        }

        /// <summary>
        /// MOV immediate DWORD into EAX
        /// </summary>
        unsafe static ulong Op_0xB8(CPU cpu)
        {
            uint value = ReadUInt32LE(cpu.instructionPtr);
            Console.WriteLine($"0xB8 MOV EAX, {value:X}");
            cpu.rax = value;
            cpu.rip += 4;
            return 0;
        }

        /// <summary>
        /// MOV immediate DWORD into ECX
        /// </summary>
        unsafe static ulong Op_0xB9(CPU cpu)
        {
            uint value = ReadUInt32LE(cpu.instructionPtr);
            Console.WriteLine($"0xB9 MOV ECX, {value:X}");
            cpu.rcx = value;
            cpu.rip += 4;
            return 0;
        }

        /// <summary>
        /// MOV immediate DWORD into EDX
        /// </summary>
        unsafe static ulong Op_0xBA(CPU cpu)
        {
            uint value = ReadUInt32LE(cpu.instructionPtr);
            Console.WriteLine($"0xBA MOV EDX, {value:X}");
            cpu.rdx = value;
            cpu.rip += 4;
            return 0;
        }

        /// <summary>
        /// MOV immediate DWORD into EBX
        /// </summary>
        unsafe static ulong Op_0xBB(CPU cpu)
        {
            uint value = ReadUInt32LE(cpu.instructionPtr);
            Console.WriteLine($"0xBB MOV EBX, {value:X}");
            cpu.rbx = value;
            cpu.rip += 4;
            return 0;
        }

        /// <summary>
        /// MOV immediate DWORD into ESP
        /// </summary>
        unsafe static ulong Op_0xBC(CPU cpu)
        {
            uint value = ReadUInt32LE(cpu.instructionPtr);
            Console.WriteLine($"0xBC MOV ESP, {value:X}");
            cpu.rsp = value;
            cpu.rip += 4;
            return 0;
        }

        /// <summary>
        /// MOV immediate DWORD into EBP
        /// </summary>
        unsafe static ulong Op_0xBD(CPU cpu)
        {
            uint value = ReadUInt32LE(cpu.instructionPtr);
            Console.WriteLine($"0xBD MOV EBP, {value:X}");
            cpu.rbp = value;
            cpu.rip += 4;
            return 0;
        }

        /// <summary>
        /// MOV immediate DWORD into ESI
        /// </summary>
        unsafe static ulong Op_0xBE(CPU cpu)
        {
            uint value = ReadUInt32LE(cpu.instructionPtr);
            Console.WriteLine($"0xB8 MOV ESI, {value:X}");
            cpu.rsi = value;
            cpu.rip += 4;
            return 0;
        }

        /// <summary>
        /// MOV immediate DWORD into EDI
        /// </summary>
        unsafe static ulong Op_0xBF(CPU cpu)
        {
            uint value = ReadUInt32LE(cpu.instructionPtr);
            Console.WriteLine($"0xBF MOV EDI, {value:X}");
            cpu.rdi = value;
            cpu.rip += 4;
            return 0;
        }

        /// <summary>
        /// RET
        /// </summary>
        /// <param name="cpu"></param>
        /// <returns></returns>
        unsafe static ulong Op_0xC3(CPU cpu)
        {
            var addr = cpu.Pop();
            Console.WriteLine($"0xC3 RET (to {addr:X})");
            cpu.rip = addr;
            return 0;
        }

        /// <summary>
        /// INT immediate byte
        /// </summary>
        /// <param name="cpu"></param>
        /// <param name="cpu.instructionPtr"></param>
        /// <returns></returns>
        unsafe static ulong Op_0xCD(CPU cpu)
        {
            Console.WriteLine($"0xCD INT {*cpu.instructionPtr:X}");
            InterruptTable.GetInterrupt(*cpu.instructionPtr++, cpu)(cpu);
            cpu.rip++;
            return 0;
        }

        /// <summary>
        /// CALL (32-bit signed addr)
        /// </summary>
        /// <param name="cpu"></param>
        /// <returns></returns>
        unsafe static ulong Op_0xE8(CPU cpu)
        {
            uint addr = ReadUInt32LE(cpu.instructionPtr);
            int signedAddr = GMath.UnsignedToSigned(addr);
            cpu.rip += 4; // size of jump address
            cpu.Push((uint)cpu.rip);
            // FIXME: this only works by not overflowing a ulong lol
            Console.Write($"0xE8 CALL {addr:X} ({signedAddr})");
            unchecked { cpu.rip += (ulong)signedAddr; }
            Console.WriteLine($" (resulting rip={cpu.rip:X})");
            return 0;
        }

        /// <summary>
        /// JMP byte
        /// </summary>
        unsafe static ulong Op_0xEB(CPU cpu)
        {
            var jmp = GMath.UnsignedToSigned(*cpu.instructionPtr);
            Console.Write($"0xEB JMP {*cpu.instructionPtr++:X} ({jmp:X}) ");
            cpu.rip++; // Size of jump address (byte)
            unchecked
            {
                cpu.rip += (ulong)jmp;
            }
            Console.WriteLine($" (resulting address {cpu.rip:X})");
            return 0;
        }
    }
}