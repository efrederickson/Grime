using Grime.Core.Executor;
using System.Text;

namespace Grime.Core
{
    public static class ELFReader
    {
        /// <summary>
        /// Parse the entire ELF binary (non-lazy).
        /// </summary>
        /// <param name="stream">Must be seekable.</param>
        /// <returns></returns>
        public static ELF64 Load(Stream stream)
        {
            var header = ReadHeader64(stream);
            var pheaders = ReadProgramHeaders64(header, stream);
            var sheaders = ReadSectionHeaders64(header, stream);

            return new ELF64(header, pheaders, sheaders);
        }

        public static ElfHeader64 ReadHeader64(Stream stream)
        {
            EIdent eident = ReadIdent(stream);
            if (eident.EI_CLASS == EIClass.ELFCLASS32)
            {
                // 32-bit
                throw new NotImplementedException("EI_CLASS=1 (32-bit) is not implemented");
            }
            else if (eident.EI_CLASS == EIClass.ELFCLASS64)
            {
                // 64-bit
                ElfHeader64 header = new()
                {
                    e_ident = eident,
                    e_type = (EType)ReadUInt16LE(stream),
                    e_machine = (EMachine)ReadUInt16LE(stream),
                    e_version = (EVersion)ReadUInt32LE(stream),
                    e_entry = ReadUInt64LE(stream),
                    e_phoff = ReadUInt64LE(stream),
                    e_shoff = ReadUInt64LE(stream),
                    e_flags = ReadUInt32LE(stream),
                    e_ehsize = ReadUInt16LE(stream),
                    e_phentsize = ReadUInt16LE(stream),
                    e_phnum = ReadUInt16LE(stream),
                    e_shentsize = ReadUInt16LE(stream),
                    e_shnum = ReadUInt16LE(stream),
                    e_shstrndx = ReadUInt16LE(stream),
                };
                if (header.e_phnum == ELFConstants.PN_XNUM)
                {
                    throw new NotImplementedException("PN_XNUM is not implemented");
                }

                if (header.e_ehsize != 64)
                {
                    throw new InvalidElfException($"Expected e_ehsize to be 64, got {header.e_ehsize}");
                }

                return header;
            }
            else
            {
                throw new InvalidElfException($"Invalid EI_CLASS {eident.EI_CLASS}");
            }
        }

        public static EIdent ReadIdent(Stream stream)
        {
            EIdent ret = new()
            {
                EI_MAG0 = (byte)stream.ReadByte(),
                EI_MAG1 = (byte)stream.ReadByte(),
                EI_MAG2 = (byte)stream.ReadByte(),
                EI_MAG3 = (byte)stream.ReadByte(),
                EI_CLASS = (EIClass)stream.ReadByte(),
                EI_DATA = (EIData)stream.ReadByte(),
                EI_VERSION = (byte)stream.ReadByte(),
                EI_OSABI = (EIOSABI)stream.ReadByte(),
                EI_ABIVERSION = (byte)stream.ReadByte()
            };
            stream.ReadExactly(ret.EI_PAD = new byte[7], 0, 7);
            return ret;
        }

        public static ProgramHeader64[] ReadProgramHeaders64(ElfHeader64 header, Stream stream)
        {
            // Seek 
            stream.Seek((long)header.e_phoff, SeekOrigin.Begin);
            ProgramHeader64[] headers = new ProgramHeader64[header.e_phnum];
            // Read 
            for (int i = 0; i < header.e_phnum; i++)
            {
                // FIXME: use phentsize
                headers[i] = ReadProgramHeader64(stream);
            }

            return headers;
        }

        public static ProgramHeader64 ReadProgramHeader64(Stream stream)
        {

            return new()
            {
                type = (PType)ReadUInt32LE(stream),
                flags = (PFlags)ReadUInt32LE(stream),
                offset = ReadUInt64LE(stream),
                vaddr = ReadUInt64LE(stream),
                paddr = ReadUInt64LE(stream),
                filesz = ReadUInt64LE(stream),
                memsz = ReadUInt64LE(stream),
                align = ReadUInt64LE(stream),
            };
        }

