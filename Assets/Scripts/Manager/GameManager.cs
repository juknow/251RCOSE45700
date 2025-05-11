using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private PlayerLevelData playerLevelData;

    [SerializeField] private StageManager stageManager;
    [SerializeField] private StageData[] allStages;

    private int currentStageIndex = 0;

    public float weaponDamage = 1f;
    public float playerHp = 3f;
    public float maxPlayerHp = 3f;

    public float playerExp = 0f;
    public float maxPlayerExp = 3f;

    public int playerLevel = 1;



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

    void Start()
    {
        Cursor.visible = false;
        playerLevel = playerLevelData.startingLevel;
        playerExp = 0f;

        maxPlayerHp = playerHp;
        playerHpSlider.maxValue = 1f;
        

        SetMaxExpForLevel(playerLevel);


        StartStage(currentStageIndex);
    }

    void Update()
    {
            playerHpSlider.value = playerHp / maxPlayerHp;

            playerExpSlider.value = playerExp / maxPlayerExp;
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
        StartStage(currentStageIndex);
    }

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
            OpenUpgradeUI();
        }

        Debug.Log($"[레벨 {playerLevel}] EXP: {playerExp:F1} / {maxPlayerExp:F1}");
    }

    private void OpenUpgradeUI()
    {
        Cursor.visible = true;
        Time.timeScale = 0f;
        upgradeCanvas.SetActive(true);

        List<UpgradeData> selected = GetRandomUpgrades(3);

        for (int i = 0; i < upgradeContainers.Length; i++)
        {
            upgradeContainers[i].SetUpgrade(selected[i]);
        }
    }

    private List<UpgradeData> GetRandomUpgrades(int count)
    {
        List<UpgradeData> result = new List<UpgradeData>();
        List<UpgradeData> pool = new List<UpgradeData>(allUpgrades);

        for (int i = 0; i < count && pool.Count > 0; i++)
        {
            int index = Random.Range(0, pool.Count);
            result.Add(pool[index]);
            pool.RemoveAt(index);
        }

        return result;
    }

    public void CloseUpgradeUI()
    {
        Cursor.visible = false;
        upgradeCanvas.SetActive(false);
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