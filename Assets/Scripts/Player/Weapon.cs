using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Weapon : NetworkBehaviour
{
    public float moveSpeed = 10; // 기본 속도 설정

    // Update is called once per frame
    void Update()
    {
        // 위쪽으로 일정 속도로 이동
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;
    }

    public override void OnStartServer()
    {
        Destroy(gameObject, 3f);
    }
}

