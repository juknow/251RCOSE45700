using System.Collections;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class CreateRoomUI : MonoBehaviour
{
    [SerializeField] private Button hostButton;

    // Use this for initialization
    void Start()
        {
        hostButton.onClick.AddListener(CreateRoom);

    }

        // Update is called once per frame
        void Update()
        {

        }


        public void CreateRoom()
        {
            var manager = RoomManager.singleton;

            // 방 설정 작업 처리

            manager.StartHost();
        }
    }