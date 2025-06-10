using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;

public class RoomManager : NetworkRoomManager
{
    public override void OnServerSceneChanged(string sceneName)
    {
        base.OnServerSceneChanged(sceneName);

        if (sceneName == GameplayScene)  // GameplayScene은 SoloPlayScene의 이름이어야 함
        {
            // RoomPlayer들을 제거
            foreach (NetworkConnectionToClient conn in NetworkServer.connections.Values)
            {
                if (conn.identity != null && conn.identity.gameObject.GetComponent<NetworkRoomPlayer>() != null)
                {
                    GameObject roomPlayer = conn.identity.gameObject;

                    // 원하는 경우 player prefab으로 교체 가능
                    GameObject gameplayPlayer = Instantiate(playerPrefab);
                    NetworkServer.ReplacePlayerForConnection(conn, gameplayPlayer, true);

                    // 기존 RoomPlayer는 제거
                    NetworkServer.Destroy(roomPlayer);
                }
            }
        }
    }

}