using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
public class OnlineUI : MonoBehaviour
    {
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

    public void OnClickStartServer()
    {
        var manager = RoomManager.singleton;
        manager.StartClient();
    }
}