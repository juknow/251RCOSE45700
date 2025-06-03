using UnityEngine;
using Mirror;
using UnityEngine.UI;
using TMPro;

public class NetworkLobbyManager : NetworkManager
{
    [Header("Lobby UI")]
    public Button hostButton;
    public Button joinButton;
    public TMP_InputField ipInputField;
    public GameObject readyButton;
    public TextMeshProUGUI statusText;

    private void Start()
    {
        // UI 버튼 이벤트 연결
        hostButton.onClick.AddListener(OnHostButtonClicked);
        joinButton.onClick.AddListener(OnJoinButtonClicked);
    }

    public void OnHostButtonClicked()
    {
        StartHost();
        statusText.text = "호스트로 시작됨";
        readyButton.SetActive(true);
    }

    public void OnJoinButtonClicked()
    {
        networkAddress = ipInputField.text;
        StartClient();
        statusText.text = "참가자로 시작됨";
        readyButton.SetActive(true);
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        Debug.Log("서버가 시작되었습니다.");
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        Debug.Log("클라이언트가 시작되었습니다.");
    }
}