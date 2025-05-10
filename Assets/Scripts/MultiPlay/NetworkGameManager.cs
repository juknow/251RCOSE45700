using UnityEngine;

public class NetworkGameManager : MonoBehaviour
{
    public static NetworkGameManager Instance { get; private set; }

    public TCPClient network;
    public GameObject player1;
    public GameObject player2;
    public int myIndex;
    public float myMouseX;
    public float otherMouseX;

    void Awake()
    {
        Instance = this;
    }


    void Start()
    {

        if (network == null)
        {
            network = FindObjectOfType<TCPClient>();
            if (network == null)
                Debug.LogWarning("[RoomManager] TCPClient�� ã�� �� �����ϴ�.");
        }

        myIndex = TCPClient.MyPlayerIndex; // myIndex : 0 -> 1PClient , 1-> 2PClient
        Debug.Log($"[NetworkGameManager] �� �ε����� {myIndex}�Դϴ�.");
    }

    void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        myMouseX = mousePos.x;
        otherMouseX = TCPClient.LastReceivedOtherPlayerX;
    }

}
