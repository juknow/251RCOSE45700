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
    public List<RoomPlayerController> roomPlayers = new(); // 유니티에서 할당

    private int currentPlayerIndex = 0;
    private int readyCount = 0;

    void Start()
    {
        server = new TcpListener(IPAddress.Any, 9050);
        server.Start();

        listenThread = new Thread(AcceptClients);
        listenThread.IsBackground = true;
        listenThread.Start();

        Debug.Log("[TCPServerUnity] 서버 시작됨");
    }

    void AcceptClients()
    {
        while (true)
        {
            TcpClient client = server.AcceptTcpClient();
            Debug.Log("[TCPServerUnity] 클라이언트 연결됨");

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

                Debug.Log($"[TCPServerUnity] 플레이어{currentPlayerIndex} 연결됨");
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
                Debug.Log("[TCPServerUnity] 수신: " + msg);

                if (msg.StartsWith("POS:") && assignedPlayer != null)
                {
                    if (float.TryParse(msg.Substring(4), out float x))
                    {
                        // Unity 메인 스레드에서 움직이기 위해 메서드 예약
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
                    Debug.Log($"[TCPServerUnity] Ready 수: {readyCount}");
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
            Debug.LogWarning("클라이언트 통신 오류: " + ex.Message);
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