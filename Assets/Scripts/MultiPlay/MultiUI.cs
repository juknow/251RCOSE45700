using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class MultiUI : MonoBehaviour
{
    public MultiPlayerManager multiplayerManager;

    public Button hostButton;
    public Button joinButton;
    public string ipAddress = "localhost";

    void Start()
    {
        hostButton.onClick.AddListener(OnHostClicked);
        joinButton.onClick.AddListener(OnJoinClicked);

    }

    void OnHostClicked()
    {
        multiplayerManager.StartHosting();
    }

    void OnJoinClicked()
    {
        multiplayerManager.JoinGame(ipAddress);
    }

}
