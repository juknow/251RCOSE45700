using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public bool isGamePaused = false;

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

    [SerializeField] private UpgradeData[] allUpgrades; // ��ü ���׷��̵� ���
    [SerializeField] private GameObject upgradeCanvas; // UpgradeCanvas ��ü
    [SerializeField] private UpgradeContainer[] upgradeContainers; // 3�� �����̳� ����


    [SerializeField] private Slider feedbackSlider; // UI ����
    [SerializeField] private float feedbackDecaySpeed = 1f; // ������ ���� �ӵ�
    [SerializeField] private float feedbackGainMultiplier = 1f; // �̵��� -> ������ ������ ����
    [SerializeField] private float maxFeedback = 100f;
    private float currentFeedback = 0f;

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

        UpdateFeedback();
    }

    public void AddFeedback(float deltaX)
    {
        float gain = deltaX * feedbackGainMultiplier;
        currentFeedback = Mathf.Clamp(currentFeedback + gain, 0, maxFeedback);
    }

    private void UpdateFeedback()
    {
        if (currentFeedback > 0)
        {
            currentFeedback -= feedbackDecaySpeed * Time.deltaTime;
            currentFeedback = Mathf.Max(0, currentFeedback);
        }

        feedbackSlider.value = currentFeedback / maxFeedback;
    }

    public float GetWeaponDamage()
    {
        float feedbackBonus = (currentFeedback / maxFeedback); // ��: 0 ~ 1
        return weaponDamage * (1f + feedbackBonus); // �ִ� 2����� ����
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
            Debug.Log("���� ��ü �Ϸ�!");
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
            // ���⿡ ���� ���� ó�� �߰� ����
        }
    }

    public void AddExp(float amount)
    {
        playerExp += amount;
        Debug.Log($" ����ġ ȹ��: +{amount}");

        while (playerExp >= maxPlayerExp && playerLevel < playerLevelData.expTable.Count)
        {
            playerExp -= maxPlayerExp;
            playerLevel++;

            SetMaxExpForLevel(playerLevel);
            Debug.Log($"������! ���� ����: {playerLevel}");
            OpenUpgradeUI();
        }

        Debug.Log($"[���� {playerLevel}] EXP: {playerExp:F1} / {maxPlayerExp:F1}");
    }

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
            Debug.LogWarning("������ �ش��ϴ� ����ġ�� �����ϴ�. maxPlayerExp�� �������� �����մϴ�.");
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
                Debug.Log("ü�� ���׷��̵�! +2 HP");
                break;

            case UpgradeType.IncreaseDamage:
                weaponDamage += 1f;
                break;

            case UpgradeType.IncreaseFireRate:
                break;

            default:
                Debug.LogWarning("�� �� ���� ���׷��̵� Ÿ��");
                break;
        }
    }




}