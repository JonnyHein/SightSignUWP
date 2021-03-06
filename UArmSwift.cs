using System;
using System.Collections.Generic;
using System.Threading;
using Windows.Devices.SerialCommunication;
using System.IO.Ports;
using System.Text;
using System.IO;

namespace SightSignUWP
{
    public class UArmSwiftPro : IArm
    {
        //private UArmSwift _arm;
        private readonly string _port;
        private SerialPort _serialPort;

        public UArmSwiftPro()
        {
            _port = GetPort();
        }

        // Get port with async arrow function & return resulting COM string
        private string GetPort()
        {
            var task = System.Threading.Tasks.Task.Run(async () => await PortDetails.FindPortAsync());
            task.Wait();
            return task.Result;
        }

        public void Connect()
        {
            SerialPort _serialPort = new SerialPort(this._port, 115200)
            {
                DtrEnable = true
            };
            _serialPort.DataReceived += this.DataReceived;
            this._serialPort = _serialPort;

            if (!this._serialPort.IsOpen)
            {
                try
                {
                    this._serialPort.Open();
                    Thread.Sleep(2000);
                }
                catch (UnauthorizedAccessException e)
                {
                    Console.WriteLine("UnauthorizedAccessException" + e);
                }
                catch (ArgumentOutOfRangeException e)
                {
                    Console.WriteLine("ArgumentOutOfRangeException" + e);
                }
                catch (ArgumentException e)
                {
                    Console.WriteLine("ArgumentException" + e);
                }
                catch (IOException e)
                {
                    Console.WriteLine("IOException" + e);
                }
                catch (InvalidOperationException e)
                {
                    Console.WriteLine("InvalidOperationException" + e);
                }
            }

            //Move(1, 2, 3, false);
            //_arm = new UArmSwift(_port);
            //_arm.Connect();
            //_arm.Mode(Mode.UniversalHolder);
        }
        private void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var local = (SerialPort)sender;
            int lengthToRead = local.BytesToRead;
            byte[] rxBytes = new byte[lengthToRead];
            local.Read(rxBytes, 0, lengthToRead);

            var response = Encoding.ASCII.GetString(rxBytes);
        }

        public void Disconnect()
        {
            _serialPort.Close();
            //_arm.Disconnect();
            //_arm = null;
        }

        public void Move(double x, double y, double z, bool scara)
        {
            // note: scara not supported
            var scale = 3.0;
            var xx = x * 70.0 * scale + 200.0; 
            var yy = y * 100.0 * scale + 50.0;
            var zz = z * 20.0 + 50;

            if (_serialPort.IsOpen)
            {
                try
                {
                    _serialPort.WriteLine($"G0 X{xx} Y{yy} Z{zz} F30000");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }


            //_arm.MoveXYZ(xx, yy, zz, 20000);
        }
    }
}