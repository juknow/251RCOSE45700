using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    public bool isGamePaused = false;

    [SerializeField] private PlayerLevelData playerLevelData;

    [SerializeField] private StageManager stageManager;
    [SerializeField] private StageData[] allStages;

    private int currentStageIndex = 0;

    [Header("Player Stats")]
    [SyncVar] public float playerHp = 3f;
    [SyncVar] public float maxPlayerHp = 3f;

    [SyncVar] public float playerExp = 0f;
    [SyncVar] public float maxPlayerExp = 3f;

    [SyncVar] public int playerLevel = 1;
    [SyncVar] public float weaponDamage = 1f;




    [SerializeField]
    private Slider playerHpSlider;

    [SerializeField]
    private Slider playerExpSlider;

    [SerializeField] private UpgradeData[] allUpgrades; // 전체 업그레이드 목록
    [SerializeField] private GameObject upgradeCanvas; // UpgradeCanvas 전체
    [SerializeField] private UpgradeContainer[] upgradeContainers; // 3개 컨테이너 참조


    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public override void OnStartServer()
    {
        playerLevel = playerLevelData.startingLevel;
        playerExp = 0f;
        maxPlayerHp = playerHp;
        SetMaxExpForLevel(playerLevel);
    }

    void Start()
    {
        Cursor.visible = false;
        RegisterSpawnPrefabs();

        playerHpSlider.maxValue = 1f;
        SetMaxExpForLevel(playerLevel);

        if (NetworkServer.active) StartStage(currentStageIndex);
    }

    void Update()
    {
        if (!isClient) return;
        playerHpSlider.value = playerHp / maxPlayerHp;
        playerExpSlider.value = playerExp / maxPlayerExp;
    }

    private void RegisterSpawnPrefabs()
    {
        foreach (StageData stage in allStages)
        {
            foreach (GameObject prefab in stage.enemyPrefabs)
            {
                if (!NetworkClient.prefabs.ContainsValue(prefab))
                    NetworkClient.RegisterPrefab(prefab);
            }
        }
    }

    public float GetWeaponDamage()
    {
        return weaponDamage;
    }

    void StartStage(int index)
    {
        if (index < allStages.Length)
        {
            StageData stage = allStages[index];
            Debug.Log($"[GameManager] Starting {stage.stageType}");

            stageManager.OnStageCompleted += HandleStageCompleted;
            stageManager.StartStage(stage);
        }
        else
        {
            Debug.Log("게임 전체 완료!");
        }
    }

    void HandleStageCompleted()
    {
        stageManager.OnStageCompleted -= HandleStageCompleted;
        currentStageIndex++;
        if (NetworkServer.active) StartStage(currentStageIndex);
    }

    [Server]
    public void DamagePlayer(float damage)
    {
        playerHp -= damage;
        Debug.Log($"[GameManager] Player damaged. Current HP: {playerHp}");

        if (playerHp <= 0)
        {
            Debug.Log("[GameManager] Player Died");
            // 여기에 게임 오버 처리 추가 가능
        }
    }

    [Server]
    public void AddExp(float amount)
    {
        playerExp += amount;
        Debug.Log($" 경험치 획득: +{amount}");

        while (playerExp >= maxPlayerExp && playerLevel < playerLevelData.expTable.Count)
        {
            playerExp -= maxPlayerExp;
            playerLevel++;

            SetMaxExpForLevel(playerLevel);
            Debug.Log($"레벨업! 현재 레벨: {playerLevel}");
            List<string> upgradeIds = GetRandomUpgradeIds(3);
            RpcOpenUpgradeUI(upgradeIds.ToArray());
        }

        Debug.Log($"[레벨 {playerLevel}] EXP: {playerExp:F1} / {maxPlayerExp:F1}");
    }

    /*
    private void OpenUpgradeUI()
    {
        Cursor.visible = true;
        Time.timeScale = 0f;
        upgradeCanvas.SetActive(true);
        isGamePaused = true;

        List<UpgradeData> selected = GetRandomUpgrades(3);

        for (int i = 0; i < upgradeContainers.Length; i++)
        {
            upgradeContainers[i].SetUpgrade(selected[i]);
        }
    }
    */

    [ClientRpc]
    void RpcOpenUpgradeUI(string[] upgradeIds)
    {
        Cursor.visible = true;
        Time.timeScale = 0f;
        upgradeCanvas.SetActive(true);
        isGamePaused = true;

        List<UpgradeData> selected = new List<UpgradeData>();
        foreach (string id in upgradeIds)
        {
            UpgradeData data = FindUpgradeDataById(id);
            if (data != null)
                selected.Add(data);
        }

        for (int i = 0; i < upgradeContainers.Length; i++)
        {
            upgradeContainers[i].SetUpgrade(selected[i]);
        }
    }

    [Server]
    private List<string> GetRandomUpgradeIds(int count)
    {
        List<string> result = new List<string>();
        List<UpgradeData> pool = new List<UpgradeData>(allUpgrades);

        for (int i = 0; i < count && pool.Count > 0; i++)
        {
            int index = Random.Range(0, pool.Count);
            result.Add(pool[index].upgradeId); // ID만 저장
            pool.RemoveAt(index);
        }

        return result;
    }

    private UpgradeData FindUpgradeDataById(string id)
    {
        foreach (var data in allUpgrades)
        {
            if (data.upgradeId == id)
                return data;
        }
        return null;
    }

    public void CloseUpgradeUI()
    {
        Cursor.visible = false;
        upgradeCanvas.SetActive(false);
        isGamePaused = false;
        Time.timeScale = 1f;
    }


    private void SetMaxExpForLevel(int level)
    {
        if (level - 1 < playerLevelData.expTable.Count)
        {
            maxPlayerExp = playerLevelData.expTable[level - 1];
        }
        else
        {
            Debug.LogWarning("레벨에 해당하는 경험치가 없습니다. maxPlayerExp를 무한으로 설정합니다.");
            maxPlayerExp = float.MaxValue;
        }
    }

    public void ApplyUpgrade(UpgradeType type)
    {
        switch (type)
        {
            case UpgradeType.IncreaseMaxHp:
                maxPlayerHp += 2f;
                playerHp += 2f;
                Debug.Log("체력 업그레이드! +2 HP");
                break;

            case UpgradeType.IncreaseDamage:
                weaponDamage += 1f;
                break;

            case UpgradeType.IncreaseFireRate:
                break;

            default:
                Debug.LogWarning("알 수 없는 업그레이드 타입");
                break;
        }
    }




}