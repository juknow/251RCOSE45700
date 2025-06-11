using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : NetworkBehaviour
{
    [Header("Ω∫≈»")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float enemyDamage = 1f;
    [SerializeField] private float expEarn = 1f;
    [SerializeField] private float networkHp = 3f;

    [SyncVar(hook = nameof(OnHpChanged))]
    private float hp;

    private float maxHp;

    [Header("UI")]
    [SerializeField] private Slider hpSlider;

    private float minY = -7f;

    public override void OnStartServer()
    {
        maxHp = networkHp;
        hp = maxHp;
    }

    void Start()
    {
        if (hpSlider != null)
        {
            hpSlider.maxValue = networkHp;
            hpSlider.value = hp;
        }
    }

    public override void OnStartClient()
    {
        Debug.Log("[Client] Enemy spawned.");
    }

    void Update()
    {
        if (!isServer) return;

        transform.position += Vector3.down * moveSpeed * Time.deltaTime;

        if (transform.position.y < minY)
        {
            NetworkServer.Destroy(gameObject);
        }
    }

    // 
    [Server]
    public void ReceiveDamage(float amount)
    {
        hp -= amount;

        if (hp <= 0)
        {
            if (PlayerStatManager.Instance != null)
                PlayerStatManager.Instance.AddExp(expEarn);

            NetworkServer.Destroy(gameObject);
        }
    }

    void OnHpChanged(float oldHp, float newHp)
    {
        if (hpSlider != null)
            hpSlider.value = newHp;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isServer) return;

        if (other.CompareTag("Player"))
        {
            if (PlayerStatManager.Instance != null)
                PlayerStatManager.Instance.TakeDamage(enemyDamage);

            NetworkServer.Destroy(gameObject);
        }
    }
}
