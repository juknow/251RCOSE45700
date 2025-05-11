using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float hp = 1f;
    [SerializeField] private float enemyDamage = 1f;
    [SerializeField] private float expEarn = 1f;

    private float maxHp;

    [SerializeField] private Slider hpSlider; // 슬라이더 연결

    private float minY = -7f;

    void Start()
    {
        maxHp = hp;

        if (hpSlider != null)
        {
            hpSlider.maxValue = maxHp;
            hpSlider.value = hp;
        }
    }


    public void SetMoveSpeed(float moveSpeed)
    {
        this.moveSpeed = moveSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.down * moveSpeed * Time.deltaTime;

        if (transform.position.y < minY)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Weapon"))
        {
            hp -= GameManager.Instance.weaponDamage;

            hpSlider.value = hp;

            if (hp <= 0)
            {
                GameManager.Instance.AddExp(expEarn);
                Destroy(gameObject);
            }

            Destroy(collision.gameObject);
        }
        else if (collision.CompareTag("Player"))
        {
            GameManager.Instance.DamagePlayer(enemyDamage);
            Destroy(gameObject);
        }
    }
}