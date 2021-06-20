using EnttecOpenDmxUsbConsole.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EnttecOpenDmxUsbConsole.Logic
{
    public class DmxCommands
    {
        public int DmxAddress { get; set; }


        public DmxCommands(int dmxAddress)
        {
            DmxAddress = dmxAddress;
        }

        public  void SendDmxCommand(int offset, int value, int duration = 1000)
        {
            DmxCommandHelper(new DmxCommand() { Offset = offset, Value = value });

            OpenDMX.WriteData();

            Thread.Sleep(duration);
        }

        public  void SendDmxCommands(List<DmxCommand> dmxCommands, int duration = 1000)
        {
            foreach (var dmxCommand in dmxCommands)
            {
                DmxCommandHelper(dmxCommand);
            }

            OpenDMX.WriteData();

            Thread.Sleep(duration);
        }

        private  void DmxCommandHelper(DmxCommand dmxCommand)
        {
            if (dmxCommand.Value < 0 || dmxCommand.Value > 255)
            {
                throw new ArgumentOutOfRangeException();
            }

            int channel = DmxAddress + dmxCommand.Offset;

            Console.WriteLine($"Sending value {dmxCommand.Value} to channel {channel}");

            OpenDMX.SetDmxValue(channel, Convert.ToByte(dmxCommand.Value));
        }
    }
}
