using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public TCPClient network;
    public GameObject roomPlayer1Prefab;
    public GameObject roomPlayer2Prefab;

    private bool player1Spawned = false;
    private bool player2Spawned = false;

    void Start()
    {
        if (network == null)
        {
            network = FindObjectOfType<TCPClient>();
            if (network == null)
                Debug.LogWarning("[RoomManager] TCPClient를 찾을 수 없습니다.");
        }
    }
    public void OnPlayer1Ready()
    {
        if (!player1Spawned)
        {
            SpawnRoomPlayer1();
            player1Spawned = true;
        }

        network.SendReady();
    }

    public void OnPlayer2Ready()
    {
        if (!player2Spawned)
        {
            SpawnRoomPlayer2();
            player2Spawned = true;
        }

        network.SendReady();
    }

    private void SpawnRoomPlayer1()
    {
        GameObject player1 = Instantiate(roomPlayer1Prefab, new Vector3(-4.5f, -3f, 0), Quaternion.identity);
        player1.name = "RoomPlayer1";

        var controller = player1.GetComponent<RoomPlayerController>();
        if (controller != null) controller.isPlayer1 = true;
    }

    private void SpawnRoomPlayer2()
    {
        GameObject player2 = Instantiate(roomPlayer2Prefab, new Vector3(4.5f, -3f, 0), Quaternion.identity);
        player2.name = "RoomPlayer2";

        var controller = player2.GetComponent<RoomPlayerController>();
        if (controller != null) controller.isPlayer1 = false;
    }

}
