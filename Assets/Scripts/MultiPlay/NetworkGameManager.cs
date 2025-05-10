using UnityEngine;

public class NetworkGameManager : MonoBehaviour
{
    public TCPClient network;
    public GameObject player1;
    public GameObject player2;
    public int myIndex;

    void Start()
    {

        if (network == null)
        {
            network = FindObjectOfType<TCPClient>();
            if (network == null)
                Debug.LogWarning("[RoomManager] TCPClient�� ã�� �� �����ϴ�.");
        }

        myIndex = myIndex;
    }

}
