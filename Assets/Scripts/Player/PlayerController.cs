using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    public float moveSpeed;

    public GameObject weapon;

    public Transform shootTransform;

    [SerializeField]
    public float shootInterval = 0.05f;
    [SerializeField] public float leftMargin;
    [SerializeField] public float rightMargin;

    public float lastShotTime = 0f;

    void Update()
    {
        if (!isLocalPlayer || !NetworkClient.ready) return;

        Debug.Log($"[{netId}] isLocalPlayer={isLocalPlayer}");

        HandleMovement();
        TryShoot();
    }

    public void HandleMovement()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float toX = Mathf.Clamp(mousePos.x, leftMargin, rightMargin);
        transform.position = new Vector3(toX, -3f, transform.position.z);
    }

    public void TryShoot()
    {
        if (Time.time - lastShotTime > shootInterval)
        {
            CmdShoot();
            lastShotTime = Time.time;
        }
    }

    [Command]
    public void CmdShoot()
    {
        GameObject bullet = Instantiate(weapon, shootTransform.position, Quaternion.identity);
        NetworkServer.Spawn(bullet);
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        if (isServer && isLocalPlayer)
        {
            leftMargin = -7.5f;
            rightMargin = -1.5f;
        }
        else
        {
            leftMargin = 1.5f;
            rightMargin = 7.5f;
        }
    }
}

