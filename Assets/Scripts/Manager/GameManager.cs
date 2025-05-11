using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private StageManager stageManager;
    [SerializeField] private StageData[] allStages;

    private int currentStageIndex = 0;

    public float weaponDamage = 1f;
    public float playerHp = 3f;
    private float maxPlayerHp;

    [SerializeField]
    private Slider playerHpSlider;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        maxPlayerHp = playerHp;
        playerHpSlider.maxValue = maxPlayerHp;
        playerHpSlider.value = playerHp;

        StartStage(currentStageIndex);
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
}