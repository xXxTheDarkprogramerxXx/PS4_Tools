//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using elf = llvm.ELF;

//namespace PS4_Tools.Files.SELF
//{
//    //C++ TO C# CONVERTER TODO TASK: Unions are not supported in C#:
//    //union hilo64_t
//    //{
//    //  ulong v;
//    //  struct
//    //  {
//    //	uint lo;
//    //	uint hi;
//    //  };
//    //};

//    public struct hilo64_t
//    {
//        ulong v;
//        uint lo;
//        uint hi;
//    }

//    public class self_header
//    {
//        public const uint signature = 0x1D3D154Fu;

//        public uint magic;
//        public uint unknown04;
//        public uint unknown08;
//        public ushort header_size;
//        public ushort unknown_size;
//        public uint file_size;
//        public uint unknown14;
//        public ushort segment_count;
//        public ushort unknown1A;
//        public uint unknown1C;

//        public void Read(BinaryReader input)
//        {
//            input.BaseStream.Seek(0, SeekOrigin.Begin);
//            input.Read(magic, 0, 4);//4 bytes
//                                    //input.Read(rsv_06, 16, 2); //+16 size 2
//            header_size = input.ReadUInt32();
//            total_bytes = input.ReadUInt64();//long
//            block_size = input.ReadUInt32();
//            ver = input.ReadByte();
//            align = input.ReadByte();
//            rsv_06 = input.ReadBytes(2);
//        }
//    }

//    public class self_segment_header
//    {
//        public uint flags;
//        public uint unknown04;
//        public hilo64_t offset = new hilo64_t();
//        public hilo64_t compressed_size = new hilo64_t();
//        public hilo64_t uncompressed_size = new hilo64_t();
//    }

//    public class self_info
//    {
//        public hilo64_t id = new hilo64_t();
//        public hilo64_t unknown08 = new hilo64_t();
//        public hilo64_t system_version_1 = new hilo64_t();
//        public hilo64_t system_version_2 = new hilo64_t();
//        public byte[] content_id = new byte[32];
//    }

//    public static class SELF
//    {
//        public static void Open(string SELFFile)
//        {
//            var handle = fopen(args[1], "rb");
//            if (handle == null)
//            {
//                Console.Write("Failed to open file.\n");
//                return 2;
//            }

//            BinaryReader rb = new BinaryReader((new FileStream(SELFFile, FileMode.Open, FileAccess.Read)))
            
//            self_header header = new self_header();

            


            
//            if (fread(header, sizeof(self_header), 1, handle) != 1)
//            {
//                Console.Write("Failed to read SELF header.\n");
//                fclose(handle);
//                return 3;
//            }

//            if (header.magic != self_header.signature)
//            {
//                Console.Write("Not a SELF file.\n");
//                fclose(handle);
//                return 4;
//            }

//            //C++ TO C# CONVERTER TODO TASK: The memory management function 'malloc' has no equivalent in C#:
//            var segment_headers = (self_segment_header)malloc(sizeof(self_segment_header) * header.segment_count);
//            if (fread(segment_headers, sizeof(self_segment_header), header.segment_count, handle) != header.segment_count)
//            {
//                Console.Write("Failed to read SELF segment headers.\n");
//                segment_headers = null;
//                fclose(handle);
//                return 5;
//            }

//            byte[] ident = new byte[elf.EI_NIDENT];
//            if (fread(ident, sizeof(byte), 1, handle) != 1)
//            {
//                Console.Write("Failed to read ELF ident.\n");
//                segment_headers = null;
//                fclose(handle);
//                return 6;
//            }

//            fseek(handle, -(int)sizeof(byte), SEEK_CUR);

