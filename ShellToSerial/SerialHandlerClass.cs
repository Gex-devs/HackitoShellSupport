using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace ShellToSerial
{
    internal class SerialHandlerClass
    {
        private SerialPort _serialPort;
        private Process _process;

        public SerialHandlerClass() 
        {
            _serialPort = new SerialPort();
            _serialPort.BaudRate = 115200;
            _serialPort.PortName = "COM6";
            _serialPort.DataBits = 8;
            _serialPort.Parity = Parity.None;
            _serialPort.StopBits = StopBits.One;
            _serialPort.Open();
            _serialPort.DataReceived += new SerialDataReceivedEventHandler(HandleSerialData);

            Thread CommandReader = new Thread(ReadCommands);
            CommandReader.Start();
        }

        public void start()
        {
             CmdLine();
            
        }   

        public void HandleSerialData(object sender,
      SerialDataReceivedEventArgs e)
        {
            Console.WriteLine(e.ToString());

           
        }


        private void CmdLine()
        {

            _process = new Process();
            _process.StartInfo.FileName = "powershell.exe";
            _process.StartInfo.UseShellExecute = false;
            _process.StartInfo.RedirectStandardInput = true;
            _process.StartInfo.RedirectStandardOutput = true;
            _process.StartInfo.RedirectStandardError = true;
            _process.OutputDataReceived += Process_OutputDataReceived;
            _process.ErrorDataReceived += Process_ErrorDataReceived;
            _process.Start();
            _process.BeginOutputReadLine();
            _process.BeginErrorReadLine();
        }

        private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            // Send command prompt error output to Netcat server
            try
            {
                //_serialPort.WriteLine(e.Data);
                //Console.WriteLine(e.Data);
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                return;
            }

        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            try
            {
                _serialPort.WriteLine(e.Data);
                Console.WriteLine(e.Data);
            }
            catch (Exception ex)
            {

                try
                {
                    _serialPort.Close();
             
                    _process.Kill();

                }
                catch (Exception)
                {
                    // Wait 1 second and try again
                    Console.WriteLine("Failed");
                }
               

            }
            // Send command prompt output to Netcat server

        }

        private void ReadCommands()
        {
            // Read commands from Netcat server and send to command prompt process
            while (true)
            {
                try
                {
                    string command = _serialPort.ReadLine();
                    _process.StandardInput.WriteLine(command);
                }
                catch (Exception ex)
                {
                    // Handle exceptions that occur when reading from the network stream
                    Debug.Print($"Disconnected: {ex.Message}");
                   
                    return;
                }
            }
        }

    }
}
