using UnityEngine;
using UnityEngine.UI;

public class RoomUI : MonoBehaviour
{
    public RoomManager roomManager;

    public Button readyBtn1;
    public Button readyBtn2;

    void Start()
    {
        readyBtn1.onClick.AddListener(() => roomManager.OnPlayer1Ready());
        readyBtn2.onClick.AddListener(() => roomManager.OnPlayer2Ready());
    }
}