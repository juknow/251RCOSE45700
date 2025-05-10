using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RoomPlayerController : MonoBehaviour
{
    public float moveSpeed;

    // 서버에서 직접 위치를 업데이트
    public void SetXPosition(float x)
    {
        float clampedX = isPlayer1
            ? Mathf.Clamp(x, -7.5f, -1.5f)
            : Mathf.Clamp(x, 1.5f, 7.5f);

        transform.position = new Vector3(clampedX, -3f, transform.position.z);
    }


    public Transform shootTransform;

    [SerializeField]
    private float shootInterval = 0.05f;

    private float lastShotTime = 0f;

    [SerializeField]
    private float leftMargin;
    [SerializeField]
    private float rightMargin;

    public bool isPlayer1 = true;


    void Update()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float clampedX;

        if (isPlayer1)
            clampedX = Mathf.Clamp(mousePos.x, -7.5f, -1.5f);
        else
            clampedX = Mathf.Clamp(mousePos.x, 1.5f, 7.5f);

        transform.position = new Vector3(clampedX, -3f, transform.position.z);
    }

}

