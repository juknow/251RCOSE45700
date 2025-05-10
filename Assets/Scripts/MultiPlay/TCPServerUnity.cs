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

    private Dictionary<TcpClient, int> clientIndexMap = new(); // 클라이언트 → index
    private Dictionary<int, RoomPlayerController> playerControllers = new(); // index → RoomPlayerController

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

        lock (clientIndexMap)
        {
            clientIndexMap[client] = currentPlayerIndex;
            Debug.Log($"[TCPServerUnity] 클라이언트에 인덱스 {currentPlayerIndex} 부여됨");
            currentPlayerIndex++;
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

                if (msg.StartsWith("POS:"))
                {
                    string[] parts = msg.Split(':');
                    if (parts.Length == 3 &&
                        int.TryParse(parts[1], out int index) &&
                        float.TryParse(parts[2], out float x))
                    {
                        Broadcast($"SETX:{index}:{x:F2}");

                        // 서버에서도 위치 반영 (옵션)
                        if (playerControllers.TryGetValue(index, out var controller))
                        {
                            float moveX = x;
                            UnityMainThreadDispatcher.Enqueue(() =>
                            {
                                controller.SetXPosition(moveX);
                            });
                        }
                    }
                }
                else if (msg == "READY")
                {
                    int index = -1;
                    lock (clientIndexMap)
                    {
                        if (clientIndexMap.TryGetValue(client, out int idx))
                        {
                            index = idx;
                        }
                    }

                    if (index >= 0)
                    {
                        Broadcast($"SPAWN:{index}");
                        Debug.Log($"[TCPServerUnity] 모든 클라이언트에 SPAWN:{index} 전송");

                        // Unity 오브젝트 캐싱
                        UnityMainThreadDispatcher.Enqueue(() =>
                        {
                            string name = index == 0 ? "RoomPlayer1" : "RoomPlayer2";
                            GameObject obj = GameObject.Find(name);
                            if (obj != null)
                            {
                                var ctrl = obj.GetComponent<RoomPlayerController>();
                                if (ctrl != null)
                                {
                                    playerControllers[index] = ctrl;
                                    Debug.Log($"[TCPServerUnity] RoomPlayer{index + 1} 컨트롤러 등록됨");
                                }
                            }
                        });
                    }

                    readyCount++;
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
        foreach (var pair in clientIndexMap)
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