using System.Collections;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using TMPro;
using System.Net.Sockets;
using System.Net;

public class CreateRoomUI : MonoBehaviour
{
    [SerializeField] private Button hostButton;
    [SerializeField] private TextMeshProUGUI ipText;

    // Use this for initialization
    void Start()
        {
        string ip = GetLocalIPAddress();
        ipText.text = $"My IP: {ip}";
        hostButton.onClick.AddListener(CreateRoom);

    }

        // Update is called once per frame
        void Update()
        {

        }


    public void CreateRoom()
    {
        var manager = RoomManager.singleton;
        manager.StartHost();
    }

    public static string GetLocalIPAddress()
    {
        string localIP = "";
        try
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                    break;
                }
            }

            if (string.IsNullOrEmpty(localIP))
            {
                localIP = "No IPv4 address found";
            }
        }
        catch
        {
            localIP = "IP not available";
        }

        return localIP;
    }
}