//            uint elf_header_size;
//            ushort program_header_size;
//            ushort program_count;
//            if (ident[elf.EI_CLASS] == 1)
//            {
//                elf.Elf32_Ehdr elf_header = new elf.Elf32_Ehdr();
//                if (fread(elf_header, sizeof(elf.Elf32_Ehdr), 1, handle) != 1)
//                {
//                    Console.Write("Failed to read ELF header.\n");
//                    segment_headers = null;
//                    fclose(handle);
//                    return 7;
//                }
//                elf_header_size = sizeof(elf.Elf32_Ehdr);
//                program_header_size = elf_header.e_phentsize;
//                program_count = elf_header.e_phnum;
//            }
//            else if (ident[elf.EI_CLASS] == 2)
//            {
//                elf.Elf64_Ehdr elf_header = new elf.Elf64_Ehdr();
//                if (fread(elf_header, sizeof(elf.Elf64_Ehdr), 1, handle) != 1)
//                {
//                    Console.Write("Failed to read ELF header.\n");
//                    segment_headers = null;
//                    fclose(handle);
//                    return 7;
//                }
//                elf_header_size = sizeof(elf.Elf64_Ehdr);
//                program_header_size = elf_header.e_phentsize;
//                program_count = elf_header.e_phnum;
//            }
//            else
//            {
//                Console.Write("Unknown ELF class.\n");
//                segment_headers = null;
//                fclose(handle);
//                return 8;
//            }

//            Console.Write("SELF header:\n");
//            Console.Write("  magic ............: {0:x8}\n", header.magic);
//            Console.Write("  unknown 04 .......: {0:x8}\n", header.unknown04);
//            Console.Write("  unknown 08 .......: {0:x8}\n", header.unknown08);
//            Console.Write("  header size ......: {0:x}\n", header.header_size);
//            Console.Write("  unknown size .....: {0:x}\n", header.unknown_size);
//            Console.Write("  file size ........: {0:x}\n", header.file_size);
//            Console.Write("  unknown 14 .......: {0:x8}\n", header.unknown14);
//            Console.Write("  segment count ....: {0:D}\n", header.segment_count);
//            Console.Write("  unknown 1A .......: {0:x4}\n", header.unknown1A);
//            Console.Write("  unknown 1C .......: {0:x4}\n", header.unknown1C);
//            Console.Write("\n");

//            Console.Write("SELF segments:\n");

//            for (int i = 0; i < header.segment_count; i++)
//            {
//                var segment_header = segment_headers[i];
//                Console.Write(" [{0:D}]\n", i);
//                Console.Write("  flags ............: {0:x8}\n", segment_header.flags);
//                Console.Write("  offset ...........: {0:x}\n", segment_header.offset.v);
//                Console.Write("  compressed size ..: {0:x}\n", segment_header.compressed_size.v);
//                Console.Write("  uncompressed size : {0:x}\n", segment_header.uncompressed_size.v);
//            }
//            Console.Write("\n");

//            uint base_header_size = 0;
//            base_header_size += sizeof(self_header);
//            base_header_size += sizeof(self_segment_header) * header.segment_count;
//            base_header_size += elf_header_size;
//            base_header_size += program_count * program_header_size;
//            base_header_size += 15;
//            base_header_size &= ~15; // align

//            if (header.header_size - base_header_size >= sizeof(self_info))
//            {
//                fseek(handle, (int)base_header_size, SEEK_SET);
//                self_info info = new self_info();
//                if (fread(info, sizeof(self_info), 1, handle) == 1)
//                {
//                    Console.Write("SELF info:\n");
//                    Console.Write("  auth id ..........: {0:x8}{1:x8}\n", info.id.hi, info.id.lo);
//                    Console.Write("  unknown 08 .......: {0:x}\n", info.unknown08.v);
//                    Console.Write("  system version 1 .: {0:x}\n", info.system_version_1.v);
//                    Console.Write("  system version 2 .: {0:x}\n", info.system_version_2.v);
//                    Console.Write("  content id .......:");
//                    for (int i = 0; i < 16; i++)
//                    {
//                        Console.Write(" {0:x2}", info.content_id[i]);
//                    }
//                    Console.Write("\n");
//                    Console.Write("                     ");
//                    for (int i = 0; i < 16; i++)
//                    {
//                        Console.Write(" {0:x2}", info.content_id[16 + i]);
//                    }
//                    Console.Write("\n");
//                    Console.Write("\n");
//                }
//            }

//            segment_headers = null;
//        }
//    }
//}
