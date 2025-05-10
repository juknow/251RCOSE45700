using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    [SerializeField] private float scrollSpeed = 2f;
    [SerializeField] private float containerHeight = 10.1f;
    [SerializeField] private float minHeight = -10.35f;

    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.down * scrollSpeed * Time.deltaTime;
        if (transform.position.y < minHeight )
        {
            transform.position += new Vector3(0, containerHeight, 0);
        }
    }
}
