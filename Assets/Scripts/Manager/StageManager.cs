using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class StageManager : NetworkBehaviour
{
    [SerializeField] private Transform spawnContainer;
    private Transform[] spawnPoints;

    public event Action OnStageCompleted;

    private StageData currentStage;

    [Server]
    public void StartStage(StageData stageData)
    {
        currentStage = stageData;

        // 스폰 포인트 캐싱
        spawnPoints = new Transform[spawnContainer.childCount];
        for (int i = 0; i < spawnContainer.childCount; i++)
            spawnPoints[i] = spawnContainer.GetChild(i);

        StartCoroutine(RunStage());
    }

    [Server]
    private IEnumerator RunStage()
    {
        yield return new WaitForSeconds(2f); // 초기 딜레이

        for (int wave = 0; wave < currentStage.totalWaves; wave++)
        {
            SpawnWave(wave);
            yield return new WaitForSeconds(currentStage.waveInterval);
        }

        Debug.Log($"[{currentStage.stageType}] 스테이지 완료");

        OnStageCompleted?.Invoke();
        RpcStageCompleted();

        currentStage = null; // 명시적으로 클리어
    }

    [Server]
    private void SpawnWave(int waveIndex)
    {
        Debug.Log($"[StageManager] Wave {waveIndex + 1} 시작");

        List<int> usedIndices = new List<int>();

        for (int i = 0; i < currentStage.enemiesPerWave; i++)
        {
            int spawnIndex;
            do
            {
                spawnIndex = UnityEngine.Random.Range(0, spawnPoints.Length);
            } while (usedIndices.Contains(spawnIndex));

            usedIndices.Add(spawnIndex);
            Transform spawnPoint = spawnPoints[spawnIndex];

            GameObject enemyPrefab = currentStage.enemyPrefabs[UnityEngine.Random.Range(0, currentStage.enemyPrefabs.Length)];

            if (!NetworkClient.prefabs.ContainsValue(enemyPrefab))
            {
                Debug.LogWarning($"[SpawnWave] 프리팹 {enemyPrefab.name} 이 Mirror에 등록되지 않음!");
                continue;
            }

            GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
            NetworkServer.Spawn(enemy);

            Debug.Log($"[Server] Enemy 생성됨: {enemy.name}");
        }
    }

    [ClientRpc]
    private void RpcStageCompleted()
    {
        if (!isServer) // 서버는 이미 알고 있음
        {
            OnStageCompleted?.Invoke();
        }
    }
}
