﻿using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace EnttecOpenDmxUsbConsole.Logic
{
    public class OpenDMX
    {
        public static byte[] buffer = new byte[513];
        public static uint handle;
        public static bool done = false;
        public static bool Connected = false;
        public static int bytesWritten = 0;
        public static FT_STATUS status;

        public const byte BITS_8 = 8;
        public const byte STOP_BITS_2 = 2;
        public const byte PARITY_NONE = 0;
        public const UInt16 FLOW_NONE = 0;
        public const byte PURGE_RX = 1;
        public const byte PURGE_TX = 2;

        [DllImport("FTD2XX.dll")]
        public static extern FT_STATUS FT_Open(UInt32 uiPort, ref uint ftHandle);
        [DllImport("FTD2XX.dll")]
        public static extern FT_STATUS FT_Close(uint ftHandle);
        [DllImport("FTD2XX.dll")]
        public static extern FT_STATUS FT_Read(uint ftHandle, IntPtr lpBuffer, UInt32 dwBytesToRead, ref UInt32 lpdwBytesReturned);
        [DllImport("FTD2XX.dll")]
        public static extern FT_STATUS FT_Write(uint ftHandle, IntPtr lpBuffer, UInt32 dwBytesToRead, ref UInt32 lpdwBytesWritten);
        [DllImport("FTD2XX.dll")]
        public static extern FT_STATUS FT_SetDataCharacteristics(uint ftHandle, byte uWordLength, byte uStopBits, byte uParity);
        [DllImport("FTD2XX.dll")]
        public static extern FT_STATUS FT_SetFlowControl(uint ftHandle, char usFlowControl, byte uXon, byte uXoff);
        [DllImport("FTD2XX.dll")]
        public static extern FT_STATUS FT_GetModemStatus(uint ftHandle, ref UInt32 lpdwModemStatus);
        [DllImport("FTD2XX.dll")]
        public static extern FT_STATUS FT_Purge(uint ftHandle, UInt32 dwMask);
        [DllImport("FTD2XX.dll")]
        public static extern FT_STATUS FT_ClrRts(uint ftHandle);
        [DllImport("FTD2XX.dll")]
        public static extern FT_STATUS FT_SetBreakOn(uint ftHandle);
        [DllImport("FTD2XX.dll")]
        public static extern FT_STATUS FT_SetBreakOff(uint ftHandle);
        [DllImport("FTD2XX.dll")]
        public static extern FT_STATUS FT_GetStatus(uint ftHandle, ref UInt32 lpdwAmountInRxQueue, ref UInt32 lpdwAmountInTxQueue, ref UInt32 lpdwEventStatus);
        [DllImport("FTD2XX.dll")]
        public static extern FT_STATUS FT_ResetDevice(uint ftHandle);
        [DllImport("FTD2XX.dll")]
        public static extern FT_STATUS FT_SetDivisor(uint ftHandle, char usDivisor);

        public static void Start()
        {
            handle = 0;
            status = FT_Open(0, ref handle);
            // Setting up the WriteData method to be on it's own thread. This will also turn all channels off
            // this unrequested change of state can be managed by getting the current state of all channels 
            // into the write buffer before calling this function.
            Thread thread = new Thread(new ThreadStart(WriteData));
            thread.Start();
        }

        public static void SetDmxValue(int channel, byte value)
        {
            if (buffer != null)
            {
                buffer[channel] = value;
            }
        }

        public static void WriteData()
        {
            try
            {
                InitOpenDMX();
                if (OpenDMX.status == FT_STATUS.FT_OK)
                {
                    status = FT_SetBreakOn(handle);
                    status = FT_SetBreakOff(handle);
                    bytesWritten = Write(handle, buffer, buffer.Length);

                    Thread.Sleep(25);      //give the system time to send the data before sending more
                }
                else
                {
                    throw new Exception("No Enttec USB Device Found");
                }
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp);
            }
        }

        public static int Write(uint handle, byte[] data, int length)
        {
            try
            {
                IntPtr ptr = Marshal.AllocHGlobal((int)length);
                Marshal.Copy(data, 0, ptr, (int)length);
                uint bytesWritten = 0;
                status = FT_Write(handle, ptr, (uint)length, ref bytesWritten);
                return (int)bytesWritten;
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp);
                return 0;
            }
        }

        public static void InitOpenDMX()
        {
            status = FT_ResetDevice(handle);
            status = FT_SetDivisor(handle, (char)12);  // set baud rate
            status = FT_SetDataCharacteristics(handle, BITS_8, STOP_BITS_2, PARITY_NONE);
            status = FT_SetFlowControl(handle, (char)FLOW_NONE, 0, 0);
            status = FT_ClrRts(handle);
            status = FT_Purge(handle, PURGE_TX);
            status = FT_Purge(handle, PURGE_RX);
        }
    }


    /// <summary>
    /// Enumaration containing the varios return status for the DLL functions.
    /// </summary>
    public enum FT_STATUS
    {
        FT_OK = 0,
        FT_INVALID_HANDLE,
        FT_DEVICE_NOT_FOUND,
        FT_DEVICE_NOT_OPENED,
        FT_IO_ERROR,
        FT_INSUFFICIENT_RESOURCES,
        FT_INVALID_PARAMETER,
        FT_INVALID_BAUD_RATE,
        FT_DEVICE_NOT_OPENED_FOR_ERASE,
        FT_DEVICE_NOT_OPENED_FOR_WRITE,
        FT_FAILED_TO_WRITE_DEVICE,
        FT_EEPROM_READ_FAILED,
        FT_EEPROM_WRITE_FAILED,
        FT_EEPROM_ERASE_FAILED,
        FT_EEPROM_NOT_PRESENT,
        FT_EEPROM_NOT_PROGRAMMED,
        FT_INVALID_ARGS,
        FT_OTHER_ERROR
    };
}
