using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    [SerializeField] private StageManager stageManager;
    [SerializeField] private StageData[] allStages;
    private int currentStageIndex = 0;

    [SyncVar]
    public bool isGamePaused = false;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        RegisterSpawnPrefabs();

        if (NetworkServer.active)
            StartStage(currentStageIndex);
    }

    [Server]                       // 서버 전용 진입점
    public void SetPause(bool paused)
    {
        if (isGamePaused == paused) return;

        isGamePaused = paused;
        RpcApplyPause(paused);     // 모든 클라이언트에 전달
        ApplyPauseLocal(paused);   // 서버 자신(호스트)도 즉시 반영
    }

    [ClientRpc]
    void RpcApplyPause(bool paused)
    {
        ApplyPauseLocal(paused);   // 각 클라이언트에서 실행
    }

    // Time.timeScale 변경은 로컬(서버·클라)에서 직접
    void ApplyPauseLocal(bool paused)
    {
        Time.timeScale = paused ? 0f : 1f;
        Cursor.visible = paused;   // 필요하면 커서 토글
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

    private void StartStage(int index)
    {
        if (index < allStages.Length)
        {
            StageData stage = allStages[index];
            Debug.Log($"[GameManager] Starting stage: {stage.stageType}");

            stageManager.OnStageCompleted += HandleStageCompleted;
            stageManager.StartStage(stage);
        }
        else
        {
            Debug.Log(" 모든 스테이지 완료!");
        }
    }

    private void HandleStageCompleted()
    {
        stageManager.OnStageCompleted -= HandleStageCompleted;
        currentStageIndex++;

        if (NetworkServer.active)
            StartStage(currentStageIndex);
    }
}
