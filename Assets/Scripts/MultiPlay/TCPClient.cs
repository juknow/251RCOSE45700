using UnityEngine;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;

public class TCPClient : MonoBehaviour
{
    private TcpClient tcpClient;
    private NetworkStream stream;
    private Thread recvThread;
    private bool stageReady = false;
    private Dictionary<int, RoomPlayerController> playerMap = new();

    public static int MyPlayerIndex { get; private set; }
    public static float LastReceivedOtherPlayerX { get; private set; }

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

        if (stream != null && stream.CanWrite && tcpClient?.Connected == true && MyPlayerIndex >= 0)
        {
            try
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                float x = mousePos.x;

                string msg = $"POS:{MyPlayerIndex}:{x:F2}";
                byte[] data = Encoding.UTF8.GetBytes(msg);
                stream.Write(data, 0, data.Length);
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"[TCPClient] 위치 전송 실패: {ex.Message}");
            }
        }
    }

    void ConnectToServer()
    {
        StartCoroutine(TryConnect());
    }

    private IEnumerator TryConnect()
    {
        int attempts = 0;
        bool retryNeeded;

        while (attempts < 5)
        {
            retryNeeded = false;

            try
            {
                tcpClient = new TcpClient("localhost", 9050);
                stream = tcpClient.GetStream();
                Debug.Log("[TCPClient] 서버 연결됨");

                recvThread = new Thread(ReceiveData);
                recvThread.IsBackground = true;
                recvThread.Start();
                yield break; // 연결 성공 시 종료
            }
            catch (SocketException ex)
            {
                attempts++;
                retryNeeded = true;
                Debug.LogWarning($"[TCPClient] 서버 연결 재시도 중 ({attempts}/5): {ex.Message}");
            }

            if (retryNeeded)
                yield return new WaitForSeconds(1f);
        }

        Debug.LogError("[TCPClient] 서버 연결 실패 - 최대 재시도 도달");
    }

    public void SendReady()
    {
        StartCoroutine(SendReadyWithDelay());
    }

    private IEnumerator SendReadyWithDelay()
    {
        if (stream == null || !tcpClient.Connected)
        {
            Debug.LogError("[TCPClient] 서버와 연결되어 있지 않습니다.");
            yield break;
        }

        try
        {
            byte[] data = Encoding.UTF8.GetBytes("READY");
            stream.Write(data, 0, data.Length);
            Debug.Log("[TCPClient] READY 전송됨");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[TCPClient] READY 전송 실패: {ex.Message}");
            yield break;
        }

        yield return new WaitForSeconds(0.2f);
    }

    void ReceiveData()
    {
        try
        {
            byte[] buffer = new byte[1024];
            while (true)
            {
                int bytes = stream.Read(buffer, 0, buffer.Length);
                if (bytes == 0)
                {
                    Debug.LogWarning("[TCPClient] 서버 연결 종료 감지 (stream.Read == 0)");
                    break;
                }

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
                        Debug.Log($"[TCPClient] → 인덱스 {index}의 좌표 {posX} 수신됨");
                        UpdateRemotePlayer(index, posX);
                    }
                }
                else if (msg.StartsWith("SETINDEX:"))
                {
                    if (int.TryParse(msg.Substring(9), out int index))
                    {
                        MyPlayerIndex = index;
                        UnityMainThreadDispatcher.Enqueue(() => SpawnPlayer(index, true));
                    }
                }
                else if (msg.StartsWith("SPAWN:"))
                {
                    if (int.TryParse(msg.Substring(6), out int index))
                    {
                        UnityMainThreadDispatcher.Enqueue(() => SpawnPlayer(index, false));
                    }
                }
            }
        }
        catch (ThreadAbortException)
        {
            Debug.Log("[TCPClient] 수신 스레드 중단됨 (ThreadAbort)");
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"[TCPClient] 수신 중 오류: {ex.Message}");
        }
    }

    void SpawnPlayer(int index, bool isMine)
    {
        if (playerMap.ContainsKey(index)) return;

        Vector3 spawnPos = index == 0 ? new Vector3(-4.5f, -3f, 0) : new Vector3(4.5f, -3f, 0);
        GameObject player = Instantiate(RoomManager.SharedRoomPlayerPrefab, spawnPos, Quaternion.identity);
        player.name = index == 0 ? "RoomPlayer1" : "RoomPlayer2";

        var controller = player.GetComponent<RoomPlayerController>();
        if (controller != null)
        {
            controller.isPlayer1 = (index == 0);
            playerMap[index] = controller;
        }

        if (isMine)
            Debug.Log($"[TCPClient] 내 플레이어 {index} 생성됨");
        else
            Debug.Log($"[TCPClient] 다른 플레이어 {index} 스폰됨");
    }

    void UpdateRemotePlayer(int index, float posX)
    {
        if (index == MyPlayerIndex) return;

        UnityMainThreadDispatcher.Enqueue(() =>
        {
            LastReceivedOtherPlayerX = posX;
            Debug.Log($"[TCPClient] 메인 스레드에서 상대 위치 저장됨: {posX}");

            if (playerMap.TryGetValue(index, out var controller))
            {
                controller.SetXPosition(posX);
            }
        });
    }

    private void OnApplicationQuit()
    {
        try { recvThread?.Abort(); } catch { }
        try { stream?.Close(); } catch { }
        try { tcpClient?.Close(); } catch { }
    }
}
