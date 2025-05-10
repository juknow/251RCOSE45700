using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public TCPClient network;
    public GameObject roomPlayerPrefab;

    public static GameObject SharedRoomPlayerPrefab;

    private bool player1Spawned = false;
    private bool player2Spawned = false;

    void Start()
    {
        SharedRoomPlayerPrefab = roomPlayerPrefab;

        if (network == null)
        {
            network = FindObjectOfType<TCPClient>();
            if (network == null)
                Debug.LogWarning("[RoomManager] TCPClient를 찾을 수 없습니다.");
        }
    }
    public void OnPlayer1Ready()
    {
        /*
        if (!player1Spawned)
        {
            SpawnRoomPlayer(true);
            player1Spawned = true;
        }
        */
        network.SendReady();
    }

    public void OnPlayer2Ready()
    {
        /*
        if (!player2Spawned)
        {
            SpawnRoomPlayer(false);
            player2Spawned = true;
        }
        */

        network.SendReady();
    }

    private void SpawnRoomPlayer(bool isPlayer1)
    {
        Vector3 spawnPos = isPlayer1 ? new Vector3(-4.5f, -3f, 0) : new Vector3(4.5f, -3f, 0);
        GameObject player = Instantiate(roomPlayerPrefab, spawnPos, Quaternion.identity);
        player.name = isPlayer1 ? "RoomPlayer1" : "RoomPlayer2";

        var controller = player.GetComponent<RoomPlayerController>();
        if (controller != null) controller.isPlayer1 = isPlayer1;
    }

}
