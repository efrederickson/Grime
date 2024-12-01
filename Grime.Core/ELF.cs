using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Grime.Core
{
    public static class ELFConstants
    {

        /// <summary>
        /// Special value for e_phnum. This indicates that the real number of
        /// program headers is too large to fit into e_phnum.Instead the real
        // value is in the field sh_info of section 0.
        /// </summary>
        public const ushort PN_XNUM = 0xFFFF;

    }

    public class ELF64(ElfHeader64 header, ProgramHeader64[] programHeaders, SectionHeader64[] sectionHeaders)
    {
        public ElfHeader64 Header = header;
        public ProgramHeader64[] ProgramHeaders = programHeaders;
        public SectionHeader64[] SectionHeaders = sectionHeaders;
    }


    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct ElfHeader64
    {
        public EIdent e_ident;

        /// <summary>
        /// Object file type
        /// </summary>
        public EType e_type;

        /// <summary>
        /// File architecture
        /// </summary>
        public EMachine e_machine;

        /// <summary>
        /// Object file version
        /// </summary>
        public EVersion e_version;

        /// <summary>
        /// Virtual address to the entry point, or zero.
        /// </summary>
        public ulong e_entry;

        /// <summary>
        /// Points to program header table
        /// </summary>
        public ulong e_phoff;

        /// <summary>
        /// Points to section header table
        /// </summary>
        public ulong e_shoff;

        /// <summary>
        /// Dependent on target architecture
        /// </summary>
        public uint e_flags;

        /// <summary>
        /// Contains the size of this header, normally 64 Bytes for 64-bit and 52 Bytes for 32-bit format
        /// </summary>
        public ushort e_ehsize;

        /// <summary>
        /// Contains the size of a program header table entry. This will typically be 0x20 (32 bit) or 0x38 (64 bit).
        /// </summary>
        public ushort e_phentsize;

        /// <summary>
        /// Number of entries in the program header table
        /// </summary>
        public ushort e_phnum;

        /// <summary>
        /// Contains the size of a section header table entry. This will typically be 0x28 (32 bit) or 0x40 (64 bit).
        /// </summary>
        public ushort e_shentsize;

        /// <summary>
        /// Number of entries in the section header table
        /// </summary>
        public ushort e_shnum;

        /// <summary>
        /// Index of the section header table entry that contains the section names
        /// </summary>
        public ushort e_shstrndx;
    }


    /// <summary>
    /// e_ident structure
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct EIdent
    {
        public byte EI_MAG0; // 0x7F
        public byte EI_MAG1; // 0x45 'E'
        public byte EI_MAG2; // 0x4C 'L'
        public byte EI_MAG3; // 0x46 'F'
        public EIClass EI_CLASS;
        public EIData EI_DATA;
        public byte EI_VERSION;
        public EIOSABI EI_OSABI;
        public byte EI_ABIVERSION;

        /// <summary>
        /// 7 padding bytes
        /// </summary>
        public byte[] EI_PAD;
    }

    public enum EIClass : byte
    {
        /// <summary>
        /// Invalid
        /// </summary>
        ELFCLASSNONE = 0,

        /// <summary>
        /// 32-bit objects
        /// </summary>
        ELFCLASS32 = 1,

        /// <summary>
        /// 64-bit objects
        /// </summary>
        ELFCLASS64 = 2,
    }

    public enum EIData : byte
    {
        NONE = 0,

        /// <summary>
        /// Little endian
        /// </summary>
        LSB = 1,

        /// <summary>
        /// Big endian
        /// </summary>
        MSB = 2,
    }

    public enum EIOSABI : byte
    {
        NONE = 0, // No extensions or unspecified
        HPUX = 1, // Hewlett-Packard HP-UX
        NETBSD = 2, // NetBSD
        LINUX = 3, // Linux
        SOLARIS = 6, // Sun Solaris
        AIX = 7, // AIX
        IRIX = 8, // IRIX
        FREEBSD = 9, // FreeBSD
        TRU64 = 10, // Compaq TRU64 UNIX
        MODESTO = 11, // Novell Modesto
        OPENBSD = 12, // Open BSD
        OPENVMS = 13, // Open VMS
        NSK = 14, // Hewlett-Packard Non-Stop Kernel
    }

    public enum EType : ushort
    {
        ET_NONE = 0x0000, // No file type
        ET_REL = 0x0001, // Relocatable file
        ET_EXEC = 0x0002, // Executable file
        EC_DYN = 0x0003, // Shared object file
        ET_CORE = 0x0004, // Core file
        ET_LOOS = 0xFE00, // Operating system specific
        ET_HIOS = 0xFEFF, // Operating system specific
        ET_LOPROC = 0xFF00, // Processor specific
        ET_HIPROC = 0xFFFF, // Processor specific
    }

    public enum EMachine : ushort
    {
        EM_NONE = 0, // No machine
        EM_M32 = 1, // AT&T WE 32100
        EM_SPARC = 2, // SPARC
        EM_386 = 3, // Intel 80386
        EM_68K = 4, // Motorola 68000
        EM_88K = 5, // Motorola 88000
        reserved = 6, // Reserved for future use (was EM_486)
        EM_860 = 7, // Intel 80860
        EM_MIPS = 8, // MIPS I Architecture
        EM_S370 = 9, // IBM System/370 Processor
        EM_MIPS_RS3_LE = 10, // MIPS RS3000 Little-endian
                             // 11-14 reserved
        EM_PARISC = 15, // Hewlett-Packard PA-RISC
        // 16 reserved
        EM_VPP500 = 17, // Fujitsu VPP500
        EM_SPARC32PLUS = 18, // Enhanced instruction set SPARC
        EM_960 = 19, // Intel 80960
        EM_PPC = 20, // PowerPC
        EM_PPC64 = 21, // 64-bit PowerPC
        EM_S390 = 22, // IBM System/390 Processor
                      // 23-35 reserved
        EM_V800 = 36, // NEC V800
        EM_FR20 = 37, // Fujitsu FR20
        EM_RH32 = 38, // TRW RH-32
        EM_RCE = 39, // Motorola RCE
        EM_ARM = 40, // Advanced RISC Machines ARM
        EM_ALPHA = 41, // Digital Alpha
        EM_SH = 42, // Hitachi SH
        EM_SPARCV9 = 43, // SPARC Version 9
        EM_TRICORE = 44, // Siemens TriCore embedded processor
        EM_ARC = 45, // Argonaut RISC Core, Argonaut Technologies Inc.
        EM_H8_300 = 46, // Hitachi H8/300
        EM_H8_300H = 47, // Hitachi H8/300H
        EM_H8S = 48, // Hitachi H8S
        EM_H8_500 = 49, // Hitachi H8/500
        EM_IA_64 = 50, // Intel IA-64 processor architecture
        EM_MIPS_X = 51, // Stanford MIPS-X
        EM_COLDFIRE = 52, // Motorola ColdFire
        EM_68HC12 = 53, // Motorola M68HC12
        EM_MMA = 54, // Fujitsu MMA Multimedia Accelerator
        EM_PCP = 55, // Siemens PCP
        EM_NCPU = 56, // Sony nCPU embedded RISC processor
        EM_NDR1 = 57, // Denso NDR1 microprocessor
        EM_STARCORE = 58, // Motorola Star*Core processor
        EM_ME16 = 59, // Toyota ME16 processor
        EM_ST100 = 60, // STMicroelectronics ST100 processor
        EM_TINYJ = 61, // Advanced Logic Corp. TinyJ embedded processor family
        EM_X86_64 = 62, // AMD x86-64 architecture
        EM_PDSP = 63, // Sony DSP Processor
        EM_PDP10 = 64, // Digital Equipment Corp. PDP-10
        EM_PDP11 = 65, // Digital Equipment Corp. PDP-11
        EM_FX66 = 66, // Siemens FX66 microcontroller
        EM_ST9PLUS = 67, // STMicroelectronics ST9+ 8/16 bit microcontroller
        EM_ST7 = 68, // STMicroelectronics ST7 8-bit microcontroller
        EM_68HC16 = 69, // Motorola MC68HC16 Microcontroller
        EM_68HC11 = 70, // Motorola MC68HC11 Microcontroller
        EM_68HC08 = 71, // Motorola MC68HC08 Microcontroller
        EM_68HC05 = 72, // Motorola MC68HC05 Microcontroller
        EM_SVX = 73, // Silicon Graphics SVx
        EM_ST19 = 74, // STMicroelectronics ST19 8-bit microcontroller
        EM_VAX = 75, // Digital VAX
        EM_CRIS = 76, // Axis Communications 32-bit embedded processor
        EM_JAVELIN = 77, // Infineon Technologies 32-bit embedded processor
        EM_FIREPATH = 78, // Element 14 64-bit DSP Processor
        EM_ZSP = 79, // LSI Logic 16-bit DSP Processor
        EM_MMIX = 80, // Donald Knuth's educational 64-bit processor
        EM_HUANY = 81, // Harvard University machine-independent object files
        EM_PRISM = 82, // SiTera Prism
        EM_AVR = 83, // Atmel AVR 8-bit microcontroller
        EM_FR30 = 84, // Fujitsu FR30
        EM_D10V = 85, // Mitsubishi D10V
        EM_D30V = 86, // Mitsubishi D30V
        EM_V850 = 87, // NEC v850
        EM_M32R = 88, // Mitsubishi M32R
        EM_MN10300 = 89, // Matsushita MN10300
        EM_MN10200 = 90, // Matsushita MN10200
        EM_PJ = 91, // picoJava
        EM_OPENRISC = 92, // OpenRISC 32-bit embedded processor
        EM_ARC_A5 = 93, // ARC Cores Tangent-A5
        EM_XTENSA = 94, // Tensilica Xtensa Architecture
        EM_VIDEOCORE = 95, // Alphamosaic VideoCore processor
        EM_TMM_GPP = 96, // Thompson Multimedia General Purpose Processor
        EM_NS32K = 97, // National Semiconductor 32000 series
        EM_TPC = 98, // Tenor Network TPC processor
        EM_SNP1K = 99, // Trebia SNP 1000 processor
        EM_ST200 = 100, // STMicroelectronics (www.st.com) ST200 microcontroller

    }

    public enum EVersion : uint
    {
        EV_CURRENT = 1, // Current version
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ProgramHeader64
    {
        /// <summary>
        /// Type of segment.
        /// </summary>
        public PType type;

        /// <summary>
        /// Relevant flags for the segment
        /// </summary>
        public PFlags flags;

        /// <summary>
        /// Offset from the beginning of the file where this segment resides
        /// </summary>
        public ulong offset;

        /// <summary>
        /// This member gives the virtual address at which the first byte of the segment resides in memory
        /// </summary>
        public ulong vaddr;

        /// <summary>
        /// On systems for which physical addressing is relevant, this member is reserved for the segment's physical address. Because System V ignores physical addressing for application programs, this member has unspecified contents for executable files and shared objects.
        /// </summary>
        public ulong paddr;

        /// <summary>
        /// This member gives the number of bytes in the file image of the segment; it may be zero.
        /// </summary>
        public ulong filesz;

        /// <summary>
        /// This member gives the number of bytes in the memory image of the segment; it may be zero.
        /// </summary>
        public ulong memsz;

        /// <summary>
        /// This member gives the value to which the segments are aligned in memory and in the file. Values 0 and 1 mean no alignment is required. Otherwise, p_align should be a positive, integral power of 2, and p_vaddr should equal p_offset, modulo p_align.
        /// </summary>
        public ulong align;
    }

    public enum PType : uint
    {
        PT_NULL = 0, // Program header table entry unused
        PT_LOAD = 1, // Loadable program segment
        PT_DYNAMIC = 2, // Dynamic linking information
        PT_INTERP = 3, // Program interpreter
        PT_NOTE = 4, // Auxiliary information
        PT_SHLIB = 5, // Reserved
        PT_PHDR = 6, // Entry for header table itself
        PT_TLS = 7, // Thread-local storage segment
        PT_NUM = 8, // Number of defined types
        PT_LOOS = 0x60000000, // Start of OS-specific
        PT_GNU_EH_FRAME = 0x6474e550, // GCC .eh_frame_hdr segment
        PT_GNU_STACK = 0x6474e551, // Indicates stack executability
        PT_GNU_RELRO = 0x6474e552, // Read-only after relocation
        PT_GNU_PROPERTY = 0x6474e553, // GNU property
        PT_GNU_SFRAME = 0x6474e554, // SFrame segment. 
        PT_LOSUNW = 0x6ffffffa,
        PT_SUNWBSS = 0x6ffffffa, // Sun Specific segment
        PT_SUNWSTACK = 0x6ffffffb, // Stack segment
        PT_HISUNW = 0x6fffffff,
        PT_HIOS = 0x6fffffff, // End of OS-specific
        PT_LOPROC = 0x70000000, // Start of processor-specific
        PT_HIPROC = 0x7fffffff, // End of processor-specific
    }

    public enum PFlags : uint
    {
        X = 1 << 0,
        W = 1 << 1,
        R = 1 << 2,
        MASKOS = 0x0ff00000,
        MASKPROC = 0xf0000000
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SectionHeader64
    {
        /// <summary>
        /// An offset to a string in the .shstrtab section that represents the name of this section
        /// </summary>
        public uint name;

        public SHType type;

        public SHFlags flags;
        public ulong addr;
        public ulong offset;
        public ulong size;
        public uint link;
        public uint info;
        public ulong addralign;
        public ulong entsize;

        public readonly override string ToString()
        {
            return $"<Section name={name} type={type} flags={flags} addr={addr} offset={offset} size={size} link={link} info={info} addralign={addralign} entsize={entsize}>";
        }

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj is not SectionHeader64)
            {
                return false;
            }
            SectionHeader64 obj2 = (SectionHeader64)obj;
            return (
                obj2.name == name
                && obj2.type == type
                && obj2.flags == flags
                && obj2.addr == addr
                && obj2.offset == offset
                && obj2.size == size
                && obj2.link == link
                && obj2.info == info
                && obj2.addralign == addralign
                && obj2.entsize == entsize
            );
        }
    }

    public enum SHType : uint
    {
        SHT_NULL = 0, // Section header table entry unused
        SHT_PROGBITS = 1, // Program data
        SHT_SYMTAB = 2, // Symbol table
        SHT_STRTAB = 3, // String table
        SHT_RELA = 4, // Relocation entries with addends
        SHT_HASH = 5, // Symbol hash table
        SHT_DYNAMIC = 6, // Dynamic linking information
        SHT_NOTE = 7, // Notes
        SHT_NOBITS = 8, // Program space with no data (bss)
        SHT_REL = 9, // Relocation entries, no addends
        SHT_SHLIB = 10, // Reserved
        SHT_DYNSYM = 11, // Dynamic linker symbol table
        SHT_INIT_ARRAY = 14, // Array of constructors
        SHT_FINI_ARRAY = 15, // Array of destructors
        SHT_PREINIT_ARRAY = 16, // Array of pre-constructors
        SHT_GROUP = 17, // Section group
        SHT_SYMTAB_SHNDX = 18, // Extended section indices
        SHT_RELR = 19, // RELR relative relocations
        SHT_NUM = 20, // Number of defined types. 
        SHT_LOOS = 0x60000000, // Start OS-specific. 
        SHT_GNU_ATTRIBUTES = 0x6ffffff5, // Object attributes. 
        SHT_GNU_HASH = 0x6ffffff6, // GNU-style hash table. 
        SHT_GNU_LIBLIST = 0x6ffffff7, // Prelink library list
        SHT_CHECKSUM = 0x6ffffff8, // Checksum for DSO content. 
        SHT_LOSUNW = 0x6ffffffa, // Sun-specific low bound. 
        SHT_SUNW_move = 0x6ffffffa,
        SHT_SUNW_COMDAT = 0x6ffffffb,
        SHT_SUNW_syminfo = 0x6ffffffc,
        SHT_GNU_verdef = 0x6ffffffd, // Version definition section. 
        SHT_GNU_verneed = 0x6ffffffe, // Version needs section. 
        SHT_GNU_versym = 0x6fffffff, // Version symbol table. 
        SHT_HISUNW = 0x6fffffff, // Sun-specific high bound. 
        SHT_HIOS = 0x6fffffff, // End OS-specific type
        SHT_LOPROC = 0x70000000, // Start of processor-specific
        SHT_HIPROC = 0x7fffffff, // End of processor-specific
        SHT_LOUSER = 0x80000000, // Start of application-specific
        SHT_HIUSER = 0x8fffffff, // End of application-specific}
    }

    public enum SHFlags : ulong
    {
        SHF_WRITE = (1 << 0),   // Writable 
        SHF_ALLOC = (1 << 1),   // Occupies memory during execution 
        SHF_EXECINSTR = (1 << 2),   // Executable 
        SHF_MERGE = (1 << 4),   // Might be merged 
        SHF_STRINGS = (1 << 5), // Contains nul-terminated strings 
        SHF_INFO_LINK = (1 << 6),   // 'sh_info' contains SHT index 
        SHF_LINK_ORDER = (1 << 7),  // Preserve order after combining 
        SHF_OS_NONCONFORMING = (1 << 8),    // Non-standard OS specific handling required 
        SHF_GROUP = (1 << 9),       // Section is member of a group.  
        SHF_TLS = (1 << 10),        // Section hold thread-local data.  
        SHF_COMPRESSED = (1 << 11), // Section with compressed data. 
        SHF_MASKOS = 0x0ff00000,    // OS-specific.  
        SHF_MASKPROC = 0xf0000000,  // Processor-specific 
        SHF_GNU_RETAIN = (1 << 21), // Not to be GCed by linker.  
        SHF_ORDERED = (1 << 30),    // Special ordering requirement (Solaris).  
        SHF_EXCLUDE = (1U << 31),	// Section is excluded unless referenced or allocated (Solaris).
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Sym64
    {
        /// <summary>
        /// Index into .strtab for the name of the symbol
        /// </summary>
        public uint name;

        /// <summary>
        /// Attributes for the symbol
        /// </summary>
        public SymInfo info;

        /// <summary>
        /// Visibility of the symbol
        /// </summary>
        public byte other;

        /// <summary>
        /// Section index of the symbol
        /// See https://docs.oracle.com/cd/E19683-01/817-3677/chapter6-94076/index.html
        /// </summary>
        public SymSectionIndex shndx;

        /// <summary>
        /// Virtual address of the symbol
        /// </summary>
        public ulong value;

        /// <summary>
        /// Size (length) of the symbol at the address
        /// </summary>
        public ulong size;

        public override readonly string ToString()
        {
            return $"<Symbol name={name} info={info} other={other} shndx={shndx} value={value} size={size}>";
        }
    }

    public enum SymInfo : byte
    {
        STT_NOTYPE = 0, // Symbol type is unspecified
        STT_OBJECT = 1, // Symbol is a data object
        STT_FUNC = 2, // Symbol is a code object
        STT_SECTION = 3, // Symbol associated with a section
        STT_FILE = 4, // Symbol's name is file name
        STT_COMMON = 5, // Symbol is a common data object
        STT_TLS = 6, // Symbol is thread-local data object
        STT_NUM = 7, // Number of defined types. 
        STT_LOOS = 10, // Start of OS-specific
        STT_GNU_IFUNC = 10, // Symbol is indirect code object
        STT_HIOS = 12, // End of OS-specific
        STT_LOPROC = 13, // Start of processor-specific
        STT_HIPROC = 15, // End of processor-specific
    }

    public enum SymSectionIndex : ushort
    {
        SHN_UNDEF = 0,

        SHN_LORESERVE =

        0xff00,

        SHN_LOPROC =

        0xff00,

        SHN_BEFORE =

        0xff00,

        SHN_AFTER =

        0xff01,

        SHN_HIPROC =

        0xff1f,

        SHN_LOOS =

        0xff20,

        SHN_HIOS =

        0xff3f,

        SHN_ABS =

        0xfff1,

        SHN_COMMON =

        0xfff2,

        SHN_XINDEX =

        0xffff,

        SHN_HIRESERVE
                    =
        0xffff,
    }
}