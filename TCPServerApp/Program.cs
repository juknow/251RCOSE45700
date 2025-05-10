using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Program
{
    static List<TcpClient> clients = new List<TcpClient>();
    static int readyCount = 0;

    public static void Main()
    {
        TcpListener server = new TcpListener(IPAddress.Any, 9050);
        server.Start();
        Console.WriteLine("서버 시작됨");

        while (true)
        {
            TcpClient client = server.AcceptTcpClient();
            clients.Add(client);
            Console.WriteLine("클라이언트 연결됨");

            Thread clientThread = new Thread(() => HandleClient(client));
            clientThread.Start();
        }
    }

    static void HandleClient(TcpClient client)
    {
        NetworkStream stream = client.GetStream();
        byte[] buffer = new byte[1024];

        while (true)
        {
            int read = stream.Read(buffer, 0, buffer.Length);
            if (read == 0) break;

            string message = Encoding.UTF8.GetString(buffer, 0, read);
            Console.WriteLine("수신: " + message);

            if (message == "READY")
            {
                readyCount++;
                Console.WriteLine($"현재 Ready 수: {readyCount}");

                if (readyCount >= 2)
                {
                    Broadcast("STAGE");
                    readyCount = 0;
                }
            }
        }

        client.Close();
    }

    static void Broadcast(string msg)
    {
        byte[] data = Encoding.UTF8.GetBytes(msg);
        foreach (TcpClient c in clients)
        {
            if (c.Connected)
            {
                c.GetStream().Write(data, 0, data.Length);
            }
        }
    }
}
