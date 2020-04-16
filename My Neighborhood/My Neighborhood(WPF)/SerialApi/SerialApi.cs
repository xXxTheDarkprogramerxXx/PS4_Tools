using System;
using System.ComponentModel;
using System.IO.Ports;
using System.Reflection;

namespace My_Neighborhood_WPF_.SerialApi
{
    internal sealed class SerialMonitor
    {
        public event EventHandler<SerialDataEventArgs> NewSerialDataRecieved;

        public SerialSettings Settings { get; private set; }
        public SerialMonitor()
        {
            this.UpdatePortList();
        }
        ~SerialMonitor()
        {
            this.Dispose(false);
        }

        private void Dispose(bool disposing = true)
        {
            if (disposing)
            {
                this._port.DataReceived -= this.SerialPortDataReceived;
            }
            if (this._port == null)
            {
                return;
            }
            if (this._port.IsOpen)
            {
                this._port.Close();
            }
            this._port.Dispose();
        }

        // Token: 0x06000029 RID: 41 RVA: 0x000035D0 File Offset: 0x000017D0
        private void UpdatePortList()
        {
            this.Settings = new SerialSettings
            {
                PortNameCollection = SerialPort.GetPortNames()
            };
            this.Settings.PropertyChanged += this.SettingsPropertyChanged;
            if (this.Settings.PortNameCollection.Length != 0)
            {
                this.Settings.PortName = this.Settings.PortNameCollection[0];
            }
        }

        // Token: 0x0600002A RID: 42 RVA: 0x00003630 File Offset: 0x00001830
        private void SettingsPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("PortName"))
            {
                this.UpdateBaudRateCollection();
            }
        }

        // Token: 0x0600002B RID: 43 RVA: 0x0000364A File Offset: 0x0000184A
        public void StopListening()
        {
            if (this._port != null && this._port.IsOpen)
            {
                this._port.Close();
            }
        }

        // Token: 0x0600002C RID: 44 RVA: 0x0000366C File Offset: 0x0000186C
        public bool IsListening()
        {
            return this._port.IsOpen;
        }

        // Token: 0x0600002D RID: 45 RVA: 0x0000367C File Offset: 0x0000187C
        public bool StartListening()
        {
            bool result;
            try
            {
                if (string.IsNullOrEmpty(this.Settings.PortName))
                {
                    result = false;
                }
                else
                {
                    if (this._port != null && this._port.IsOpen)
                    {
                        this._port.Close();
                    }
                    this._port = new SerialPort(this.Settings.PortName, this.Settings.BaudRate, this.Settings.Parity, this.Settings.DataBits, this.Settings.StopBits);
                    this._port.DataReceived += this.SerialPortDataReceived;
                    this._port.Open();
                    result = this.IsListening();
                }
            }
            catch
            {
                result = false;
            }
            return result;
        }

        // Token: 0x0600002E RID: 46 RVA: 0x00003748 File Offset: 0x00001948
        private void SerialPortDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int bytesToRead = this._port.BytesToRead;
            byte[] array = new byte[bytesToRead];
            if (this._port.Read(array, 0, bytesToRead) == 0)
            {
                return;
            }
            EventHandler<SerialDataEventArgs> newSerialDataRecieved = this.NewSerialDataRecieved;
            if (newSerialDataRecieved == null)
            {
                return;
            }
            newSerialDataRecieved(this, new SerialDataEventArgs(array));
        }

        // Token: 0x0600002F RID: 47 RVA: 0x00003790 File Offset: 0x00001990
        private void UpdateBaudRateCollection()
        {
            try
            {
                this._port = new SerialPort(this.Settings.PortName);
                this._port.Open();
                FieldInfo field = this._port.BaseStream.GetType().GetField("commProp", BindingFlags.Instance | BindingFlags.NonPublic);
                if (field == null)
                {
                    return;
                }
                object value = field.GetValue(this._port.BaseStream);
                field = value.GetType().GetField("dwSettableBaud", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (field == null)
                {
                    return;
                }
                int possibleBaudRates = (int)field.GetValue(value);
                this._port.Close();
                this.Settings.UpdateBaudRateCollection(possibleBaudRates);
            }
            catch(Exception ex)
            {
                //probabbly could not open port or something
               // throw new Exception(ex.Message);
            }
        }

        // Token: 0x06000030 RID: 48 RVA: 0x00003838 File Offset: 0x00001A38
        public void SetSpeed(bool lowspeed)
        {
            this.Settings.BaudRate = (lowspeed ? 38400 : 115200);
        }

        // Token: 0x04000025 RID: 37
        private SerialPort _port;
    }
}
