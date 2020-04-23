using PS4_Tools.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace PS4_Tools.TROPHY
{
    public class TROPUSR
    {
        string path;
        Header header;
        Dictionary<int, TypeRecord> typeRecordTable;
        public List<TrophyType> trophyTypeTable = new List<TrophyType>();
        public List<TrophyTimeInfo> trophyTimeInfoTable = new List<TrophyTimeInfo>();
        public TrophyListInfo trophyListInfo;
        public string account_id;
        public string trophy_id;
        public int all_trophy_number;
        byte[] unknowHash;
        uint[] AchievementRate = new uint[4];
        UnknowType7 unknowType7;

        public TROPUSR(string path_in)
        {
            this.path = path_in;
            BinaryReader TROPUSRReader = null;

            if (path == null)
                throw new Exception("Path cannot be null!");

            if (!path.EndsWith(@"\"))
                path += @"\";

            if (!File.Exists(path + "TROPUSR.DAT"))
                throw new Exception("Cannot find TROPUSR.DAT.");

            try
            {
                TROPUSRReader = new BinaryReader(new FileStream(path + "TROPUSR.DAT", FileMode.Open));
            }
            catch (IOException)
            {
                throw new Exception("Cannot Open TROPUSR.DAT.");
            }

            header = TROPUSRReader.ReadBytes(Marshal.SizeOf(typeof(Header))).ToStruct<Header>();
            if (header.Magic != 0x0000000100ad548f81)
                throw new Exception("Not a vaild TROPUSR.DAT.");



            typeRecordTable = new Dictionary<int, TypeRecord>();
            for (int i = 0; i < header.UnknowCount; i++)
            {
                TypeRecord TypeRecordTmp = TROPUSRReader.ReadBytes(Marshal.SizeOf(typeof(TypeRecord))).ToStruct<TypeRecord>();
                typeRecordTable.Add(TypeRecordTmp.ID, TypeRecordTmp);
            }

            do
            {
                // 1 unknow 2 account_id 3 trophy_id and hash(?) 4 trophy info
                // 
                int type = TROPUSRReader.ReadInt32();
                int blocksize = TROPUSRReader.ReadInt32();
                int sequenceNumber = TROPUSRReader.ReadInt32(); // if have more than same type block, it will be used
                int unknow = TROPUSRReader.ReadInt32();
                byte[] blockdata = TROPUSRReader.ReadBytes(blocksize);
                switch (type)
                {
                    case 1: // unknow
                        break;
                    case 2:
                        account_id = Encoding.UTF8.GetString(blockdata, 16, 16);
                        break;
                    case 3:
                        trophy_id = Encoding.UTF8.GetString(blockdata, 0, 16).Trim('\0');
                        short u1 = BitConverter.ToInt16(blockdata, 16).ChangeEndian();
                        short u2 = BitConverter.ToInt16(blockdata, 18).ChangeEndian();
                        short u3 = BitConverter.ToInt16(blockdata, 20).ChangeEndian();
                        short u4 = BitConverter.ToInt16(blockdata, 22).ChangeEndian();
                        all_trophy_number = BitConverter.ToInt32(blockdata, 24).ChangeEndian();
                        int u5 = BitConverter.ToInt32(blockdata, 28).ChangeEndian();
                        AchievementRate[0] = BitConverter.ToUInt32(blockdata, 64);
                        AchievementRate[1] = BitConverter.ToUInt32(blockdata, 68);
                        AchievementRate[2] = BitConverter.ToUInt32(blockdata, 72);
                        AchievementRate[3] = BitConverter.ToUInt32(blockdata, 76);
                        break;
                    case 4:
                        trophyTypeTable.Add(blockdata.ToStruct<TrophyType>());
                        break;
                    case 5:
                        trophyListInfo = blockdata.ToStruct<TrophyListInfo>();
                        break;
                    case 6:
                        trophyTimeInfoTable.Add(blockdata.ToStruct<TrophyTimeInfo>());
                        break;
                    case 7:// unknow
                        unknowType7 = blockdata.ToStruct<UnknowType7>();
                        // Console.WriteLine("Unsupported block type. (Type{0})", type);
                        break;
                    case 8: // hash
                        unknowHash = blockdata.SubArray(0, 20);
                        break;
                    case 9: // 通常寫著白金獎盃的一些數字，不明
                        // Console.WriteLine("Unsupported block type. (Type{0})", type);
                        break;
                    case 10: // i think it just a padding
                        break;
                }

            } while (TROPUSRReader.BaseStream.Position < TROPUSRReader.BaseStream.Length);

            trophyListInfo.ListLastUpdateTime = DateTime.Now;
            TROPUSRReader.Close();
        }

        public TROPUSR(byte[] TROPUSRFile)
        {
          
            BinaryReader TROPUSRReader = null;

           
            try
            {
                TROPUSRReader = new BinaryReader(new MemoryStream(TROPUSRFile));
            }
            catch (IOException)
            {
                throw new Exception("Cannot Open TROPUSR.DAT.");
            }

            header = TROPUSRReader.ReadBytes(Marshal.SizeOf(typeof(Header))).ToStruct<Header>();
            if (header.Magic != 0x0000000100ad548f81)
                throw new Exception("Not a vaild TROPUSR.DAT.");



            typeRecordTable = new Dictionary<int, TypeRecord>();
            for (int i = 0; i < header.UnknowCount; i++)
            {
                TypeRecord TypeRecordTmp = TROPUSRReader.ReadBytes(Marshal.SizeOf(typeof(TypeRecord))).ToStruct<TypeRecord>();
                typeRecordTable.Add(TypeRecordTmp.ID, TypeRecordTmp);
            }

            do
            {
                // 1 unknow 2 account_id 3 trophy_id and hash(?) 4 trophy info
                // 
                int type = TROPUSRReader.ReadInt32();
                int blocksize = TROPUSRReader.ReadInt32();
                int sequenceNumber = TROPUSRReader.ReadInt32(); // if have more than same type block, it will be used
                int unknow = TROPUSRReader.ReadInt32();
                byte[] blockdata = TROPUSRReader.ReadBytes(blocksize);
                switch (type)
                {
                    case 1: // unknow
                        break;
                    case 2:
                        account_id = Encoding.UTF8.GetString(blockdata, 16, 16);
                        break;
                    case 3:
                        trophy_id = Encoding.UTF8.GetString(blockdata, 0, 16).Trim('\0');
                        short u1 = BitConverter.ToInt16(blockdata, 16).ChangeEndian();
                        short u2 = BitConverter.ToInt16(blockdata, 18).ChangeEndian();
                        short u3 = BitConverter.ToInt16(blockdata, 20).ChangeEndian();
                        short u4 = BitConverter.ToInt16(blockdata, 22).ChangeEndian();
                        all_trophy_number = BitConverter.ToInt32(blockdata, 24).ChangeEndian();
                        int u5 = BitConverter.ToInt32(blockdata, 28).ChangeEndian();
                        AchievementRate[0] = BitConverter.ToUInt32(blockdata, 64);
                        AchievementRate[1] = BitConverter.ToUInt32(blockdata, 68);
                        AchievementRate[2] = BitConverter.ToUInt32(blockdata, 72);
                        AchievementRate[3] = BitConverter.ToUInt32(blockdata, 76);
                        break;
                    case 4:
                        trophyTypeTable.Add(blockdata.ToStruct<TrophyType>());
                        break;
                    case 5:
                        trophyListInfo = blockdata.ToStruct<TrophyListInfo>();
                        break;
                    case 6:
                        trophyTimeInfoTable.Add(blockdata.ToStruct<TrophyTimeInfo>());
                        break;
                    case 7:// unknow
                        unknowType7 = blockdata.ToStruct<UnknowType7>();
                        // Console.WriteLine("Unsupported block type. (Type{0})", type);
                        break;
                    case 8: // hash
                        unknowHash = blockdata.SubArray(0, 20);
                        break;
                    case 9: // 通常寫著白金獎盃的一些數字，不明
                        // Console.WriteLine("Unsupported block type. (Type{0})", type);
                        break;
                    case 10: // i think it just a padding
                        break;
                }

            } while (TROPUSRReader.BaseStream.Position < TROPUSRReader.BaseStream.Length);

            trophyListInfo.ListLastUpdateTime = DateTime.Now;
            TROPUSRReader.Close();
        }

        public void PrintState()
        {

            Console.WriteLine("Counter: {0}", header.UnknowCount);
            Console.WriteLine("Padding:{0}", header.padding.ToHexString());
            foreach (KeyValuePair<int, TypeRecord> fk in typeRecordTable)
            {
                Console.WriteLine(fk.Value);
            }
            Console.WriteLine("account_id:{0}", account_id);
            Console.WriteLine("列表生成時間:{0}", trophyListInfo.ListCreateTime);
            Console.WriteLine("最後取得獎杯時間:{0}", trophyListInfo.ListLastGetTrophyTime);
            Console.WriteLine("最後更新時間:{0}", trophyListInfo.ListLastUpdateTime);
            Console.WriteLine("取得獎杯數:{0}", trophyListInfo.GetTrophyNumber);
            Console.WriteLine("完成率(尚未解析):{0}", trophyListInfo.AchievementRate[0]);

            for (int i = 0; i < trophyTypeTable.Count; i++)
            {
                Console.WriteLine("SN:{0}, 類型:{1}, 取得:{2}, 取得時間:{3} ", trophyTypeTable[i].SequenceNumber,
                    trophyTypeTable[i].Type, trophyTimeInfoTable[i].IsGet, trophyTimeInfoTable[i].Time);
            }

        }

        public void Save()
        {
            BinaryWriter TROPUSRWriter = new BinaryWriter(new FileStream(path + "TROPUSR.DAT", FileMode.Open));
            TROPUSRWriter.Write(header.StructToBytes());
            TypeRecord account_id_Record = typeRecordTable[2];
            TROPUSRWriter.BaseStream.Position = account_id_Record.Offset + 32; // 空行
            TROPUSRWriter.Write(account_id.ToCharArray()); // 帳號


            TypeRecord trophy_id_Record = typeRecordTable[3];
            TROPUSRWriter.BaseStream.Position = trophy_id_Record.Offset + 16;
            TROPUSRWriter.Write(trophy_id.ToCharArray()); // 獎杯ID
            TROPUSRWriter.BaseStream.Position = trophy_id_Record.Offset + 80;
            //TROPUSRWriter.Write(AchievementRate[0]); // 完成度
            //TROPUSRWriter.Write(AchievementRate[1]); // 完成度
            //TROPUSRWriter.Write(AchievementRate[2]); // 完成度
            //TROPUSRWriter.Write(AchievementRate[3]); // 完成度



            TypeRecord TrophyType_Record = typeRecordTable[4];
            TROPUSRWriter.BaseStream.Position = TrophyType_Record.Offset;
            foreach (TrophyType type in trophyTypeTable)
            {
                TROPUSRWriter.BaseStream.Position += 16;
                TROPUSRWriter.Write(type.StructToBytes());
            }

            TypeRecord trophyListInfo_Record = typeRecordTable[5];
            TROPUSRWriter.BaseStream.Position = trophyListInfo_Record.Offset + 16;
            TROPUSRWriter.Write(trophyListInfo.StructToBytes());


            TypeRecord TrophyTimeInfo_Record = typeRecordTable[6];
            TROPUSRWriter.BaseStream.Position = TrophyTimeInfo_Record.Offset;
            foreach (TrophyTimeInfo time in trophyTimeInfoTable)
            {
                TROPUSRWriter.BaseStream.Position += 16;
                TROPUSRWriter.Write(time.StructToBytes());
            }

            TypeRecord unknowType7_Record = typeRecordTable[7];
            TROPUSRWriter.BaseStream.Position = unknowType7_Record.Offset + 16;
            TROPUSRWriter.Write(unknowType7.StructToBytes());

            TROPUSRWriter.Flush();
            TROPUSRWriter.Close();
        }

        public void UnlockTrophy(int id, DateTime dt)
        {
            TrophyTimeInfo tti = trophyTimeInfoTable[id];
            tti.Time = dt;
            if (!tti.IsGet)
            {
                trophyListInfo.GetTrophyNumber = trophyListInfo.GetTrophyNumber + 1;
                unknowType7.TrophyCount = unknowType7.TrophyCount + 1;
            }
            trophyListInfo.AchievementRate[id / 32] |= (uint)(1 << id).ChangeEndian();
            AchievementRate[id / 32] |= (uint)(1 << id);
            tti.IsGet = true;
            tti.SyncState = 0x100000; //  0x100100 表示已同步
            trophyTimeInfoTable[id] = tti;
            if (dt > trophyListInfo.ListLastGetTrophyTime)
            {
                trophyListInfo.ListLastGetTrophyTime = dt;
                // trophyListInfo.ListLastUpdateTime = dt;
            }
        }

        public void LockTrophy(int id)
        {
            TrophyTimeInfo tti = trophyTimeInfoTable[id];
            if (tti.SyncState == 0x100100)
                throw new Exception("此獎杯已同步過，無法上鎖或修改");

            tti.Time = new DateTime(0);
            if (tti.IsGet)
            {
                trophyListInfo.GetTrophyNumber = trophyListInfo.GetTrophyNumber - 1;
                unknowType7.TrophyCount = unknowType7.TrophyCount - 1;
            }
            trophyListInfo.AchievementRate[id / 32] &= 0xFFFFFFFF ^ (uint)(1 << id).ChangeEndian();
            AchievementRate[id / 32] &= 0xFFFFFFFF ^ (uint)(1 << id);
            tti.IsGet = false;
            tti.SyncState = 0;
            trophyTimeInfoTable[id] = tti;

        }

        #region Structs
        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct Header
        {

            /// long
            public ulong Magic;

            /// int
            public int _unknowCount;
            public int UnknowCount
            {
                get
                {
                    return _unknowCount.ChangeEndian();
                }
            }


            /// byte[36]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 36, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
            public byte[] padding;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct TypeRecord
        {

            /// int
            private int _id;
            public int ID
            {
                get
                {
                    return _id.ChangeEndian();
                }
            }

            /// int
            private int _size;
            public int Size
            {
                get
                {
                    return _size.ChangeEndian();
                }
            }

            /// int
            public int _unknow3;
            public int unknow3
            {
                get
                {
                    return _unknow3.ChangeEndian();
                }
            }

            /// int
            private int _usedTimes;
            public int UsedTimes
            {
                get
                {
                    return _usedTimes.ChangeEndian();
                }
            }

            /// int
            public long _offset;
            public long Offset
            {
                get
                {
                    return _offset.ChangeEndian();
                }
            }

            /// int
            public long _unknow6;
            public long unknow6
            {
                get
                {
                    return _unknow6.ChangeEndian();
                }
            }

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("{ID:").Append(ID).Append(", ");
                sb.Append("Size:").Append(Size).Append(", ");
                sb.Append("u3:").Append(unknow3).Append(", ");
                sb.Append("使用次數:").Append(UsedTimes).Append(", ");
                sb.Append("Offset:").Append(Offset).Append(", ");
                sb.Append("u6:").Append(unknow6).Append("}");
                return sb.ToString();
            }
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct TrophyType
        {

            /// int
            private int _sequenceNumber;
            public int SequenceNumber
            {
                get
                {
                    return _sequenceNumber.ChangeEndian();
                }
            }

            /// int
            private int _type;
            public int Type
            {
                get
                {
                    return _type.ChangeEndian();
                }
                set
                {
                    _type = value.ChangeEndian();
                }
            }

            /// byte[16]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 16, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
            public byte[] unknow;

            /// byte[56]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 56, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
            public byte[] padding;

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("[").Append("SequenceNumber:").Append(SequenceNumber).Append(", ");
                sb.Append("Type:").Append(Type).Append("]");
                return sb.ToString();
            }
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct TrophyListInfo
        {

            /// byte[16]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 16, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
            public byte[] padding;

            /// byte[16]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 16, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
            private byte[] _listCreateTime;
            public DateTime ListCreateTime
            {
                get
                {
                    DateTime realDateTime = new DateTime(BitConverter.ToInt64(_listCreateTime, 0).ChangeEndian() * 10);
                    if (realDateTime.Ticks == 0)
                    {
                        return realDateTime;
                    }
                    else
                    {
                        return realDateTime.AddHours(TimeZoneInfo.Local.BaseUtcOffset.Hours);
                    }
                }
                set
                {
                    if (value.Ticks == 0)
                    {
                        Array.Clear(_listCreateTime, 0, 16);
                    }
                    else
                    {
                        long temp = value.AddHours(-TimeZoneInfo.Local.BaseUtcOffset.Hours).Ticks;
                        Array.Copy(BitConverter.GetBytes((temp / 10).ChangeEndian()), 0, _listCreateTime, 0, 8);
                        Array.Copy(BitConverter.GetBytes((temp / 10).ChangeEndian()), 0, _listCreateTime, 8, 8);
                    }
                }
            }

            /// byte[16]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 16, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
            private byte[] _listLastUpdateTime;
            public DateTime ListLastUpdateTime
            {
                get
                {
                    DateTime realDateTime = new DateTime(BitConverter.ToInt64(_listLastUpdateTime, 0).ChangeEndian() * 10);
                    if (realDateTime.Ticks == 0)
                    {
                        return realDateTime;
                    }
                    else
                    {
                        return realDateTime.AddHours(TimeZoneInfo.Local.BaseUtcOffset.Hours);
                    }
                }
                set
                {
                    if (value.Ticks == 0)
                    {
                        Array.Clear(_listLastUpdateTime, 0, 16);
                    }
                    else
                    {
                        long temp = value.AddHours(-TimeZoneInfo.Local.BaseUtcOffset.Hours).Ticks;
                        Array.Copy(BitConverter.GetBytes((temp / 10).ChangeEndian()), 0, _listLastUpdateTime, 0, 8);
                        Array.Copy(BitConverter.GetBytes((temp / 10).ChangeEndian()), 0, _listLastUpdateTime, 8, 8);
                    }
                }
            }

            /// byte[16]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 16, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
            public byte[] padding2;

            /// byte[16]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 16, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
            private byte[] _listLastGetTrophyTime;
            public DateTime ListLastGetTrophyTime
            {
                get
                {
                    DateTime realDateTime = new DateTime(BitConverter.ToInt64(_listLastGetTrophyTime, 0).ChangeEndian() * 10);
                    if (realDateTime.Ticks == 0)
                    {
                        return realDateTime;
                    }
                    else
                    {
                        return realDateTime.AddHours(TimeZoneInfo.Local.BaseUtcOffset.Hours);
                    }
                }
                set
                {
                    if (value.Ticks == 0)
                    {
                        Array.Clear(_listLastGetTrophyTime, 0, 16);
                    }
                    else
                    {
                        long temp = value.AddHours(-TimeZoneInfo.Local.BaseUtcOffset.Hours).Ticks;
                        Array.Copy(BitConverter.GetBytes((temp / 10).ChangeEndian()), 0, _listLastGetTrophyTime, 0, 8);
                        Array.Copy(BitConverter.GetBytes((temp / 10).ChangeEndian()), 0, _listLastGetTrophyTime, 8, 8);
                    }
                }
            }

            /// byte[32]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 32, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
            public byte[] padding3;

            /// int
            private int _getTrophyNumber;
            public int GetTrophyNumber
            {
                get
                {
                    return _getTrophyNumber.ChangeEndian();
                }
                set
                {
                    _getTrophyNumber = value.ChangeEndian();
                }
            }

            /// byte[12]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 12, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
            public byte[] padding4;

            /// uint[4]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I4)]
            public uint[] AchievementRate;

            /// byte[16]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 16, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
            public byte[] padding5;

            /// byte[16]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 16, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
            public byte[] hash;

            /// byte[32]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 32, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
            public byte[] padding6;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct TrophyTimeInfo
        {

            /// int
            private int _sequenceNumber;
            public int SequenceNumber
            {
                get
                {
                    return _sequenceNumber.ChangeEndian();
                }
            }

            /// byte[4]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
            private byte[] _getOrNot;
            public bool IsGet
            {
                get
                {
                    return (_getOrNot[3] != 0) ? true : false;
                }
                set
                {
                    _getOrNot[3] = (byte)((value) ? 1 : 0);
                }
            }

            /// int
            public int SyncState;

            /// int
            public int unknow2;

            /// byte[16]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 16, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
            private byte[] _getTime;
            public DateTime Time
            {
                get
                {
                    DateTime realDateTime = new DateTime(BitConverter.ToInt64(_getTime, 0).ChangeEndian() * 10);
                    if (realDateTime.Ticks == 0)
                    {
                        return realDateTime;
                    }
                    else
                    {
                        return realDateTime.AddHours(TimeZoneInfo.Local.BaseUtcOffset.Hours);
                    }
                }
                set
                {
                    if (value.Ticks == 0)
                    {
                        Array.Clear(_getTime, 0, 16);
                    }
                    else
                    {
                        long temp = value.AddHours(-TimeZoneInfo.Local.BaseUtcOffset.Hours).Ticks;
                        Array.Copy(BitConverter.GetBytes((temp / 10).ChangeEndian()), 0, _getTime, 0, 8);
                        Array.Copy(BitConverter.GetBytes((temp / 10).ChangeEndian()), 0, _getTime, 8, 8);
                    }
                }
            }

            /// byte[64]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 64, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
            public byte[] padding;

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("[").Append("SequenceNumber:").Append(SequenceNumber).Append(", ");
                sb.Append("GetOrNot:").Append(IsGet).Append(", ");
                sb.Append("GetTime:").Append(Time).Append("]");

                return sb.ToString();
            }
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct UnknowType7
        {

            /// int
            private int _getTrophyCount;
            public int TrophyCount
            {
                get
                {
                    return _getTrophyCount.ChangeEndian();
                }
                set
                {
                    _getTrophyCount = value.ChangeEndian();
                }
            }

            /// int
            private int _syncTrophyCount;

            /// int
            public int u3;

            /// int
            public int u4;

            /// byte[8]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 8, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
            private byte[] _lastSyncTime;
            public DateTime ListSyncTime
            {
                get
                {
                    DateTime realDateTime = new DateTime(BitConverter.ToInt64(_lastSyncTime, 0).ChangeEndian() * 10);
                    if (realDateTime.Ticks == 0)
                    {
                        return realDateTime;
                    }
                    else
                    {
                        return realDateTime.AddHours(TimeZoneInfo.Local.BaseUtcOffset.Hours);
                    }
                }
                set
                {
                    if (value.Ticks == 0)
                    {
                        Array.Clear(_lastSyncTime, 0, 8);
                    }
                    else
                    {
                        long temp = value.AddHours(-TimeZoneInfo.Local.BaseUtcOffset.Hours).Ticks;
                        Array.Copy(BitConverter.GetBytes((temp / 10).ChangeEndian()), 0, _lastSyncTime, 0, 8);
                    }
                }
            }


            /// byte[8]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 8, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
            public byte[] padding;

            /// byte[48]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 48, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
            public byte[] padding2;
        }

        #endregion

    }
}