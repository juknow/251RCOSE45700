using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MultiPlayerManager : NetworkManager
{ //나중에 RoomManager 구현할거임
    public void StartHosting()
    {
        StartHost();
    }

    public void JoinGame(string address)
    {
        networkAddress = address;
        StartClient();
    }

    // 서버 시작 시 자동으로 플레이어 생성
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        GameObject player = Instantiate(playerPrefab);
        NetworkServer.AddPlayerForConnection(conn, player);
    }
}
