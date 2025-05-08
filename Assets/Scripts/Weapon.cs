using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 10; // 기본 속도 설정

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        // 위쪽으로 일정 속도로 이동
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;
    }
}

