using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject[] enemies;  // 프리팹 배열

    private float[] arrPosX = { -2.2f, -1.1f, 0f, 1.1f, 2.2f };  // 적이 등장할 X 위치

    [SerializeField]
    private float spawnInterval = 1.5f;  // 생성 간격

    void Start()
    {
        StartCoroutine(EnemyRoutine());
    }

    IEnumerator EnemyRoutine()
    {
        yield return new WaitForSeconds(3f);  // 처음 시작 전 대기 시간

        while (true)
        {
            foreach (float posX in arrPosX)
            {
                int index = Random.Range(0, enemies.Length);  // 랜덤한 적 고르기
                SpawnEnemy(posX, index);
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnEnemy(float posX, int index)
    {
        Vector3 spawnPos = new Vector3(posX, transform.position.y, 0f);
        Instantiate(enemies[index], spawnPos, Quaternion.identity);
    }
}
