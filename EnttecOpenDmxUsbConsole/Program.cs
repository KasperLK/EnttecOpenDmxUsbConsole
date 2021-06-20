using EnttecOpenDmxUsbConsole.Logic;
using EnttecOpenDmxUsbConsole.Objects;
using System;
using System.Collections.Generic;
using System.Threading;

namespace EnttecOpenDmxUsbConsole
{
    class Program
    {
        static Logic.DmxCommands DmxCommandObj;

        static int dmxAddress = 168;

        static int panOffset = 0;
        static int tiltOffset = 1;
        static int continuesOffset = 5;
        static int dimmerOffset = 6;
        static int led1Offset = 7;
        static int led2Offset = 8;
        static int led3Offset = 9;
        static int led4Offset = 10;


        static void Main(string[] args)
        {
            try
            {
                OpenDMX.Start();                                           //find and connect to devive (first found if multiple)
                if (OpenDMX.status == FT_STATUS.FT_DEVICE_NOT_FOUND)       //update status
                    Console.WriteLine("No Enttec USB Device Found");
                else if (OpenDMX.status == FT_STATUS.FT_OK)
                    Console.WriteLine("Found DMX on USB");
                else
                    Console.WriteLine("Error Opening Device");
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp);
                Console.WriteLine("Error Connecting to Enttec USB Device");
            }

            DmxCommandObj = new Logic.DmxCommands(dmxAddress);



            ResetAll();
            Thread.Sleep(2500);


            #region Send multiple commands
            Console.WriteLine();
            Console.WriteLine("Running preprogrammed commands");

            var panAndTiltCommand = new List<DmxCommand>()
            {
                new DmxCommand { Offset = panOffset, Value = 255 },
                new DmxCommand { Offset = continuesOffset, Value = 180 },
                new DmxCommand { Offset = dimmerOffset, Value = 255 },
                new DmxCommand { Offset = led1Offset, Value = (int)Util.Colors.Blue },
                new DmxCommand { Offset = led2Offset, Value = (int)Util.Colors.Green },
                new DmxCommand { Offset = led3Offset, Value = (int)Util.Colors.Red },
                new DmxCommand { Offset = led4Offset, Value = (int)Util.Colors.White }
            };

            DmxCommandObj.SendDmxCommands(panAndTiltCommand);
            Thread.Sleep(5000);
            #endregion


            #region Send single command
            ResetAll();
            Thread.Sleep(2500);
            Console.WriteLine();
            Console.WriteLine("Running single preprogrammed command");

            DmxCommandObj.SendDmxCommand(panOffset, 255, 2000);
            Thread.Sleep(5000);
            #endregion


            #region Send commands from console            
            ResetAll();
            Thread.Sleep(2500);

            Console.WriteLine();
            Console.WriteLine("Enter channel offset and command value to send a single command");

            string line;
            while ((line = Console.ReadLine()) != null)
            {
                if (string.IsNullOrEmpty(line))
                {
                    break;
                }
                else
                {
                    try
                    {
                        var character = line.Split(' ');
                        if (character.Length > 1)
                        {
                            DmxCommandObj.SendDmxCommand(int.Parse(character[0]), int.Parse(character[1]));
                        }
                        else
                        {
                            DmxCommandObj.SendDmxCommand(panOffset, int.Parse(line));
                        }
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        Console.WriteLine("Entered values is not valid. Must be between 0 and 255");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                }
            }
            #endregion

            ResetAll();

            Console.WriteLine();
            Console.WriteLine("Done");
        }


        public static void ResetAll()
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
            DmxCommandObj.SendDmxCommands(resetCommand);
        }
    }
}
