using UnityEngine;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class TCPClient : MonoBehaviour
{
    private TcpClient tcpClient;
    private NetworkStream stream;
    private Thread recvThread;
    private bool stageReady = false;
    private Dictionary<int, RoomPlayerController> playerMap = new();
    public static int MyPlayerIndex { get; private set; }


    void Start()
    {
        ConnectToServer();
    }

    void Update()
    {
        if (stageReady)
        {
            stageReady = false;
            SceneManager.LoadScene("StageScene");
        }

        // 위치 전송 (내 플레이어만)
        if (stream != null && stream.CanWrite && MyPlayerIndex >= 0)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float x = mousePos.x;

            string msg = $"POS:{MyPlayerIndex}:{x:F2}";
            byte[] data = Encoding.UTF8.GetBytes(msg);
            stream.Write(data, 0, data.Length);
        }

    }

    void ConnectToServer()
    {
        try
        {
            tcpClient = new TcpClient("localhost", 9050);
            stream = tcpClient.GetStream();
            Debug.Log("서버 연결됨");

            recvThread = new Thread(ReceiveData);
            recvThread.IsBackground = true;
            recvThread.Start();
        }
        catch (SocketException e)
        {
            Debug.LogError("서버 연결 실패: " + e.Message);
        }
    }

    public void SendReady()
    {
        if (stream == null) return;

        byte[] data = Encoding.UTF8.GetBytes("READY");
        stream.Write(data, 0, data.Length);
        Debug.Log("READY 전송됨");
    }


    void ReceiveData()
    {
        try
        {
            byte[] buffer = new byte[1024];
            while (true)
            {
                int bytes = stream.Read(buffer, 0, buffer.Length);
                string msg = Encoding.UTF8.GetString(buffer, 0, bytes);
                Debug.Log("서버로부터 수신: " + msg);

                if (msg == "STAGE")
                {
                    stageReady = true;
                }
                else if (msg.StartsWith("SETX:"))
                {
                    string[] parts = msg.Split(':');
                    if (parts.Length == 3 &&
                        int.TryParse(parts[1], out int index) &&
                        float.TryParse(parts[2], out float posX))
                    {
                        UpdateRemotePlayer(index, posX);
                    }
                }
                else if (msg.StartsWith("SETINDEX:"))
                {
                    if (int.TryParse(msg.Substring(9), out int index))
                    {
                        MyPlayerIndex = index;
                        UnityMainThreadDispatcher.Enqueue(() => SpawnMyPlayer(index));
                    }
                }
                else if (msg.StartsWith("SPAWN:"))
                {
                    if (int.TryParse(msg.Substring(6), out int index))
                    {
                        UnityMainThreadDispatcher.Enqueue(() =>
                        {
                            SpawnPlayer(index);
                        });
                    }
                }
            }
        }
        catch
        {
            Debug.LogWarning("서버 연결 종료됨.");
        }
    }

    private void SpawnMyPlayer(int index)
    {
        Vector3 spawnPos = index == 0 ? new Vector3(-4.5f, -3f, 0) : new Vector3(4.5f, -3f, 0);
        GameObject player = Instantiate(RoomManager.SharedRoomPlayerPrefab, spawnPos, Quaternion.identity);
        player.name = index == 0 ? "RoomPlayer1" : "RoomPlayer2";

        var controller = player.GetComponent<RoomPlayerController>();
        if (controller != null)
        {
            controller.isPlayer1 = (index == 0);
            playerMap[index] = controller;
        }
    }

    private void SpawnPlayer(int index)
    {
        if (playerMap.ContainsKey(index)) return;

        GameObject prefab = RoomManager.SharedRoomPlayerPrefab;
        if (prefab != null)
        {
            Vector3 spawnPos = index == 0 ? new Vector3(-4.5f, -3f, 0) : new Vector3(4.5f, -3f, 0);
            GameObject player = Instantiate(prefab, spawnPos, Quaternion.identity);
            player.name = index == 0 ? "RoomPlayer1" : "RoomPlayer2";

            var ctrl = player.GetComponent<RoomPlayerController>();
            if (ctrl != null)
            {
                ctrl.isPlayer1 = (index == 0);
                playerMap[index] = ctrl;
            }
        }
        else
        {
            Debug.LogWarning("[TCPClient] SharedRoomPlayerPrefab is null.");
        }
    }

    void UpdateRemotePlayer(int index, float posX)
    {
        if (index == MyPlayerIndex) return;

        if (playerMap.TryGetValue(index, out var controller))
        {
            controller.SetXPosition(posX);
        }
        else
        {
            Debug.LogWarning($"[TCPClient] Remote player {index} not found in map.");
        }
    }

    private void OnApplicationQuit()
    {
        stream?.Close();
        tcpClient?.Close();
        recvThread?.Abort();
    }

}