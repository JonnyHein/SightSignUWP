using System;
using System.Collections.Generic;
using System.Threading;

namespace SightSignUWP
{
    public class UArmSwiftPro : IArm
    {
        //private UArmSwift _arm;
        private readonly string _port;

        public UArmSwiftPro()
        {
            _port = GetPort();
        }

        // Get port through async arrow function & return result
        private string GetPort()
        {
            var task = System.Threading.Tasks.Task.Run(async () => await PortDetails.FindPortAsync());
            task.Wait();
            return task.Result;
        }


        public void Connect()
        {
            //_arm = new UArmSwift(_port);
            //_arm.Connect();
            //_arm.Mode(Mode.UniversalHolder);
        }

        public void Disconnect()
        {
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
            System.Diagnostics.Debug.WriteLine($"X={xx} Y={yy} Z={zz}");

            //_arm.MoveXYZ(xx, yy, zz, 20000);
        }
    }
}