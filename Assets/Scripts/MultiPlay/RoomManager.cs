using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;
using UnityEngine.SceneManagement;

public class RoomManager : NetworkRoomManager
{
    public override void OnServerSceneChanged(string sceneName)
    {
        base.OnServerSceneChanged(sceneName);

        if (sceneName == GameplayScene)
        {
            Scene activeScene = SceneManager.GetActiveScene();

            foreach (NetworkConnectionToClient conn in NetworkServer.connections.Values)
            {
                if (conn.identity != null && conn.identity.TryGetComponent(out NetworkRoomPlayer roomPlayer))
                {
                    // 1. 먼저 DontDestroyOnLoad 회피
                    SceneManager.MoveGameObjectToScene(roomPlayer.gameObject, activeScene);

                    // 2. 플레이어 생성
                    GameObject player = Instantiate(playerPrefab);
                    NetworkServer.ReplacePlayerForConnection(conn, player, true);

                    // 3. 이제 안전하게 Destroy 가능 → 모든 클라이언트에 전파됨
                    NetworkServer.Destroy(roomPlayer.gameObject);
                    Debug.Log("Destroyed roomPlayer on server: " + roomPlayer.gameObject.name);
                }
            }
        }
    }

    public override void OnRoomClientSceneChanged()
    {
        Debug.Log("OnRoomClientSceneChanged CALLED");
        base.OnRoomClientSceneChanged();

        Debug.Log(SceneManager.GetActiveScene().name);
        
        if (SceneManager.GetActiveScene().name == "SoloPlayScene")
        {
            Debug.Log("[Client] Destroying leftover RoomPlayer: ");
            var roomPlayers = GameObject.FindObjectsOfType<NetworkRoomPlayer>();
            foreach (var roomPlayer in roomPlayers)
            {
                    Debug.Log("[Client] Destroying leftover RoomPlayer: " + roomPlayer.name);
                    Destroy(roomPlayer.gameObject);

            }
        }
        
    }



}