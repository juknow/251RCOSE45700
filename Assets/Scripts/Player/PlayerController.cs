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
        transform.position = new Vector3(toX, -4.5f , 0);

        previousX = toX;

        CmdMove(toX);
    }

    [Command]
    void CmdMove(float toX)
    {
        RpcMove(toX);
    }

    [ClientRpc]
    void RpcMove(float toX)
    {
        transform.position = new Vector3(toX, transform.position.y, transform.position.z);
    }

    void TryShoot()
    {
        float interval = GameManager.Instance.fireRate;

        if (Time.time - lastShotTime > interval)
        {
            lastShotTime = Time.time;
            CmdShoot();
        }
    }

    [Command]
    void CmdShoot()
    {
        if (weapon == null || shootTransform == null)
            return;

        GameObject bullet = Instantiate(weapon, shootTransform.position, Quaternion.identity);
        NetworkServer.Spawn(bullet);
    }


    [Command] // 게임오브젝트가 AUtority가 없어서 룸매니저나 PlayerPrefab에 있는 authority에 커맨드를 가지고 있어야함.
    public void CmdRequestUpgrade(UpgradeType type)
    {
        GameManager.Instance.ApplyUpgradeOnServer(type); 
        GameManager.Instance.RpcCloseUpgradeUI();
        Debug.LogWarning("업그레이드 적용");
    }

}

