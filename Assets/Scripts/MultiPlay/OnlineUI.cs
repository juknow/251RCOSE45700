using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;
public class OnlineUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField ipInputField;
    [SerializeField] private Button joinButton;

    void Start()
    {
        joinButton.onClick.AddListener(JoinServer);
    }

    void JoinServer()
    {
        string ipAddress = ipInputField.text;

        if (string.IsNullOrWhiteSpace(ipAddress))
        {
            Debug.LogWarning("IP 주소를 입력하세요.");
            return;
        }

        var manager = NetworkManager.singleton;
        manager.networkAddress = ipAddress;
        manager.StartClient(); 
    }
}