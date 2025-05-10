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

    private Dictionary<TcpClient, int> clientIndexMap = new();
    private Dictionary<int, RoomPlayerController> playerControllers = new();

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
            try
            {
                TcpClient client = server.AcceptTcpClient();
                Debug.Log("[TCPServerUnity] Ŭ���̾�Ʈ �����");

                Thread t = new Thread(() => HandleClient(client));
                t.IsBackground = true;
                t.Start();
            }
            catch (Exception ex)
            {
                Debug.LogError($"[TCPServerUnity] Ŭ���̾�Ʈ ���� ����: {ex.Message}");
            }
        }
    }

    void HandleClient(TcpClient client)
    {
        NetworkStream stream = client.GetStream();
        int assignedIndex;

        lock (clientIndexMap)
        {
            assignedIndex = currentPlayerIndex;
            clientIndexMap[client] = assignedIndex;
            Debug.Log($"[TCPServerUnity] Ŭ���̾�Ʈ�� �ε��� {assignedIndex} �ο���");
            currentPlayerIndex++;
        }

        try
        {
            // 1. �ڽ��� �ε��� ����
            byte[] indexMsg = Encoding.UTF8.GetBytes($"SETINDEX:{assignedIndex}");
            stream.Write(indexMsg, 0, indexMsg.Length);

            //  2. ���� Ŭ���̾�Ʈ���� �� Ŭ���̾�Ʈ���� SPAWN
            lock (clientIndexMap)
            {
                foreach (var pair in clientIndexMap)
                {
                    int otherIndex = pair.Value;
                    if (otherIndex != assignedIndex)
                    {
                        byte[] spawnMsg = Encoding.UTF8.GetBytes($"SPAWN:{otherIndex}");
                        stream.Write(spawnMsg, 0, spawnMsg.Length);
                        Debug.Log($"[TCPServerUnity] ���� Ŭ���̾�Ʈ {otherIndex}�� SPAWN:{otherIndex}�� �� Ŭ���̾�Ʈ���� ����");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"[TCPServerUnity] �ʱ� SETINDEX �Ǵ� SPAWN ���� ����: {ex.Message}");
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

                if (msg.StartsWith("POS:"))
                {
                    string[] parts = msg.Split(':');
                    if (parts.Length == 3 &&
                        int.TryParse(parts[1], out int index) &&
                        float.TryParse(parts[2], out float x))
                    {
                        Broadcast($"SETX:{index}:{x:F2}");

                        if (playerControllers.TryGetValue(index, out var controller))
                        {
                            UnityMainThreadDispatcher.Enqueue(() =>
                            {
                                controller.SetXPosition(x);
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
                        Debug.Log($"[TCPServerUnity] ��� Ŭ���̾�Ʈ�� SPAWN:{index} ����");

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
                                    Debug.Log($"[TCPServerUnity] RoomPlayer{index + 1} ��Ʈ�ѷ� ��ϵ�");
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
            Debug.LogWarning("[TCPServerUnity] Ŭ���̾�Ʈ ��� ����: " + ex.Message);
        }
        finally
        {
            if (clientIndexMap.TryGetValue(client, out int idx))
            {
                Debug.Log($"[TCPServerUnity] Ŭ���̾�Ʈ {idx} ���� ����");
                clientIndexMap.Remove(client);
                playerControllers.Remove(idx);
            }

            client.Close();
        }
    }

    void Broadcast(string msg)
    {
        byte[] data = Encoding.UTF8.GetBytes(msg);
        foreach (var pair in clientIndexMap)
        {
            try
            {
                var stream = pair.Key.GetStream();
                if (stream.CanWrite)
                {
                    stream.Write(data, 0, data.Length);
                    Debug.Log($"[TCPServerUnity] �� Ŭ���̾�Ʈ {pair.Value}���� ���۵�: {msg}");
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[TCPServerUnity] Broadcast ���� (Ŭ���̾�Ʈ {pair.Value}): {ex.Message}");
            }
        }
    }

    void OnApplicationQuit()
    {
        try { server?.Stop(); } catch { }
        try { listenThread?.Abort(); } catch { }
    }
}
