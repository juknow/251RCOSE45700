using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
public class TCPServerUnity : MonoBehaviour
{
    private TcpListener server;
    private Thread listenThread;
    private Dictionary<TcpClient, RoomPlayerController> clientMap = new();
    public List<RoomPlayerController> roomPlayers = new(); // ����Ƽ���� �Ҵ�

    private int currentPlayerIndex = 0;
    private int readyCount = 0;

    void Start()
    {
        server = new TcpListener(IPAddress.Any, 9050);
        server.Start();

        listenThread = new Thread(AcceptClients);
        listenThread.IsBackground = true;
        listenThread.Start();

        Debug.Log("[TCPServerUnity] ���� ���۵�");
    }

    void AcceptClients()
    {
        while (true)
        {
            TcpClient client = server.AcceptTcpClient();
            Debug.Log("[TCPServerUnity] Ŭ���̾�Ʈ �����");

            Thread t = new Thread(() => HandleClient(client));
            t.IsBackground = true;
            t.Start();
        }
    }

    void HandleClient(TcpClient client)
    {
        NetworkStream stream = client.GetStream();
        RoomPlayerController assignedPlayer = null;

        lock (clientMap)
        {
            if (currentPlayerIndex < roomPlayers.Count)
            {
                assignedPlayer = roomPlayers[currentPlayerIndex];
                clientMap[client] = assignedPlayer;
                currentPlayerIndex++;

                Debug.Log($"[TCPServerUnity] �÷��̾�{currentPlayerIndex} �����");
            }
        }

        byte[] buffer = new byte[1024];

        try
        {
            while (true)
            {
                int read = stream.Read(buffer, 0, buffer.Length);
                if (read == 0) break;

                string msg = Encoding.UTF8.GetString(buffer, 0, read);
                Debug.Log("[TCPServerUnity] ����: " + msg);

                if (msg.StartsWith("POS:") && assignedPlayer != null)
                {
                    if (float.TryParse(msg.Substring(4), out float x))
                    {
                        // Unity ���� �����忡�� �����̱� ���� �޼��� ����
                        float moveX = x;
                        UnityMainThreadDispatcher.Enqueue(() =>
                        {
                            assignedPlayer.SetXPosition(moveX);
                        });
                    }
                }
                else if (msg == "READY")
                {
                    readyCount++;
                    Debug.Log($"[TCPServerUnity] Ready ��: {readyCount}");
                    if (readyCount >= 2)
                    {
                        Broadcast("STAGE");
                        readyCount = 0;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning("Ŭ���̾�Ʈ ��� ����: " + ex.Message);
        }

        client.Close();
    }

    void Broadcast(string msg)
    {
        byte[] data = Encoding.UTF8.GetBytes(msg);
        foreach (var pair in clientMap)
        {
            var stream = pair.Key.GetStream();
            if (stream.CanWrite)
                stream.Write(data, 0, data.Length);
        }
    }

    void OnApplicationQuit()
    {
        server?.Stop();
        listenThread?.Abort();
    }
}