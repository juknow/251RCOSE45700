using UnityEngine;
using Mirror;

public class NetworkPlayer : NetworkBehaviour
{
    [SyncVar]
    private bool isReady = false;

    [SerializeField]
    private GameObject playerPrefab;

    public void SetReady()
    {
        if (!isReady)
        {
            CmdSetReady();
        }
    }

    [Command]
    private void CmdSetReady()
    {
        isReady = true;
        RpcUpdateReadyStatus(isReady);
        CheckAllPlayersReady();
    }

    [ClientRpc]
    private void RpcUpdateReadyStatus(bool ready)
    {
        isReady = ready;
    }

    [Server]
    private void CheckAllPlayersReady()
    {
        NetworkPlayer[] players = FindObjectsOfType<NetworkPlayer>();
        bool allReady = true;

        foreach (NetworkPlayer player in players)
        {
            if (!player.isReady)
            {
                allReady = false;
                break;
            }
        }

        if (allReady && players.Length >= 2)
        {
            // 게임 씬으로 전환
            NetworkManager.singleton.ServerChangeScene("GameScene");
        }
    }
}