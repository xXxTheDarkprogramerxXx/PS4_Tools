using System;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace PS4_Tools
{
    internal class Unpacker
    {
        public void Unpack(string PUPFile,string SaveDir,bool saveTables = false)
        {
            string inputPath = PUPFile;
            string baseOutputPath = SaveDir;

            var fileNames = new Dictionary<uint, string>()
            {
                { 3, "wlan_firmware.bin" },
                { 5, "secure_modules.bin" },
                { 6, Path.Combine("fs", "system.img" ) },
                { 8, Path.Combine("fs", "eap.img") },
                { 9, Path.Combine("fs", "recovery.img") },
                { 11, Path.Combine("fs", "preinst.img") },
                { 12, Path.Combine("fs", "system_ex.img") },
                { 34, "torus2_firmware.bin" },
                { 257, "eula.xml" },
                { 512, "orbis_swu.elf" },
                { 514, "orbis_swu.self" },
                { 3337, "cp_firmware.bin" },
            };

            var deviceNames = new Dictionary<uint, string>()
            {
                { 1, "/dev/sflash0s0x32b" },
                { 13, "/dev/sflash0s0x32b" },
                { 32, "/dev/sflash0s0x32b" },
                { 36, "/dev/sflash0s0x32b" },
                { 40, "/dev/sflash0s0x32b" },
                { 42, "/dev/sflash0s0x32b" },
                { 44, "/dev/sflash0s0x32b" },
                { 46, "/dev/sflash0s0x32b" },
                { 2, "/dev/sflash0s0x33" },
                { 14, "/dev/sflash0s0x33" },
                { 33, "/dev/sflash0s0x33" },
                { 37, "/dev/sflash0s0x33" },
                { 43, "/dev/sflash0s0x33" },
                { 3, "/dev/sflash0s0x38" },
                { 34, "/dev/sflash0s0x38" },
                { 48, "/dev/sflash0s0x38" },
                { 4, "/dev/sflash0s1.cryptx2b" },
                { 35, "/dev/sflash0s1.cryptx2b" },
                { 38, "/dev/sflash0s1.cryptx2b" },
                { 39, "/dev/sflash0s1.cryptx2b" },
                { 45, "/dev/sflash0s1.cryptx2b" },
                { 5, "/dev/sflash0s1.cryptx3b" },
                { 10, "/dev/sflash0s1.cryptx40" },
                { 9, "/dev/da0x0.crypt" },
                { 11, "/dev/da0x1.crypt" },
                { 7, "/dev/da0x2" },
                { 8, "/dev/da0x3.crypt" },
                { 6, "/dev/da0x4b.crypt" },
                { 12, "/dev/da0x5b.crypt" },
                { 3328, "/dev/sc_fw_update0" },
                { 3336, "/dev/sc_fw_update0" },
                { 3335, "/dev/sc_fw_update0" },
                { 3329, "cd0" },
                { 3330, "da0" },
                { 16, "/dev/sbram0" },
                { 17, "/dev/sbram0" },
                { 19, "/dev/sbram0" },
                { 20, "/dev/sbram0" },
                { 21, "/dev/sbram0" },
                { 22, "/dev/sbram0" },
                { 18, "/dev/sbram0" },
                { 3337, "cpfirm" },
                { 15, "test" },
                { 769, "/update" },
                { 770, "/update" },
                { 782, "test" },
                { 783, "test" },
            };

            using (var input = File.OpenRead(inputPath))
            {
                var reader = new BinaryReader(input);

                var magic = reader.ReadUInt32();
                var unknown04 = reader.ReadUInt32();
                var unknown08 = reader.ReadUInt16();
                var flags = reader.ReadByte();
                var unknown0B = reader.ReadByte();
                var headerSize = reader.ReadUInt16();
                var hashSize = reader.ReadUInt16();
                var fileSize = reader.ReadInt64();
                var entryCount = reader.ReadUInt16();
                var hashCount = reader.ReadUInt16();
                var unknown1C = reader.ReadUInt32();

                var entries = new Entry[entryCount];
                for (int i = 0; i < entryCount; i++)
                {
                    Entry entry;
                    entry.Flags = reader.ReadUInt32();
                    input.Seek(4, SeekOrigin.Current);
                    entry.Offset = reader.ReadInt64();
                    entry.CompressedSize = reader.ReadInt64();
                    entry.UncompressedSize = reader.ReadInt64();
                    entries[i] = entry;
                }

                var tableEntries = new int[entryCount];
                for (int i = 0; i < entryCount; i++)
                {
                    tableEntries[i] = -2;
                }

                for (int i = 0; i < entryCount; i++)
                {
                    var entry = entries[i];
                    if (entry.IsBlocked == false)
                    {
                        tableEntries[i] = -2;
                    }
                    else
                    {
                        if (((entry.Id | 0x100) & 0xF00) == 0xF00)
                        {
                            throw new InvalidOperationException();
                        }

                        int tableIndex = -1;
                        for (int j = 0; j < entryCount; j++)
                        {
                            if ((entries[j].Flags & 1) != 0)
                            {
                                if (entries[j].Id == i)
                                {
                                    tableIndex = j;
                                    break;
                                }
                            }
                        }

                        if (tableIndex < 0)
                        {
                            throw new InvalidOperationException();
                        }

                        if (tableEntries[tableIndex] != -2)
                        {
                            throw new InvalidOperationException();
                        }

                        tableEntries[tableIndex] = i;
                    }
                }

                for (int i = 0; i < entries.Length; i++)
                {
                    var entry = entries[i];

                    var special = entry.Flags & 0xF0000000;
                    if (special == 0xE0000000 || special == 0xF0000000)
                    {
                        continue;
                    }

                    string name;

                    if (tableEntries[i] < 0)
                    {
                        if (fileNames.TryGetValue(entry.Id, out name) == true)
                        {
                        }
                        else if (deviceNames.TryGetValue(entry.Id, out name) == true)
                        {
                            var index = name.LastIndexOf('/');
                            name = name.Substring(index + 1);
                            name = entry.Id + "__" + name + ".bin";
                            name = Path.Combine("devices", name);
                        }
                        else
                        {
                            name = entry.Id + ".bin";
                            name = Path.Combine("unknown", name);
                        }
                    }
                    else
                    {
                        if (saveTables == false)
                        {
                            continue;
                        }
                        name = string.Format("{0} for {1}.bin", entry.Id, entries[tableEntries[i]].Id);
                        name = Path.Combine("tables", name);
                    }

                    var outputPath = Path.Combine(baseOutputPath, name);

                    var outputParentPath = Path.GetDirectoryName(outputPath);
                    if (string.IsNullOrEmpty(outputParentPath) == false)
                    {
                        Directory.CreateDirectory(outputParentPath);
                    }

                    using (var output = File.Create(outputPath))
                    {
                        ExtractEntry(input, i, entry, entries, output);
                    }
                }
            }
        }

        private static void CopyStream(Stream input, Stream output, long size)
        {
            long left = size;
            var data = new byte[1 * 1024 * 1024];
            while (left > 0)
            {
                var block = (int)(Math.Min(left, data.Length));
                var read = input.Read(data, 0, block);
                if (read != block)
                {
                    throw new EndOfStreamException();
                }
                output.Write(data, 0, block);
                left -= block;
            }
        }

        private static void ExtractEntry(Stream input, int index, Entry entry, Entry[] entries, Stream output)
        {
            if (entry.IsBlocked == false)
            {
                input.Position = entry.Offset;
                if (entry.IsCompressed == false)
                {
                    CopyStream(input, output, entry.CompressedSize);
                }
                else
                {
                    var zlib = new InflaterInputStream(input);
                    CopyStream(zlib, output, entry.UncompressedSize);
                }
            }
            else
            {
                if (((entry.Id | 0x100) & 0xF00) == 0xF00)
                {
                    throw new InvalidOperationException();
                }

                int tableIndex = -1;
                for (int j = 0; j < entries.Length; j++)
                {
                    if ((entries[j].Flags & 1) != 0)
                    {
                        if (entries[j].Id == index)
                        {
                            tableIndex = j;
                            break;
                        }
                    }
                }

                if (tableIndex < 0)
                {
                    throw new InvalidOperationException();
                }

                var tableEntry = entries[tableIndex];

                var blockSize = 1u << (int)(((entry.Flags & 0xF000) >> 12) + 12);
                var blockCount = (blockSize + entry.UncompressedSize - 1) / blockSize;
                var tailSize = entry.UncompressedSize % blockSize;
                if (tailSize == 0)
                {
                    tailSize = blockSize;
                }

                BlockInfo[] blockInfos = null;
                if (entry.IsCompressed == true)
                {
                    blockInfos = new BlockInfo[blockCount];
                    using (var tableData = new MemoryStream())
                    {
                        input.Position = tableEntry.Offset;
                        if (tableEntry.IsCompressed == false)
                        {
                            CopyStream(input, tableData, tableEntry.CompressedSize);
                        }
                        else
                        {
                            var zlib = new InflaterInputStream(input);
                            CopyStream(zlib, tableData, tableEntry.UncompressedSize);
                        }

                        tableData.Position = 32 * blockCount;
                        var tableReader = new BinaryReader(tableData);
                        for (int j = 0; j < blockCount; j++)
                        {
                            BlockInfo blockInfo;
                            blockInfo.Offset = tableReader.ReadUInt32();
                            blockInfo.Size = tableReader.ReadUInt32();
                            blockInfos[j] = blockInfo;
                        }
                    }
                }

                var buffer = new byte[blockSize];

                input.Position = entry.Offset;

                var compressedRemainingSize = entry.CompressedSize;
                var uncompressedRemainingSize = entry.UncompressedSize;
                var lastIndex = blockCount - 1;

                for (int i = 0; i < blockCount; i++)
                {
                    long compressedReadSize, uncompressedReadSize;
                    bool blockIsCompressed = false;

                    if (entry.IsCompressed == true)
                    {
                        var blockInfo = blockInfos[i];
                        var unpaddedSize = (blockInfo.Size & ~0xFu) - (blockInfo.Size & 0xFu);

                        compressedReadSize = blockSize;
                        if (unpaddedSize != blockSize)
                        {
                            compressedReadSize = blockInfo.Size;
                            if (i != lastIndex || tailSize != blockInfo.Size)
                            {
                                compressedReadSize &= ~0xFu;
                                blockIsCompressed = true;
                            }
                        }

                        if (blockInfo.Offset != 0)
                        {
                            var blockOffset = entry.Offset + blockInfo.Offset;
                            input.Position = blockOffset;
                            output.Position = i * blockSize;
                        }
                    }
                    else
                    {
                        compressedReadSize = compressedRemainingSize;
                        if (blockSize < compressedReadSize)
                        {
                            compressedReadSize = blockSize;
                        }
                    }

                    if (blockIsCompressed == true)
                    {
                        using (var temp = new MemoryStream())
                        {
                            CopyStream(input, temp, compressedReadSize - (blockInfos[i].Size & 0xF));
                            temp.Position = 0;

                            uncompressedReadSize = uncompressedRemainingSize;
                            if (blockSize < uncompressedReadSize)
                            {
                                uncompressedReadSize = blockSize;
                            }

                            var zlib = new InflaterInputStream(temp);
                            var read = zlib.Read(buffer, 0, (int)uncompressedReadSize);
                            if (read != uncompressedReadSize)
                            {
                                throw new InvalidOperationException();
                            }

                            output.Write(buffer, 0, (int)(uncompressedReadSize));
                        }
                    }
                    else
                    {
                        uncompressedReadSize = compressedReadSize;
                        var read = input.Read(buffer, 0, (int)compressedReadSize);
                        if (read != compressedReadSize)
                        {
                            throw new InvalidOperationException();
                        }
                        output.Write(buffer, 0, (int)compressedReadSize);
                    }

                    compressedRemainingSize -= compressedReadSize;
                    uncompressedRemainingSize -= uncompressedReadSize;
                }
            }
        }
    }
}
