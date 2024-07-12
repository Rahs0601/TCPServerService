using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCPServerService;

namespace ConsoleApp1
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            while (true)
            {
                //if key borad key is pressed, then exit the loop
                if (Console.KeyAvailable)
                {
                    break;
                }
                //Create a TCP/IP Server Socket
                TCPServer server = new TCPServer();
                server.Start();
            }
        }
    }
}