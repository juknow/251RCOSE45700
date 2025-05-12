using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float previousX;
    public float moveSpeed;

    public GameObject weapon;

    public Transform shootTransform;

    [SerializeField] private float shootInterval = 0.5f;
    [SerializeField] private float leftMargin = -4f;
    [SerializeField] private float rightMargin = 4f;

    private float lastShotTime = 0f;

    void Start()
    {
        previousX = transform.position.x;
    }
    void Update()
    {
        if (GameManager.Instance.isGamePaused) return;
        HandleMovement();
        TryShoot();
    }

    public void HandleMovement()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float toX = Mathf.Clamp(mousePos.x, leftMargin, rightMargin);
        float deltaX = Mathf.Abs(toX - previousX);
        transform.position = new Vector3(toX, transform.position.y, transform.position.z);

        previousX = toX;
        GameManager.Instance.AddFeedback(deltaX);
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

