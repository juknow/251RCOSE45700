using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class LobbyPlayerController : NetworkBehaviour
{
    private float previousX;

    [SerializeField] private float leftMargin = -4f;
    [SerializeField] private float rightMargin = 4f;


    void Start()
    {
        previousX = transform.position.x;
    }
    void Update()
    {
        if (!isLocalPlayer) return;

        HandleMovement();
    }

    // 로컬에서 마우스 입력으로 서버에 위치 전송
    public void HandleMovement()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float toX = Mathf.Clamp(mousePos.x, leftMargin, rightMargin);

        if (Mathf.Abs(toX - previousX) > 0.01f)
        {
            CmdMove(toX);
            previousX = toX;
        }
    }

    // 서버에서 위치 갱신 → 모든 클라이언트로 전파
    [Command]
    void CmdMove(float toX)
    {
        RpcMove(toX);
    }

    // 모든 클라이언트가 위치 반영
    [ClientRpc]
    void RpcMove(float toX)
    {
        transform.position = new Vector3(toX, transform.position.y, transform.position.z);
    }
}

