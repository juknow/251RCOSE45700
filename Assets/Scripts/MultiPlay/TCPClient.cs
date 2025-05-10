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
            }
        }
        catch
        {
            Debug.LogWarning("���� ���� �����.");
        }
    }

    private void OnApplicationQuit()
    {
        stream?.Close();
        tcpClient?.Close();
        recvThread?.Abort();
    }

}