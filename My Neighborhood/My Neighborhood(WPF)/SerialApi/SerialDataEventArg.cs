using System;

namespace My_Neighborhood_WPF_.SerialApi
{
    internal sealed class SerialDataEventArgs : EventArgs
    {
        // Token: 0x17000001 RID: 1
        // (get) Token: 0x06000020 RID: 32 RVA: 0x000034A7 File Offset: 0x000016A7
        public byte[] Data
        {
            get
            {
                return this._data;
            }
        }

        // Token: 0x06000021 RID: 33 RVA: 0x000034AF File Offset: 0x000016AF
        public SerialDataEventArgs(byte[] dataInByteArray)
        {
            this._data = dataInByteArray;
        }

        // Token: 0x04000024 RID: 36
        private readonly byte[] _data;
    }
}
