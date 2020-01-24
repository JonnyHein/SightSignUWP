namespace SightSignUWP
{
    using System;
    using System.Collections.Generic;
    using Windows.Devices.Enumeration;
    using Windows.Devices.SerialCommunication;
    using Windows.Management;

    internal class PortDetails
    {
        public string Name { get; set; }
        public string PnPId { get; set; }
        public string Manufacturer { get; set; }
        public string ComName
        {
            get
            {
                var parts = Name.Split('(', ')');
                return parts.Length > 1 ? parts[1] : null;
            }
        }

        public static async System.Threading.Tasks.Task<string> FindPortAsync()
        {
            /* Find all serial devices on the Machine */
            string aqs = SerialDevice.GetDeviceSelector();
            var deviceCollection = await DeviceInformation.FindAllAsync(aqs);

            foreach (var item in deviceCollection)
            {
                /* return COM port if item is uARM Swift Pro*/
                if (item.Id.Contains("USB#VID_2341&PID_0042"))
                {
                    var serialDevice = await SerialDevice.FromIdAsync(item.Id);
                    string portName = serialDevice.PortName;
                    serialDevice.Dispose();
                    return portName;
                }
            }

            throw new Exception("Could not find COM port");
        }
    }
}