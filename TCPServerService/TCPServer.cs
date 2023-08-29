using System.Net.Sockets;
using System.Net;
using System.Text;
using System;
using System.Configuration;
using System.Threading;

namespace TCPServerService
{
    internal class TCPServer
    {
        string dataToBeRecived = ConfigurationManager.AppSettings["DataToBeRecived"];
        string dataToBeSent = ConfigurationManager.AppSettings["DataToBeSent"];
        string MissedData = ConfigurationManager.AppSettings["MissedData"];
        public void start()
        {
            // Set the IP address and port on which the server will listen
            string ipAddress = "0.0.0.0"; // Use "0.0.0.0" to listen on all available network interfaces
            int port = 8888;

            // Create a TCP listener
            TcpListener listener = new TcpListener(IPAddress.Parse(ipAddress), port);
            listener.Start();

            Logger.WriteDebugLog($"Server is listening on {ipAddress}:{port}");

            while (true)
            {
                Logger.WriteDebugLog("Waiting for a connection...");
                try
                {
                    TcpClient client = listener.AcceptTcpClient(); // Accept incoming client connections

                    Logger.WriteDebugLog($"Client connected! Local:{client.Client.LocalEndPoint} <-- Client:{client.Client.RemoteEndPoint}");

                    // Handle the client connection in a separate thread
                    System.Threading.Tasks.Task.Run(() => HandleClient(client));
                }
                catch (Exception ex)
                {
                    Logger.WriteErrorLog(ex.Message);
                }

            }
        }

        void HandleClient(TcpClient client)
        {
            Thread.CurrentThread.Name = client.Client.RemoteEndPoint.ToString();
            using (NetworkStream stream = client.GetStream())
            {
                byte[] buffer = new byte[1024];
                int bytesRead;

                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    // Convert the received bytes to a string
                    string receivedData = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    if (receivedData == dataToBeRecived)
                    {
                        Logger.WriteDebugLog("Received: " + receivedData);
                        Logger.WriteDebugLog("Data Matched");
                        string responseData = "Server: " + dataToBeSent;
                        byte[] responseBytes = Encoding.ASCII.GetBytes(responseData);
                        stream.Write(responseBytes, 0, responseBytes.Length);

                    }
                    else
                    {
                        Logger.WriteDebugLog("Received: " + receivedData);
                        Logger.WriteDebugLog("Data Not Matched");
                        string responseData = "Server: " + receivedData;
                        byte[] responseBytes = Encoding.ASCII.GetBytes(responseData);
                        stream.Write(responseBytes, 0, responseBytes.Length);
                    }

                    //// Send a response back to the client
                    //string responseData = "Server: " + receivedData;
                    //byte[] responseBytes = Encoding.ASCII.GetBytes(responseData);
                    //stream.Write(responseBytes, 0, responseBytes.Length);
                }

                // Client disconnected
                Logger.WriteDebugLog("Client disconnected!");

                // Clean up resources
                client.Close();
            }
        }

    }

}