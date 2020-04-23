using PS4_Tools.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace PS4_Tools.TROPHY
{
    public class TROPTRNS
    {
        string path;
        Header header;
        Dictionary<int, TypeRecord> typeRecordTable;
        List<TrophyInfo> trophyInfoTable = new List<TrophyInfo>();
        public string account_id;
        public string trophy_id;
        int u1;
        int AllGetTrophysCount;
        int AllSyncPSNTrophyCount;
        TrophyInitTime trophyInitTime;

        public TROPTRNS(string path_in)
        {

            this.path = path_in;
            BinaryReader TROPTRNSReader = null;
            try
            {

                if (path == null)
                    throw new Exception("Path cannot be null!");

                if (!path.EndsWith(@"\"))
                    path += @"\";

                if (!File.Exists(path + "TROPTRNS.DAT"))
                    throw new Exception("Cannot find TROPTRNS.DAT.");

                try
                {
                    TROPTRNSReader = new BinaryReader(new FileStream(path + "TROPTRNS.DAT", FileMode.Open));
                }
                catch (IOException)
                {
                    throw new Exception("Cannot Open TROPTRNS.DAT.");
                }

                header = TROPTRNSReader.ReadBytes(Marshal.SizeOf(typeof(Header))).ToStruct<Header>();
                if (header.Magic != 0x0000000100ad548f81)
                {
                    TROPTRNSReader.Close();
                    throw new Exception("Not a vaild TROPTRNS.DAT.");
                }


                typeRecordTable = new Dictionary<int, TypeRecord>();
                for (int i = 0; i < header.UnknowCount; i++)
                {
                    TypeRecord TypeRecordTmp = TROPTRNSReader.ReadBytes(Marshal.SizeOf(typeof(TypeRecord))).ToStruct<TypeRecord>();
                    typeRecordTable.Add(TypeRecordTmp.ID, TypeRecordTmp);
                }

                // Type 2
                TypeRecord account_id_Record = typeRecordTable[2];
                TROPTRNSReader.BaseStream.Position = account_id_Record.Offset + 32; // 空行
                account_id = Encoding.UTF8.GetString(TROPTRNSReader.ReadBytes(16));

                // Type 3
                TypeRecord trophy_id_Record = typeRecordTable[3];
                TROPTRNSReader.BaseStream.Position = trophy_id_Record.Offset + 16; // 空行
                trophy_id = Encoding.UTF8.GetString(TROPTRNSReader.ReadBytes(16)).Trim('\0');
                u1 = TROPTRNSReader.ReadInt32(); // always 00000090
                AllGetTrophysCount = TROPTRNSReader.ReadInt32();
                AllSyncPSNTrophyCount = TROPTRNSReader.ReadInt32();

                // Type 4
                TypeRecord TrophyInfoRecord = typeRecordTable[4];
                TROPTRNSReader.BaseStream.Position = TrophyInfoRecord.Offset; // 空行
                int type = TROPTRNSReader.ReadInt32();
                int blocksize = TROPTRNSReader.ReadInt32();
                int sequenceNumber = TROPTRNSReader.ReadInt32(); // if have more than same type block, it will be used
                int unknow = TROPTRNSReader.ReadInt32();
                byte[] blockdata = TROPTRNSReader.ReadBytes(blocksize);
                trophyInitTime = blockdata.ToStruct<TrophyInitTime>();


                for (int i = 0; i < (AllGetTrophysCount - 1); i++)
                {
                    TROPTRNSReader.BaseStream.Position += 16;
                    TrophyInfo ti = TROPTRNSReader.ReadBytes(blocksize).ToStruct<TrophyInfo>();
                    trophyInfoTable.Add(ti);
                }
            }
            finally
            {
                if (TROPTRNSReader != null) TROPTRNSReader.Close();
            }
        }

        public TROPTRNS(byte[] TROPTRNSFile)
        {

           
            BinaryReader TROPTRNSReader = null;
            try
            {

              

                try
                {
                    TROPTRNSReader = new BinaryReader(new MemoryStream(TROPTRNSFile));
                }
                catch (IOException)
                {
                    throw new Exception("Cannot Open TROPTRNS.DAT.");
                }

                header = TROPTRNSReader.ReadBytes(Marshal.SizeOf(typeof(Header))).ToStruct<Header>();
                if (header.Magic != 0x0000000100ad548f81)
                {
                    TROPTRNSReader.Close();
                    throw new Exception("Not a vaild TROPTRNS.DAT.");
                }


                typeRecordTable = new Dictionary<int, TypeRecord>();
                for (int i = 0; i < header.UnknowCount; i++)
                {
                    TypeRecord TypeRecordTmp = TROPTRNSReader.ReadBytes(Marshal.SizeOf(typeof(TypeRecord))).ToStruct<TypeRecord>();
                    typeRecordTable.Add(TypeRecordTmp.ID, TypeRecordTmp);
                }

                // Type 2
                TypeRecord account_id_Record = typeRecordTable[2];
                TROPTRNSReader.BaseStream.Position = account_id_Record.Offset + 32; // 空行
                account_id = Encoding.UTF8.GetString(TROPTRNSReader.ReadBytes(16));

                // Type 3
                TypeRecord trophy_id_Record = typeRecordTable[3];
                TROPTRNSReader.BaseStream.Position = trophy_id_Record.Offset + 16; // 空行
                trophy_id = Encoding.UTF8.GetString(TROPTRNSReader.ReadBytes(16)).Trim('\0');
                u1 = TROPTRNSReader.ReadInt32(); // always 00000090
                AllGetTrophysCount = TROPTRNSReader.ReadInt32();
                AllSyncPSNTrophyCount = TROPTRNSReader.ReadInt32();

                // Type 4
                TypeRecord TrophyInfoRecord = typeRecordTable[4];
                TROPTRNSReader.BaseStream.Position = TrophyInfoRecord.Offset; // 空行
                int type = TROPTRNSReader.ReadInt32();
                int blocksize = TROPTRNSReader.ReadInt32();
                int sequenceNumber = TROPTRNSReader.ReadInt32(); // if have more than same type block, it will be used
                int unknow = TROPTRNSReader.ReadInt32();
                byte[] blockdata = TROPTRNSReader.ReadBytes(blocksize);
                trophyInitTime = blockdata.ToStruct<TrophyInitTime>();


                for (int i = 0; i < (AllGetTrophysCount - 1); i++)
                {
                    TROPTRNSReader.BaseStream.Position += 16;
                    TrophyInfo ti = TROPTRNSReader.ReadBytes(blocksize).ToStruct<TrophyInfo>();
                    trophyInfoTable.Add(ti);
                }
            }
            finally
            {
                if (TROPTRNSReader != null) TROPTRNSReader.Close();
            }
        }

        public void PrintState()
        {
            Console.WriteLine("AllGetTrophysCount:{0}", AllGetTrophysCount);
            Console.WriteLine("Counter: {0}", header.UnknowCount);
            Console.WriteLine("Padding:{0}", header.padding.ToHexString());
            foreach (KeyValuePair<int, TypeRecord> fk in typeRecordTable)
            {
                Console.WriteLine(fk.Value);
            }
            Console.WriteLine("account_id:{0}", account_id);
            Console.WriteLine("trophy_id:{0}", trophy_id);

            Console.WriteLine("Geted Trophys:{0} Sync Trophys:{1} ", AllGetTrophysCount, AllSyncPSNTrophyCount);


            for (int i = 0; i < trophyInfoTable.Count; i++)
            {
                Console.WriteLine("SN:{0}, Trophy ID:{1}, 類型:{2}, 存在:{3}, 取得時間:{4}, 同步:{5} ",
                    trophyInfoTable[i].SequenceNumber, trophyInfoTable[i].TrophyID,
                    trophyInfoTable[i].TrophyType, trophyInfoTable[i].IsExist, trophyInfoTable[i].Time,
                    trophyInfoTable[i].IsSync
                   );
            }

        }

        public void Save()
        {
            BinaryWriter TROPTRNSWriter = new BinaryWriter(new FileStream(path + "TROPTRNS.DAT", FileMode.Open));
            TROPTRNSWriter.Write(header.StructToBytes());
            TypeRecord account_id_Record = typeRecordTable[2];
            TROPTRNSWriter.BaseStream.Position = account_id_Record.Offset + 32; // 空行
            TROPTRNSWriter.Write(account_id.ToCharArray());

            TypeRecord trophy_id_Record = typeRecordTable[3];
            TROPTRNSWriter.BaseStream.Position = trophy_id_Record.Offset + 16; // 空行
            TROPTRNSWriter.Write(trophy_id.ToCharArray());
            TROPTRNSWriter.BaseStream.Position = trophy_id_Record.Offset + 32; // 字串長度不定，直接跳過
            TROPTRNSWriter.Write(u1);
            Console.WriteLine(trophyInfoTable.Count);
            TROPTRNSWriter.Write(trophyInfoTable.Count + 1); // AllGetTrophysCount
            AllSyncPSNTrophyCount = 0;
            for (int i = 0; i < trophyInfoTable.Count; i++)
            {
                if (trophyInfoTable[i].IsSync)
                {
                    AllSyncPSNTrophyCount++;
                }
            }
            // AllSyncPSNTrophyCount++;
            TROPTRNSWriter.Write(AllSyncPSNTrophyCount + 1);

            // Type 4
            TypeRecord TrophyType_Record = typeRecordTable[4];
            TROPTRNSWriter.BaseStream.Position = TrophyType_Record.Offset;
            TROPTRNSWriter.BaseStream.Position += 16;
            TROPTRNSWriter.Write(trophyInitTime.StructToBytes());


            for (int i = 0; i < trophyInfoTable.Count; i++)
            {
                TROPTRNSWriter.BaseStream.Position += 16;
                TrophyInfo ti = trophyInfoTable[i];
                ti.SequenceNumber = i + 1; // 整理順序
                //if (i == 0) {
                //    ti._unknowInt2 = 0x100000;
                //} else {
                //    ti._unknowInt2 = 0x100000;
                //}
                ti._unknowInt3 = 0;

                trophyInfoTable[i] = ti;
                TROPTRNSWriter.Write(trophyInfoTable[i].StructToBytes());
            }

            byte[] emptyStruct = new byte[Marshal.SizeOf(typeof(TrophyInfo))];
            Array.Clear(emptyStruct, 0, emptyStruct.Length);
            TrophyInfo emptyTrophyInfo = emptyStruct.ToStruct<TrophyInfo>();
            for (int i = trophyInfoTable.Count; i < TrophyType_Record.Size; i++)
            {
                TROPTRNSWriter.BaseStream.Position += 16;
                emptyTrophyInfo.SequenceNumber = i + 1;
                TROPTRNSWriter.Write(emptyTrophyInfo.StructToBytes());
            }


            TROPTRNSWriter.Flush();
            TROPTRNSWriter.Close();
        }

        public void PutTrophy(int id, int TrophyType, DateTime dt)
        {
            TrophyInfo ti = new TrophyInfo(id, TrophyType, dt);
            foreach (TrophyInfo titmp in trophyInfoTable)
            {
                if (titmp.TrophyID == id)
                {
                    throw new Exception("請勿重複取得獎杯。");
                }
            }

            int insertPoint = 0;
            for (insertPoint = 0; insertPoint < trophyInfoTable.Count; insertPoint++)
            {
                if (DateTime.Compare(trophyInfoTable[insertPoint].Time, dt) > 0)
                {
                    if (trophyInfoTable[insertPoint].IsSync)
                    {
                        throw new Exception("該時間點已做過PSN同步，請設定更晚的時間試試。");
                    }
                    break;
                }
            }
            trophyInfoTable.Insert(insertPoint, ti);
            // Console.WriteLine(trophyInfoTable.Count);
            AllGetTrophysCount++;
        }

        public TrophyInfo? PopTrophy()
        {
            if (trophyInfoTable.Count == 0)
                return null;
            TrophyInfo pop = trophyInfoTable[trophyInfoTable.Count - 1];
            if (pop.IsSync)
            {
                return null;
            }
            else
            {
                trophyInfoTable.RemoveAt(trophyInfoTable.Count - 1);
                AllGetTrophysCount--;
            }
            return pop;
        }

        public void ChangeTime(int id, DateTime dt)
        {
            Nullable<TrophyInfo> ti = null;
            int originalIndex = 1;
            for (originalIndex = 0; originalIndex < trophyInfoTable.Count; originalIndex++)
            {
                if (trophyInfoTable[originalIndex].TrophyID == id)
                {
                    if (trophyInfoTable[originalIndex].IsSync)
                    {
                        throw new Exception("該獎杯已同步於PSN，為避免錯誤，請勿修改。");
                    }
                    else
                    {
                        ti = trophyInfoTable[originalIndex];
                        trophyInfoTable.RemoveAt(originalIndex);
                        break;
                    }
                }
            }

            if (ti == null)
            {
                throw new Exception("無此獎杯ID，請先新增獎杯再修改時間。");
            }


            for (int i = 0; i < trophyInfoTable.Count; i++)
            {
                if (DateTime.Compare(trophyInfoTable[i].Time, dt) > 0)
                {
                    if (trophyInfoTable[i].IsSync)
                    {
                        trophyInfoTable.Insert(originalIndex, (TrophyInfo)ti);
                        throw new Exception("該時間點已做過PSN同步，請設定更晚的時間試試。");
                    }
                    else
                    {
                        TrophyInfo titmp = (TrophyInfo)ti;
                        titmp.Time = dt;
                        trophyInfoTable.Insert(i, titmp);
                        ti = null;
                    }
                    break;
                }
            } // Insert Into Table

            if (ti != null)
            {
                TrophyInfo titmp = (TrophyInfo)ti;
                titmp.Time = dt;
                trophyInfoTable.Add(titmp);
            }

        }

        public void DeleteTrophyByID(int id)
        {
            for (int i = 0; i < trophyInfoTable.Count; i++)
            {
                if (trophyInfoTable[i].TrophyID == id)
                {
                    if (trophyInfoTable[i].IsSync)
                    {
                        throw new Exception("該獎杯已同步於PSN，為避免錯誤，請勿刪除。");
                    }
                    else
                    {
                        trophyInfoTable.RemoveAt(i);
                        AllGetTrophysCount--;
                    }
                }
            }
        }

        public TrophyInfo? this[int TrophyID]
        {
            get
            {
                TrophyInfo? ret = null;
                for (int i = 0; i < trophyInfoTable.Count; i++)
                {
                    if (trophyInfoTable[i].TrophyID == TrophyID)
                    {
                        ret = trophyInfoTable[i];
                        break;
                    }
                }
                return ret;
            }
        }

        public int Count
        {
            get
            {
                return trophyInfoTable.Count;
            }
        }

        public DateTime GetLastTrophyTime()
        {
            return trophyInfoTable[trophyInfoTable.Count - 1].Time;
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
        public struct TrophyInfo
        {

            /// int
            private int _sequenceNumber;
            public int SequenceNumber
            {
                get
                {
                    return _sequenceNumber.ChangeEndian();
                }
                set
                {
                    _sequenceNumber = value.ChangeEndian();
                }
            }

            /// byte[4]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
            private byte[] _isExist;
            public bool IsExist
            {
                set
                {
                    _isExist[3] = (byte)((value) ? 2 : 0);
                }
                get
                {
                    return (_isExist[3] == 2) ? true : false;
                }
            }


            /// byte[4]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
            private byte[] _syncState;
            public bool IsSync
            {
                set
                {
                    _syncState[3] = (byte)((value) ? 1 : 0);
                }
                get
                {
                    return (_syncState[3] == 0) ? false : true;
                }
            }

            /// int
            public int _unknowInt1;

            /// byte[16]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 16, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
            public byte[] padding;


            /// int
            private int _trophyID;
            public int TrophyID
            {
                get
                {
                    return _trophyID.ChangeEndian();
                }
                set
                {
                    _trophyID = value.ChangeEndian();
                }
            }

            /// int
            private int _trophyType;
            public int TrophyType
            {
                get
                {
                    return _trophyType.ChangeEndian();
                }
                set
                {
                    _trophyType = value.ChangeEndian();
                }
            }


            /// int
            public int _unknowInt2;

            /// int
            public int _unknowInt3;

            /// byte[16]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 16, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
            private byte[] _getTime;
            public DateTime Time
            {
                get
                {
                    DateTime dt = new DateTime(BitConverter.ToInt64(_getTime, 0).ChangeEndian() * 10);
                    return dt.AddHours(TimeZoneInfo.Local.BaseUtcOffset.Hours);
                }
                set
                {
                    if (value.Ticks == 0)
                    {
                        Array.Clear(_getTime, 0, 16);
                    }
                    else
                    {
                        long tmp = value.AddHours(-TimeZoneInfo.Local.BaseUtcOffset.Hours).Ticks;
                        Array.Copy(BitConverter.GetBytes((tmp / 10).ChangeEndian()), 0, _getTime, 0, 8);
                        Array.Copy(BitConverter.GetBytes((tmp / 10).ChangeEndian()), 0, _getTime, 8, 8);
                    }
                }
            }

            /// byte[96]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 96, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
            public byte[] padding2;

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("[").Append("SequenceNumber:").Append(SequenceNumber).Append(", ");
                sb.Append("GetState:").Append(IsExist).Append("]");
                return sb.ToString();
            }

            public TrophyInfo(int id, int TrophyType, DateTime dt)
            {
                _sequenceNumber = 0;
                _isExist = new byte[4];
                _getTime = new byte[16];
                _syncState = new byte[4];
                _trophyID = id.ChangeEndian();
                _trophyType = TrophyType.ChangeEndian();
                _unknowInt1 = 0;
                _unknowInt2 = 0x00100000;
                _unknowInt3 = 0;
                padding = new byte[16];
                padding2 = new byte[96];
                Time = dt;
                IsExist = true;
            }
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct TrophyInitTime
        {

            /// int
            public int u1;

            /// int
            public int u2;

            /// int
            public int u3;

            /// int
            public int u4;

            /// byte[16]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 16, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
            public byte[] padding;

            /// byte[16]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 16, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
            public byte[] _initTime;

            /// byte[112]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 112, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
            public byte[] padding2;
        }

        #endregion
    }
}