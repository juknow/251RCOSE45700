using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : NetworkBehaviour
{
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float enemyDamage = 1f;
    [SerializeField] private float expEarn = 1f;
    public float networkHp = 3f;

    [SyncVar(hook = nameof(OnHpChanged))]
    private float hp;

    private float maxHp;

    [SerializeField] private Slider hpSlider; // 슬라이더 연결

    private float minY = -7f;

    public override void OnStartServer()
    {
        maxHp = hp = networkHp;
    }

    void Start()
    {
        maxHp = hp;

        if (hpSlider != null)
        {
            hpSlider.maxValue = maxHp;
            hpSlider.value = hp;
        }
    }

    public override void OnStartClient()
    {
        Debug.Log("[Client] Enemy appeared!");
    }


    public void SetMoveSpeed(float moveSpeed)
    {
        this.moveSpeed = moveSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isServer) return;

        transform.position += Vector3.down * moveSpeed * Time.deltaTime;

        if (transform.position.y < minY)
        {
            NetworkServer.Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isServer) return;

        if (collision.CompareTag("Weapon"))
        {
            hp -= GameManager.Instance.GetWeaponDamage();

            hpSlider.value = hp;

            if (hp <= 0)
            {
                GameManager.Instance.AddExp(expEarn);
                NetworkServer.Destroy(gameObject);
            }

            Destroy(collision.gameObject);
        }
        else if (collision.CompareTag("Player"))
        {
            GameManager.Instance.DamagePlayer(enemyDamage);
            NetworkServer.Destroy(gameObject);
        }
    }

    void OnHpChanged(float oldHp, float newHp)
    {
        if (hpSlider != null)
        {
            hpSlider.value = newHp;
        }
    }
}