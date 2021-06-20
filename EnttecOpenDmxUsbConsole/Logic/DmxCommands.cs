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

        static int dmxAddress = 168;
        static int panOffset = 0;
        static int tiltOffset = 1;
        static int continuesOffset = 5;
        static int dimmerOffset = 6;
        static int led1Offset = 7;
        static int led2Offset = 8;
        static int led3Offset = 9;
        static int led4Offset = 10;


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

        public void ResetAll()
        {
            Console.WriteLine();
            Console.WriteLine("Resetting all");
            var resetCommand = new List<DmxCommand>()
            {
                new DmxCommand { Offset = panOffset, Value = 0 },
                new DmxCommand { Offset = tiltOffset, Value = 0 },
                new DmxCommand { Offset = continuesOffset, Value = 0 },
                new DmxCommand { Offset = dimmerOffset, Value = 0 },
                new DmxCommand { Offset = led1Offset, Value = 0 },
                new DmxCommand { Offset = led2Offset, Value = 0 },
                new DmxCommand { Offset = led3Offset, Value = 0 },
                new DmxCommand { Offset = led4Offset, Value = 0 }
            };
            SendDmxCommands(resetCommand);
        }
    }
}