        public static SectionHeader64[] ReadSectionHeaders64(ElfHeader64 header, Stream stream)
        {
            // Seek 
            stream.Seek((long)header.e_shoff, SeekOrigin.Begin);
            SectionHeader64[] headers = new SectionHeader64[header.e_shnum];
            // Read 
            for (int i = 0; i < header.e_shnum; i++)
            {
                // FIXME: use shentsize
                headers[i] = ReadSectionHeader64(stream);
            }

            return headers;

        }

        public static SectionHeader64 ReadSectionHeader64(Stream stream)
        {
            var header = new SectionHeader64()
            {
                name = ReadUInt32LE(stream),
                type = (SHType)ReadUInt32LE(stream),
                flags = (SHFlags)ReadUInt64LE(stream),
                addr = ReadUInt64LE(stream),
                offset = ReadUInt64LE(stream),
                size = ReadUInt64LE(stream),
                link = ReadUInt32LE(stream),
                info = ReadUInt32LE(stream),
                addralign = ReadUInt64LE(stream),
                entsize = ReadUInt64LE(stream),
            };
            if ((header.flags & SHFlags.SHF_COMPRESSED) == SHFlags.SHF_COMPRESSED)
            {
                throw new NotImplementedException("SHF_COMPRESSED not implemented");
            }

            return header;
        }

        public static Sym64 ReadSymbol(Stream stream)
        {
            return new Sym64()
            {
                name = ReadUInt32LE(stream),
                info = (SymInfo)stream.ReadByte(),
                other = (byte)stream.ReadByte(),
                shndx = (SymSectionIndex)ReadUInt16LE(stream),
                value = ReadUInt64LE(stream),
                size = ReadUInt64LE(stream),
            };
        }

        public static String ReadString(Stream stream, ELF64 elf, ulong sectionIndex, uint nameOffset)
        {
            // Sections have already been read.
            // Otherwise, we can find the string offset, given the section index, like this:
            // var offset = elf.Header.e_shoff + (sectionIndex * elf.Header.e_shentsize);
            // stream.Seek((long)offset, SeekOrigin.Begin);
            // var strtab = ELFReader.ReadSectionHeader64(stream);
            // var stroffset = strtab.offset + nameOffset;
            var stroffset = elf.SectionHeaders[sectionIndex].offset + nameOffset;
            stream.Seek((long)stroffset, SeekOrigin.Begin);
            return ReadString(stream);
        }

        public static String ReadString(Stream s)
        {
            StringBuilder sb = new();
            char c;
            while ((c = (char)s.ReadByte()) != 0)
            {
                sb.Append(c);
            }

            return sb.ToString();
        }

        public static Page64[] ReadAndAlignSegments(ELF64 elf, Stream stream)
        {
            Page64[] pages = new Page64[elf.ProgramHeaders.Length];
            for (int i = 0; i < elf.ProgramHeaders.Length; i++)
            {
                var ph = elf.ProgramHeaders[i];
                if (ph.type != PType.PT_LOAD)
                {
                    throw new NotImplementedException($"Program Header type {ph.type} is not implemented");
                }
                // Seek to file offset (p_offset)
                stream.Seek((long)ph.offset, SeekOrigin.Begin);
                // Alloc memory of size memsz
                byte[] region = new byte[ph.memsz];
                // Read filesz bytes from stream into mem
                stream.Read(region, 0, (int)ph.filesz);
                // Move read mem into virtual memory at virtaddr (sort of)
                pages[i] = new Page64(region, ph.memsz, ph.vaddr, ph.align, ph.flags);
            }
            return pages;
        }

        public static ushort ReadUInt16LE(Stream stream)
        {
            return (ushort)(
                (byte)stream.ReadByte() |
                (byte)stream.ReadByte() << 8
            );
        }
        public static uint ReadUInt32LE(Stream stream)
        {
            return (uint)(
                (byte)stream.ReadByte() |
                (byte)stream.ReadByte() << 8 |
                (byte)stream.ReadByte() << 16 |
                (byte)stream.ReadByte() << 24
            );
        }
        public static ulong ReadUInt64LE(Stream stream)
        {
            return (ulong)(
                (byte)stream.ReadByte() |
                (byte)stream.ReadByte() << 8 |
                (byte)stream.ReadByte() << 16 |
                (byte)stream.ReadByte() << 24 |
                (byte)stream.ReadByte() << 32 |
                (byte)stream.ReadByte() << 40 |
                (byte)stream.ReadByte() << 48 |
                (byte)stream.ReadByte() << 56
            );
        }
    }
}
