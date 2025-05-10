using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public float moveSpeed = 10; // 기본 속도 설정

    // Update is called once per frame
    void Update()
    {
        // 위쪽으로 일정 속도로 이동
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;
    }

    void Start()
    {
        Destroy(gameObject, 3f);
    }
}

