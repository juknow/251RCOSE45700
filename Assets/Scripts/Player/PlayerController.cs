using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed;

    public GameObject weapon;

    public Transform shootTransform;

    [SerializeField] private float shootInterval = 0.5f;
    [SerializeField] private float leftMargin = -4f;
    [SerializeField] private float rightMargin = 4f;

    private float lastShotTime = 0f;

    void Update()
    {

        HandleMovement();
        TryShoot();
    }

    public void HandleMovement()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float toX = Mathf.Clamp(mousePos.x, leftMargin, rightMargin);
        transform.position = new Vector3(toX, transform.position.y, transform.position.z);
    }

    public void TryShoot()
    {
        if (Time.time - lastShotTime > shootInterval)
        {
            Shoot();
            lastShotTime = Time.time;
        }
    }
    public void Shoot()
    {
        GameObject bullet = Instantiate(weapon, shootTransform.position, Quaternion.identity);
    }

}

