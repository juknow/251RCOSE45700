using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UIElements;

public class RoomPlayerController : NetworkBehaviour
{
    public float moveSpeed;


    public Transform shootTransform;

    [SerializeField]
    private float shootInterval = 0.05f;

    private float lastShotTime = 0f;

    [SerializeField]
    private float leftMargin;
    [SerializeField]
    private float rightMargin;


    void Update()
    {
        if (!isLocalPlayer) return;

        HandleMovement();
    }

    public void HandleMovement()
    {

        // (1) 플레이어 마우스 위치에 따라 X축으로 이동
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float toX = Mathf.Clamp(mousePos.x, leftMargin, rightMargin); // 이동 제한
        CmdMove(toX);
    }


    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();


        if (isServer && isLocalPlayer)
        {
            leftMargin = -7.5f;
            rightMargin = -1.5f;
        }
        else
        {
            leftMargin = 1.5f;
            rightMargin = 7.5f;
        }
    }

    [Command]
    void CmdMove(float targetX)
    {
         Debug.Log($"[SERVER] CmdMove called by {connectionToClient.connectionId} with X={targetX}");
        float clampedX = Mathf.Clamp(targetX, leftMargin, rightMargin);
        transform.position = new Vector3(clampedX, -3.0f, transform.position.z);
    }

}

