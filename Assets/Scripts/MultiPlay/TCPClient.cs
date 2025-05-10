using UnityEngine;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine.SceneManagement;

public class TCPClient : MonoBehaviour
{
    private TcpClient tcpClient;
    private NetworkStream stream;
    private Thread recvThread;
    private bool stageReady = false;


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

        //���콺 ��ġ ����
        if(stream != null && stream.CanWrite)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float x = mousePos.x;

            string msg = $"POS:{x:F2}";
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
            Debug.Log("���� �����");

            recvThread = new Thread(ReceiveData);
            recvThread.IsBackground = true;
            recvThread.Start();
        }
        catch (SocketException e)
        {
            Debug.LogError("���� ���� ����: " + e.Message);
        }
    }

    public void SendReady()
    {
        if (stream == null) return;

        byte[] data = Encoding.UTF8.GetBytes("READY");
        stream.Write(data, 0, data.Length);
        Debug.Log("READY ���۵�");
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
                Debug.Log("�����κ��� ����: " + msg);

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
                    if (int.TryParse(msg.Substring(9), out int myIndex))
                    {
                        UnityMainThreadDispatcher.Enqueue(() =>
                        {
                            SpawnMyPlayer(myIndex);
                        });
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
            Debug.LogWarning("���� ���� �����.");
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
        }
    }

    private void SpawnPlayer(int index)
    {
        string name = index == 0 ? "RoomPlayer1" : "RoomPlayer2";
        if (GameObject.Find(name) != null) return;

        GameObject prefab = RoomManager.SharedRoomPlayerPrefab;
        if (prefab != null)
        {
            Vector3 spawnPos = index == 0 ? new Vector3(-4.5f, -3f, 0) : new Vector3(4.5f, -3f, 0);
            GameObject player = Instantiate(prefab, spawnPos, Quaternion.identity);
            player.name = name;

            var ctrl = player.GetComponent<RoomPlayerController>();
            if (ctrl != null) ctrl.isPlayer1 = (index == 0);
        }
        else
        {
            Debug.LogWarning("[TCPClient] �÷��̾� �������� RoomManager.SharedRoomPlayerPrefab���� �Ҵ���� ����");
        }
    }

    void UpdateRemotePlayer(int index, float posX)
    {
        string name = index == 0 ? "RoomPlayer1" : "RoomPlayer2";
        GameObject obj = GameObject.Find(name);

        // �ڵ� ���� (���� �÷��̾ ���� ����)
        if (obj == null)
        {
            GameObject prefab = RoomManager.SharedRoomPlayerPrefab;
            if (prefab != null)
            {
                Vector3 spawnPos = index == 0 ? new Vector3(-4.5f, -3f, 0) : new Vector3(4.5f, -3f, 0);
                obj = Instantiate(prefab, spawnPos, Quaternion.identity);
                obj.name = name;

                var ctrl = obj.GetComponent<RoomPlayerController>();
                if (ctrl != null) ctrl.isPlayer1 = (index == 0);
            }
            else
            {
                Debug.LogWarning("RoomPlayer prefab�� RoomManager.SharedRoomPlayerPrefab���� ã�� �� �����ϴ�.");
            }
        }

        if (obj != null)
        {
            var controller = obj.GetComponent<RoomPlayerController>();
            controller?.SetXPosition(posX);
        }
    }

    private void OnApplicationQuit()
    {
        stream?.Close();
        tcpClient?.Close();
        recvThread?.Abort();
    }

}