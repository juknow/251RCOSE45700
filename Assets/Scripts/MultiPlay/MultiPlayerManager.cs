using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MultiPlayerManager : NetworkManager
{ //���߿� RoomManager �����Ұ���
    public void StartHosting()
    {
        StartHost();
    }

    public void JoinGame(string address)
    {
        networkAddress = address;
        StartClient();
    }

    // ���� ���� �� �ڵ����� �÷��̾� ����
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        GameObject player = Instantiate(playerPrefab);
        NetworkServer.AddPlayerForConnection(conn, player);
    }
}
