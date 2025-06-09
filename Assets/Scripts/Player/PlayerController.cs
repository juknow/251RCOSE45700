using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class PlayerController : NetworkBehaviour
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
        // 로컬 플레이어가 아니면 입력 무시
        if (!isLocalPlayer) return;

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

        // 이동 위치를 서버에 전달하려면 커맨드 함수 추가 가능
        // 예) CmdMove(toX);
    }

    public void TryShoot()
    {
        if (Time.time - lastShotTime > shootInterval)
        {
            Shoot();
            lastShotTime = Time.time;
        }
    }
    [Command]
    public void CmdShoot()
    {
        GameObject bullet = Instantiate(weapon, shootTransform.position, Quaternion.identity);
        NetworkServer.Spawn(bullet);
    }

    public void Shoot()
    {
        // 실제로는 커맨드 호출해야 함
        CmdShoot();
    }

}

