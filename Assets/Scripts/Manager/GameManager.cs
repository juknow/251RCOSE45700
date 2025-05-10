using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] enemies;  // 프리팹 배열

    private Transform[] arrEnemyTransform;  // SpawnPoint들의 Transform (글로벌 기준)

    [SerializeField]
    private Transform spawnContainer;  // SpawnContainer 오브젝트 (부모)

    [SerializeField]
    private float spawnInterval = 3f;  // 생성 간격

    // Start is called before the first frame update
    void Start()
    {
        // SpawnContainer의 자식들만 배열로 저장 (자기 자신 제외)
        arrEnemyTransform = new Transform[spawnContainer.childCount];
        for (int i = 0; i < spawnContainer.childCount; i++)
        {
            arrEnemyTransform[i] = spawnContainer.GetChild(i);
        }

        StartCoroutine(EnemyRoutine());
    }

    IEnumerator EnemyRoutine()
    {
        yield return new WaitForSeconds(3f);  // 처음 시작 전 대기

        while (true)
        {
            foreach (Transform t in arrEnemyTransform)
            {
                int index = Random.Range(0, enemies.Length);  // 0~2
                SpawnEnemy(t.position, index);
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnEnemy(Vector3 globalPos, int index)
    {
        Vector3 spawnPos = new Vector3(globalPos.x, spawnContainer.position.y, 0f);  // X는 스폰포인트, Y는 GameManager 기준
        Instantiate(enemies[index], spawnPos, Quaternion.identity);
    }


}
