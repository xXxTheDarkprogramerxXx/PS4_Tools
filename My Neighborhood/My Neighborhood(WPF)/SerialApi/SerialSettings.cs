using System;
using System.ComponentModel;
using System.IO.Ports;

namespace My_Neighborhood_WPF_.SerialApi
{
    internal sealed class SerialSettings : INotifyPropertyChanged
    {
        // Token: 0x14000002 RID: 2
        // (add) Token: 0x06000031 RID: 49 RVA: 0x00003854 File Offset: 0x00001A54
        // (remove) Token: 0x06000032 RID: 50 RVA: 0x0000388C File Offset: 0x00001A8C
        public event PropertyChangedEventHandler PropertyChanged;

        // Token: 0x17000003 RID: 3
        // (get) Token: 0x06000033 RID: 51 RVA: 0x000038C1 File Offset: 0x00001AC1
        // (set) Token: 0x06000034 RID: 52 RVA: 0x000038C9 File Offset: 0x00001AC9
        public string PortName
        {
            get
            {
                return this._port;
            }
            set
            {
                if (this._port.Equals(value))
                {
                    return;
                }
                this._port = value;
                this.SendPropertyChangedEvent("PortName");
            }
        }

        // Token: 0x17000004 RID: 4
        // (get) Token: 0x06000035 RID: 53 RVA: 0x000038EC File Offset: 0x00001AEC
        // (set) Token: 0x06000036 RID: 54 RVA: 0x000038F4 File Offset: 0x00001AF4
        public int BaudRate
        {
            get
            {
                return this._baud;
            }
            set
            {
                if (this._baud == value)
                {
                    return;
                }
                this._baud = value;
                this.SendPropertyChangedEvent("BaudRate");
            }
        }

        // Token: 0x17000005 RID: 5
        // (get) Token: 0x06000037 RID: 55 RVA: 0x00003912 File Offset: 0x00001B12
        // (set) Token: 0x06000038 RID: 56 RVA: 0x0000391A File Offset: 0x00001B1A
        public Parity Parity
        {
            get
            {
                return this._parity;
            }
            set
            {
                if (this._parity == value)
                {
                    return;
                }
                this._parity = value;
                this.SendPropertyChangedEvent("Parity");
            }
        }

        // Token: 0x17000006 RID: 6
        // (get) Token: 0x06000039 RID: 57 RVA: 0x00003938 File Offset: 0x00001B38
        // (set) Token: 0x0600003A RID: 58 RVA: 0x00003940 File Offset: 0x00001B40
        public int DataBits
        {
            get
            {
                return this._bits;
            }
            set
            {
                if (this._bits == value)
                {
                    return;
                }
                this._bits = value;
                this.SendPropertyChangedEvent("DataBits");
            }
        }

        // Token: 0x17000007 RID: 7
        // (get) Token: 0x0600003B RID: 59 RVA: 0x0000395E File Offset: 0x00001B5E
        // (set) Token: 0x0600003C RID: 60 RVA: 0x00003966 File Offset: 0x00001B66
        public StopBits StopBits
        {
            get
            {
                return this._stop;
            }
            set
            {
                if (this._stop == value)
                {
                    return;
                }
                this._stop = value;
                this.SendPropertyChangedEvent("StopBits");
            }
        }

        // Token: 0x17000008 RID: 8
        // (get) Token: 0x0600003D RID: 61 RVA: 0x00003984 File Offset: 0x00001B84
        // (set) Token: 0x0600003E RID: 62 RVA: 0x0000398C File Offset: 0x00001B8C
        public string[] PortNameCollection { get; set; }

        // Token: 0x17000009 RID: 9
        // (get) Token: 0x0600003F RID: 63 RVA: 0x00003995 File Offset: 0x00001B95
        // (set) Token: 0x06000040 RID: 64 RVA: 0x0000399D File Offset: 0x00001B9D
        private BindingList<int> BaudRateCollection { get; set; }

        // Token: 0x1700000A RID: 10
        // (get) Token: 0x06000041 RID: 65 RVA: 0x000039A6 File Offset: 0x00001BA6
        // (set) Token: 0x06000042 RID: 66 RVA: 0x000039AE File Offset: 0x00001BAE
        private int[] DataBitsCollection { get; set; }

        // Token: 0x06000043 RID: 67 RVA: 0x000039B8 File Offset: 0x00001BB8
        public SerialSettings()
        {
            this.DataBitsCollection = new int[]
            {
                5,
                6,
                7,
                8
            };
            this.BaudRateCollection = new BindingList<int>();
        }

        // Token: 0x06000044 RID: 68 RVA: 0x00003A14 File Offset: 0x00001C14
        public void UpdateBaudRateCollection(int possibleBaudRates)
        {
            this.BaudRateCollection.Clear();
            if ((possibleBaudRates & 1) > 0)
            {
                this.BaudRateCollection.Add(75);
            }
            if ((possibleBaudRates & 2) > 0)
            {
                this.BaudRateCollection.Add(110);
            }
            if ((possibleBaudRates & 8) > 0)
            {
                this.BaudRateCollection.Add(150);
            }
            if ((possibleBaudRates & 16) > 0)
            {
                this.BaudRateCollection.Add(300);
            }
            if ((possibleBaudRates & 32) > 0)
            {
                this.BaudRateCollection.Add(600);
            }
            if ((possibleBaudRates & 64) > 0)
            {
                this.BaudRateCollection.Add(1200);
            }
            if ((possibleBaudRates & 128) > 0)
            {
                this.BaudRateCollection.Add(1800);
            }
            if ((possibleBaudRates & 256) > 0)
            {
                this.BaudRateCollection.Add(2400);
            }
            if ((possibleBaudRates & 512) > 0)
            {
                this.BaudRateCollection.Add(4800);
            }
            if ((possibleBaudRates & 1024) > 0)
            {
                this.BaudRateCollection.Add(7200);
            }
            if ((possibleBaudRates & 2048) > 0)
            {
                this.BaudRateCollection.Add(9600);
            }
            if ((possibleBaudRates & 4096) > 0)
            {
                this.BaudRateCollection.Add(14400);
            }
            if ((possibleBaudRates & 8192) > 0)
            {
                this.BaudRateCollection.Add(19200);
            }
            if ((possibleBaudRates & 16384) > 0)
            {
                this.BaudRateCollection.Add(38400);
            }
            if ((possibleBaudRates & 32768) > 0)
            {
                this.BaudRateCollection.Add(56000);
            }
            if ((possibleBaudRates & 262144) > 0)
            {
                this.BaudRateCollection.Add(57600);
            }
            if ((possibleBaudRates & 131072) > 0)
            {
                this.BaudRateCollection.Add(115200);
            }
            if ((possibleBaudRates & 65536) > 0)
            {
                this.BaudRateCollection.Add(128000);
            }
            this.SendPropertyChangedEvent("BaudRateCollection");
        }

        // Token: 0x06000045 RID: 69 RVA: 0x00003BF0 File Offset: 0x00001DF0
        private void SendPropertyChangedEvent(string propertyName)
        {
            PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if (propertyChanged == null)
            {
                return;
            }
            propertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        // Token: 0x04000028 RID: 40
        private int _baud = 115200;

        // Token: 0x04000029 RID: 41
        private int _bits = 8;

        // Token: 0x0400002A RID: 42
        private Parity _parity;

        // Token: 0x0400002B RID: 43
        private string _port = "";

        // Token: 0x0400002C RID: 44
        private StopBits _stop = StopBits.One;
    }
}