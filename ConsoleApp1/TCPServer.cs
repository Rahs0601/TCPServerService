using System;
using System.Collections.Concurrent;
using System.Configuration;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TCPServerService
{
    internal class TCPServer
    {
        private int timeoutMinutes = 5; // Set the timeout duration in minutes
        private ConcurrentDictionary<string, ClientInfo> connectedClients = new ConcurrentDictionary<string, ClientInfo>();

        public byte[] response { get; private set; }

        public void Start()
        {
            string ipAddress = "0.0.0.0";
            int port = 6010;

            TcpListener listener = new TcpListener(IPAddress.Parse(ipAddress), port);
            listener.Start();
            Console.WriteLine($"Server is listening on {ipAddress}:{port}");

            while (true)
            {
                Console.WriteLine("Waiting for a connection...");
                try
                {
                    TcpClient client = listener.AcceptTcpClient();
                    Console.WriteLine($"Client connected! Local:{client.Client.LocalEndPoint} <-- Client:{client.Client.RemoteEndPoint}");
                    Task.Run(() => HandleClient(client));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private void HandleClient(TcpClient client)
        {
            string clientIP = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
            Console.WriteLine($"Client IP Address: {clientIP}");

            ClientInfo clientInfo = new ClientInfo { Client = client, LastActivityTime = DateTime.Now };
            connectedClients.TryAdd(clientIP, clientInfo);

            try
            {
                using (NetworkStream stream = client.GetStream())
                {
                    byte[] buffer = new byte[1024];
                    int bytesRead;

                    while (true)
                    {
                        if (client.Available > 0)
                        {
                            bytesRead = stream.Read(buffer, 0, buffer.Length);
                            if (bytesRead == 0) break;
                            string receivedData = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                            Console.WriteLine($"Received from {clientIP}: {receivedData}");

                            // Process received data here
                            //Thread.Sleep(1000); // Simulate processing time

                            // Send response
                            if (receivedData.Equals("OK"))
                            {
                                response = Encoding.ASCII.GetBytes("NOT OK");
                                stream.Write(response, 0, response.Length);
                                Console.WriteLine($"Sent to {clientIP}: Server response");
                            }
                            response = Encoding.ASCII.GetBytes("OK");
                            stream.Write(response, 0, response.Length);
                            Console.WriteLine($"Sent to {clientIP}: Server response");

                            clientInfo.LastActivityTime = DateTime.Now;
                        }
                        if (DateTime.Now - clientInfo.LastActivityTime > TimeSpan.FromMinutes(timeoutMinutes))
                        {
                            Console.WriteLine($"Client {clientIP} timed out. Closing connection.");
                            client.Close();
                            break;
                        }
                        Thread.Sleep(100); // Avoid tight loop
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling client {clientIP}: {ex.Message}");
            }
            finally
            {
                connectedClients.TryRemove(clientIP, out _);
                client.Close();
                Console.WriteLine($"Client {clientIP} disconnected!");
            }
        }
    }

    internal class ClientInfo
    {
        public TcpClient Client { get; set; }
        public DateTime LastActivityTime { get; set; }
    }
}