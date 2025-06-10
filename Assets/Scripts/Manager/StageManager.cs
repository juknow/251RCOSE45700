using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Mirror;

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

        // 스폰 위치 캐시
        spawnPoints = new Transform[spawnContainer.childCount];
        for (int i = 0; i < spawnContainer.childCount; i++)
            spawnPoints[i] = spawnContainer.GetChild(i);

        StartCoroutine(RunStage());
    }

    [Server]
    IEnumerator RunStage()
    {
        yield return new WaitForSeconds(2f); // 초기 대기

        for (int wave = 0; wave < currentStage.totalWaves; wave++)
        {
            SpawnWave(wave);
            yield return new WaitForSeconds(currentStage.waveInterval);
        }

        Debug.Log($"[{currentStage.stageType}] 완료");
        OnStageCompleted?.Invoke();
        RpcStageCompleted();
    }

    [Server]
    void SpawnWave(int waveIndex)
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
            GameObject enemy = Instantiate(
                currentStage.enemyPrefabs[UnityEngine.Random.Range(0, currentStage.enemyPrefabs.Length)],
                spawnPoint.position,
                Quaternion.identity);
            NetworkServer.Spawn(enemy);
        }
    }

    [ClientRpc]
    void RpcStageCompleted()
    {
        if (isServer) return;
        OnStageCompleted?.Invoke();
    }
}