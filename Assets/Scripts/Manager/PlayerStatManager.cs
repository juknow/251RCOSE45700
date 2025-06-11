using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatManager : NetworkBehaviour
{
    public static PlayerStatManager Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private Slider hpSlider;
    [SerializeField] private Slider expSlider;
    [SerializeField] private GameObject upgradeCanvas;
    [SerializeField] private UpgradeContainer[] upgradeContainers;

    [Header("Data")]
    [SerializeField] private PlayerLevelData levelData;
    [SerializeField] private UpgradeData[] allUpgrades;

    [Header("기본 스탯")]
    [SyncVar(hook = nameof(OnHpChanged))] private float hp;
    [SyncVar] private float maxHp = 3f;
    [SyncVar(hook = nameof(OnExpChanged))] private float exp;
    [SyncVar] private float maxExp;
    [SyncVar(hook = nameof(OnLevelChanged))] private int level;
    [SyncVar] private float weaponDamage = 1f;
    [SyncVar] private float shootInterval = 0.5f;
    [SyncVar] private bool isUpgradeOpen = false;

    // 싱글톤 등록
    public override void OnStartLocalPlayer()
    {
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("[PlayerStatManager] 로컬 싱글톤 등록 완료");
        }
    }

    public override void OnStopClient()
    {
        if (isLocalPlayer && Instance == this)
        {
            Instance = null;
            Debug.Log("[PlayerStatManager] 로컬 싱글톤 해제");
        }
    }

    public override void OnStartServer()
    {
        level = levelData.startingLevel;
        maxHp = hp = 3f;
        exp = 0f;
        maxExp = levelData.expTable[level - 1];
    }

    public override void OnStartClient()
    {
        if (hpSlider != null)
        {
            hpSlider.maxValue = 1f;
            hpSlider.value = hp / maxHp;
        }

        if (expSlider != null)
        {
            expSlider.maxValue = 1f;
            expSlider.value = exp / maxExp;
        }
    }

    // Hook 콜백
    private void OnHpChanged(float oldHp, float newHp)
    {
        if (hpSlider != null)
            hpSlider.value = newHp / maxHp;
    }

    private void OnExpChanged(float oldExp, float newExp)
    {
        if (expSlider != null)
            expSlider.value = newExp / maxExp;
    }

    private void OnLevelChanged(int oldLevel, int newLevel)
    {
        Debug.Log($"[Client] 레벨업! {oldLevel} → {newLevel}");
    }

    public bool IsUpgradeOpen() => isUpgradeOpen;
    [Server] public float GetWeaponDamage() => weaponDamage;
    [Server] public float GetShootInterval() => shootInterval;

    // 데미지 처리
    [Server]
    public void TakeDamage(float amount)
    {
        hp = Mathf.Max(hp - amount, 0f);
        if (hp <= 0)
        {
            Debug.Log("[PlayerStatManager] 사망 처리 필요");
            // TODO: 죽음 처리 추가
        }
    }

    // 경험치 처리
    [Server]
    public void AddExp(float amount)
    {
        if (isUpgradeOpen) return;

        exp += amount;
        Debug.Log($"[PlayerStatManager] EXP +{amount} → {exp}/{maxExp}");

        while (exp >= maxExp)
        {
            exp -= maxExp;
            level++;
            Debug.Log($"[PlayerStatManager] 레벨업 → {level}");

            maxExp = (level - 1 < levelData.expTable.Count) ? levelData.expTable[level - 1] : float.MaxValue;
            isUpgradeOpen = true;

            UpgradeData[] selected = GetRandomUpgrades(3);
            TargetOpenUpgradeUI(connectionToClient, selected);
        }
    }

    // 업그레이드 UI 열기
    [TargetRpc]
    private void TargetOpenUpgradeUI(NetworkConnection target, UpgradeData[] upgrades)
    {
        Cursor.visible = true;
        upgradeCanvas.SetActive(true);
        CmdSetGamePaused(true);

        for (int i = 0; i < upgradeContainers.Length; i++)
        {
            upgradeContainers[i].SetUpgrade(upgrades[i]);
        }
    }

    [TargetRpc]
    private void TargetCloseUpgradeUI()
    {
        Cursor.visible = false;
        upgradeCanvas.SetActive(false);
        CmdSetGamePaused(false);
    }

    [Command]
    public void CmdApplyUpgrade(UpgradeType type)
    {
        Debug.Log($"[PlayerStatManager] 업그레이드 선택됨: {type}");
        ApplyUpgrade(type);
        isUpgradeOpen = false;
        TargetCloseUpgradeUI();
    }

    [Server]
    private void ApplyUpgrade(UpgradeType type)
    {
        switch (type)
        {
            case UpgradeType.IncreaseMaxHp:
                maxHp += 2f;
                hp += 2f;
                break;
            case UpgradeType.IncreaseDamage:
                weaponDamage += 1f;
                break;
            case UpgradeType.IncreaseFireRate:
                shootInterval = Mathf.Max(0.1f, shootInterval - 0.1f);
                break;
            default:
                Debug.LogWarning($"[PlayerStatManager] 알 수 없는 업그레이드: {type}");
                break;
        }
    }

    [Server]
    private UpgradeData[] GetRandomUpgrades(int count)
    {
        List<UpgradeData> pool = new List<UpgradeData>(allUpgrades);
        UpgradeData[] result = new UpgradeData[count];

        for (int i = 0; i < count && pool.Count > 0; i++)
        {
            int index = Random.Range(0, pool.Count);
            result[i] = pool[index];
            pool.RemoveAt(index);
        }

        return result;
    }

    [Command]
    private void CmdSetGamePaused(bool paused)
    {
        GameManager.Instance.SetPause(paused);
    }
}
