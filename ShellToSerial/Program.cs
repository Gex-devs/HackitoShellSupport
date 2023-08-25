using System;
using System.IO.Ports;


namespace ShellToSerial
{
    public class program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            SerialHandlerClass serialHandlerClass = new SerialHandlerClass();
            serialHandlerClass.start();
        }


        
    }
}
