using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 10f;

    private float minY = -7f;

    void Update()
    {
        // 아래로 이동
        transform.position += Vector3.down * moveSpeed * Time.deltaTime;

        // 화면 아래로 벗어나면 제거
        if (transform.position.y < minY)
        {
            Destroy(gameObject);
        }
    }
}

